using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using TBFramework.AssetBundles;
using TBFramework.Pool;
using JetBrains.Annotations;
using System;
using TBFramework.Event;

namespace TBFramework.Lua.XLua
{
    public class XLuaManager : Singleton<XLuaManager>
    {

        // 唯一的XLua解析器
        private LuaEnv luaEnv;

        // Lua存放路径
        private List<string> paths = new List<string>();

        //Lua的AB包
        private List<string> abs = new List<string>();

        /// <summary>
        /// 获取_G
        /// </summary>
        /// <value></value>
        public LuaTable Global
        {
            get
            {
                Init();
                return luaEnv.Global;
            }
        }

        /// <summary>
        /// 内部执行添加Lua脚本路径
        /// </summary>
        private void AddPathIn()
        {
            //添加本地路径
            //TODO
            paths.Add(Path.Combine(Application.dataPath, "TBFramework", "Scripts", "Module", "Lua", "XLua", "Extra", "Lua"));

            //添加AB包
            //TODO

        }

        /// <summary>
        /// 提供外部添加存放Lua脚本路径
        /// </summary>
        /// <param name="path">存放Lua脚本路径</param>
        public void AddPath(string path)
        {
            if (!paths.Contains(path))
            {
                paths.Add(path);
            }
        }

        /// <summary>
        /// 提供外部删除存放Lua脚本路径
        /// </summary>
        /// <param name="path">存放Lua脚本路径</param>
        public void RemovePath(string path)
        {
            Init();
            if (paths.Contains(path))
            {
                paths.Remove(path);
            }
        }

        /// <summary>
        /// 清空存放Lua脚本路径
        /// </summary>
        public void ClearPath()
        {
            Init();
            paths.Clear();
        }

        /// <summary>
        /// 提供外部添加LuaAB包
        /// </summary>
        /// <param name="abName">ab包名</param>
        public void AddABPath(string abName)
        {
            if (!abs.Contains(abName))
            {
                abs.Add(abName);
            }
        }

        /// <summary>
        /// 提供外部移出ab包
        /// </summary>
        /// <param name="abName">ab包名</param>
        public void RemoveABPath(string abName)
        {
            Init();
            if (abs.Contains(abName))
            {
                abs.Remove(abName);
            }
        }

        /// <summary>
        /// 去除所有AB包
        /// </summary>
        public void ClearABPath()
        {
            Init();
            abs.Clear();
        }

        /// <summary>
        /// 清除所有路径
        /// </summary>
        public void ClearAllPath()
        {
            ClearPath();
            ClearABPath();
        }

        /// <summary>
        /// XLua脚本路径重定向函数
        /// </summary>
        /// <param name="filePath">lua脚本名</param>
        /// <returns></returns>
        private byte[] CustomLoader(ref string filePath)
        {
            //获取本地路径的Lua脚本
            if (!filePath.EndsWith(".lua"))
            {
                filePath += ".lua";
            }
            string path;
            foreach (string p in paths)
            {
                path = Path.Combine(p, filePath);
                if (File.Exists(path))
                {
                    return File.ReadAllBytes(path);
                }
            }
            //获取AB中的Lua脚本
            foreach (string ab in abs)
            {
                TextAsset lua = ABManager.Instance.LoadRes<TextAsset>(ab, filePath);
                if (lua != null)
                {
                    return lua.bytes;
                }
            }
            return null;
        }

        /// <summary>
        /// 初始化Lua管理器
        /// </summary>
        public void Init()
        {
            if (luaEnv != null)
            {
                return;
            }
            luaEnv = new LuaEnv();
            AddPathIn();
            luaEnv.AddLoader(CustomLoader);
        }

        /// <summary>
        /// 执行Lua脚本
        /// </summary>
        /// <param name="luaFileName">Lua脚本名</param>
        public object[] DoLuaFile(string luaFileName, string chunkName = "chunk", LuaTable env = null)
        {
            Init();
            return luaEnv.DoString($"require('{luaFileName}')", chunkName, env);
        }

        /// <summary>
        /// 执行Luau语句
        /// </summary>
        /// <param name="chunk">Lua语句</param>
        public object[] DoString(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            Init();
            return luaEnv.DoString(chunk, chunkName, env);
        }

