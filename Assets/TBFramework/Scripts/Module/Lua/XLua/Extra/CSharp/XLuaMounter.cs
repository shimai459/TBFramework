
using System.Collections.Generic;
using TBFramework.Pool;
using UnityEngine;
using XLua;

namespace TBFramework.Lua.XLua
{
    [DisallowMultipleComponent]
    public class XLuaMounter : XLuaBase
    {

        public Dictionary<string, List<XLuaComponent>> compoents = new Dictionary<string, List<XLuaComponent>>();

        public void AddComponent(XLuaComponent luaComponent)
        {
            string name = luaComponent.luaTable.Get<string>("name");
            if (!compoents.ContainsKey(name))
            {
                compoents.Add(name, new List<XLuaComponent>());
            }
            bool addMore = luaComponent.luaTable.Get<bool>("addMore");
            if (addMore || compoents[name].Count == 0)
            {
                compoents[name].Add(luaComponent);
            }
        }

        public void AddComponents(params XLuaComponent[] luaComponents)
        {
            for (int i = 0; i < luaComponents.Length; i++)
            {
                AddComponent(luaComponents[i]);
            }
        }

        public void RemoveComponent(LuaTable luaTable)
        {
            string name = luaTable.Get<string>("name");
            if (compoents.ContainsKey(name))
            {
                for (int i = 0; i < compoents[name].Count; i++)
                {
                    if (compoents[name][i].luaTable == luaTable)
                    {
                        compoents[name][i].Destroy();
                        compoents[name].RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void RemoveComponents(params LuaTable[] luaTables)
        {
            for (int i = 0; i < luaTables.Length; i++)
            {
                RemoveComponent(luaTables[i]);
            }
        }

        public void RemoveComponent(XLuaComponent luaComponent)
        {
            string name = luaComponent.luaTable.Get<string>("name");
            if (compoents.ContainsKey(name) && compoents[name].Contains(luaComponent))
            {
                compoents[name].Remove(luaComponent);
                luaComponent.Destroy();
            }
        }

        public void RemoveComponents(params XLuaComponent[] luaComponents)
        {
            for (int i = 0; i < luaComponents.Length; i++)
            {
                RemoveComponent(luaComponents[i]);
            }
        }

        public void RemoveComponentFirst(string componentName)
        {
            if (compoents.ContainsKey(componentName) && compoents[componentName].Count > 0)
            {
                compoents[componentName][0].Destroy();
                compoents[componentName].RemoveAt(0);
            }
        }

        public void RemoveComponentAll(string componentName)
        {
            if (compoents.ContainsKey(componentName))
            {
                for (int i = 0; i < compoents[componentName].Count; i++)
                {
                    compoents[componentName][i].Destroy();
                }
                compoents[componentName].Clear();
                compoents.Remove(componentName);
            }
        }

        public void DoFirstComponentFunction(string componentName, string functionName, params object[] args)
        {
            if (compoents.ContainsKey(componentName) && compoents[componentName].Count > 0)
            {
                compoents[componentName][0].DoFunction(functionName, args);
            }
        }

        public void DoOneComponentFunction(string componentName, string functionName, params object[] args)
        {
            if (compoents.ContainsKey(componentName))
            {
                foreach (var luaComponent in compoents[componentName])
                {
                    luaComponent.DoFunction(functionName, args);
                }
            }
        }

        public void DoAllFunction(string functionName, params object[] args)
        {
            foreach (var item in compoents.Values)
            {
                foreach (var luaComponent in item)
                {
                    luaComponent.DoFunction(functionName, args);
                }
            }
        }

        public override void DoAllLiftCycleFunction(string functionName, params object[] args)
        {
            foreach (var item in compoents.Values)
            {
                foreach (var luaComponent in item)
                {
                    if (luaComponent.enabled)
                    {
                        if ((functionName == LuaComponentFunctionName.Start && luaComponent.isStart) || (functionName == LuaComponentFunctionName.Awake && luaComponent.isAwake))
                        {
                            continue;
                        }
                        luaComponent.DoFunction(functionName, args);
                    }
                }
            }
        }

        public XLuaComponent GetLuaComponentFirst(string componentName)
        {
            if (compoents.ContainsKey(componentName) && compoents[componentName].Count > 0)
            {
                return compoents[componentName][0];
            }
            return null;
        }

        public List<XLuaComponent> GetLuaComponents(string componentName)
        {
            if (compoents.ContainsKey(componentName) && compoents[componentName].Count > 0)
            {
                return compoents[componentName];
            }
            return null;
        }

        public List<XLuaComponent> GetAllCompoents()
        {
            List<XLuaComponent> result = new List<XLuaComponent>();
            foreach (var item in compoents.Values)
            {
                result.AddRange(item);
            }
            return result;
        }

        public void Clear()
        {
            foreach (var item in compoents.Values)
            {
                foreach (var luaComponent in item)
                {
                    luaComponent.Destroy();
                }
            }
            compoents.Clear();
        }

        protected override void OnDestroy()
        {
            this.Clear();
        }
    }
}