        /// <summary>
        /// 执行Lua垃圾回收
        /// </summary>
        public void Tick()
        {
            Init();
            luaEnv.Tick();
        }

        /// <summary>
        /// 销毁Lua解析器
        /// </summary>
        public void Dispose()
        {
            if (luaEnv == null)
            {
                return;
            }
            luaEnv.Dispose();
            luaEnv = null;
        }

        #region 监听事件

        public void AddEventListenerNoParam<T>(string eventName, Action<T> action, T param)
        {
            EventCenter.Instance.AddEventListener(eventName, action, param);
        }

        public void AddEventListenerWithParam<T, K>(string eventName, Action<T, K> action, T param)
        {
            EventCenter.Instance.AddEventListener(eventName, action, param);
        }

        public void RemoveEventListenerNoParam<T>(string eventName, Action<T> action, T param)
        {
            EventCenter.Instance.RemoveEventListener(eventName, action, param);
        }

        public void RemoveEventListenerWithParam<T, K>(string eventName, Action<T, K> action, T param)
        {
            EventCenter.Instance.RemoveEventListener(eventName, action, param);
        }


        #endregion

        #region Lua挂载组件

        public void AddComponent(GameObject obj, LuaTable table, bool isEnable = true)
        {
            XLuaMounter LuaMounter = obj.GetComponent<XLuaMounter>();
            if (LuaMounter == null)
            {
                LuaMounter = obj.AddComponent<XLuaMounter>();
            }
            XLuaComponent luaComponent = CPoolManager.Instance.Pop<XLuaComponent>();
            luaComponent.Init(table, table.Get<string>("name"), obj, LuaMounter);
            LuaMounter.AddComponent(luaComponent);
            luaComponent.ChangeEnabled(isEnable);
        }

        public void AddComponents(GameObject obj, bool isEnable = true, params LuaTable[] tables)
        {
            for (int i = 0; i < tables.Length; i++)
            {
                AddComponent(obj, tables[i], isEnable);
            }
        }

        public void RemoveComponent(GameObject obj, LuaTable table)
        {
            XLuaMounter LuaMounter = obj.GetComponent<XLuaMounter>();
            if (LuaMounter == null)
            {
                return;
            }
            LuaMounter.RemoveComponent(table);
        }

        public void RemoveComponents(GameObject obj, params LuaTable[] tables)
        {
            XLuaMounter LuaMounter = obj.GetComponent<XLuaMounter>();
            if (LuaMounter == null)
            {
                return;
            }
            LuaMounter.RemoveComponents(tables);
        }

        public void RemoveComponentFirst(GameObject obj, string name)
        {
            XLuaMounter LuaMounter = obj.GetComponent<XLuaMounter>();
            if (LuaMounter == null)
            {
                return;
            }
            LuaMounter.RemoveComponentFirst(name);
        }

        public void RemoveComponentAll(GameObject obj, string name)
        {
            XLuaMounter LuaMounter = obj.GetComponent<XLuaMounter>();
            if (LuaMounter == null)
            {
                return;
            }
            LuaMounter.RemoveComponentAll(name);
        }

        public void RemoveComponentsFirst(GameObject obj, params string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                RemoveComponentFirst(obj, names[i]);
            }
        }

        public void RemoveComponentsAll(GameObject obj, params string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                RemoveComponentAll(obj, names[i]);
            }
        }

        public XLuaComponent GetComponent(GameObject obj, string name)
        {
            XLuaMounter LuaMounter = obj.GetComponent<XLuaMounter>();
            if (LuaMounter == null)
            {
                return null;
            }
            return LuaMounter.GetLuaComponentFirst(name);
        }

        public List<XLuaComponent> GetComponents(GameObject obj, string name)
        {
            XLuaMounter LuaMounter = obj.GetComponent<XLuaMounter>();
            if (LuaMounter == null)
            {
                return null;
            }
            return LuaMounter.GetLuaComponents(name);
        }

        public List<XLuaComponent> GetAllCompoents(GameObject obj)
        {
            XLuaMounter LuaMounter = obj.GetComponent<XLuaMounter>();
            if (LuaMounter == null)
            {
                return null;
            }
            return LuaMounter.GetAllCompoents();
        }

        public void AddSingleComponent(GameObject obj, LuaTable table, bool isEnable = true)
        {
            bool addMore = table.Get<bool>("addMore");
            string name = table.Get<string>("name");
            if (!addMore)
            {
                XLuaSingle[] luaSingles = obj.GetComponents<XLuaSingle>();
                for (int i = 0; i < luaSingles.Length; i++)
                {
                    if (luaSingles[i].luaComponent.name == name)
                    {
                        return;
                    }
                }
            }
            XLuaSingle luaSingle = obj.AddComponent<XLuaSingle>();
            XLuaComponent luaComponent = CPoolManager.Instance.Pop<XLuaComponent>();
            luaComponent.Init(table, name, obj, luaSingle);
            luaComponent.ChangeEnabled(isEnable);
            luaSingle.luaComponent = luaComponent;
        }

        public void AddSingleComponents(GameObject obj, bool isEnable = true, params LuaTable[] tables)
        {
            for (int i = 0; i < tables.Length; i++)
            {
                AddSingleComponent(obj, tables[i], isEnable);
            }
        }

        public void RemoveSingleComponent(GameObject obj, LuaTable table)
        {
            XLuaSingle[] luaSingle = obj.GetComponents<XLuaSingle>();
            for (int i = 0; i < luaSingle.Length; i++)
            {
                if (luaSingle[i].luaComponent.luaTable == table)
                {
                    GameObject.Destroy(luaSingle[i]);
                }
            }
        }

        public void RemoveSingleComponentFirst(GameObject obj, string name)
        {
            XLuaSingle[] luaSingle = obj.GetComponents<XLuaSingle>();
            for (int i = 0; i < luaSingle.Length; i++)
            {
                if (luaSingle[i].luaComponent.name == name)
                {
                    GameObject.Destroy(luaSingle[i]);
                    break;
                }
            }
        }

        public void RemoveSingleComponentAll(GameObject obj, string name)
        {
            XLuaSingle[] luaSingle = obj.GetComponents<XLuaSingle>();
            for (int i = 0; i < luaSingle.Length; i++)
            {
                if (luaSingle[i].luaComponent.name == name)
                {
                    GameObject.Destroy(luaSingle[i]);
                }
            }
        }

        public void RemoveSingleComponents(GameObject obj, params LuaTable[] tables)
        {
            for (int i = 0; i < tables.Length; i++)
            {
                RemoveSingleComponent(obj, tables[i]);
            }
        }

        public void RemoveSingleComponentsFirst(GameObject obj, params string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                RemoveSingleComponentFirst(obj, names[i]);
            }
        }

        public void RemoveSingleComponentsAll(GameObject obj, params string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                RemoveSingleComponentAll(obj, names[i]);
            }
        }

        public XLuaComponent GetSingleComponent(GameObject obj, string name)
        {
            XLuaSingle[] luaSingle = obj.GetComponents<XLuaSingle>();
            for (int i = 0; i < luaSingle.Length; i++)
            {
                if (luaSingle[i].luaComponent.name == name)
                {
                    return luaSingle[i].luaComponent;
                }
            }
            return null;
        }

        public List<XLuaComponent> GetSingleComponents(GameObject obj, string name)
        {
            XLuaSingle[] luaSingle = obj.GetComponents<XLuaSingle>();
            List<XLuaComponent> list = new List<XLuaComponent>();
            for (int i = 0; i < luaSingle.Length; i++)
            {
                if (luaSingle[i].luaComponent.name == name)
                {
                    list.Add(luaSingle[i].luaComponent);
                }
            }
            return list;
        }

        public List<XLuaComponent> GetAllSingleComponents(GameObject obj)
        {
            XLuaSingle[] luaSingle = obj.GetComponents<XLuaSingle>();
            List<XLuaComponent> list = new List<XLuaComponent>();
            for (int i = 0; i < luaSingle.Length; i++)
            {
                list.Add(luaSingle[i].luaComponent);
            }
            return list;
        }

        #endregion
    }
}
