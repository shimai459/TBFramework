using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using TBFramework.AssetBundles;

namespace TBFramework.Lua.XLua
{
    public class XLuaManager : Singleton<XLuaManager>
    {

            // 唯一的XLua解析器
            private LuaEnv luaEnv;

            // Lua存放路径
            private List<string> paths=new List<string>();

            //Lua的AB包
            private List<string> abs=new List<string>();

            /// <summary>
            /// 获取_G
            /// </summary>
            /// <value></value>
            public LuaTable Global{
                get
                {
                    Init();
                    return luaEnv.Global;
                }
            }

            /// <summary>
            /// 内部执行添加Lua脚本路径
            /// </summary>
            private void AddPathIn(){
                //添加本地路径
                //TODO
                

                //添加AB包
                //TODO
                
            }

            /// <summary>
            /// 提供外部添加存放Lua脚本路径
            /// </summary>
            /// <param name="path">存放Lua脚本路径</param>
            public void AddPath(string path){
                if(!paths.Contains(path)){
                    paths.Add(path);
                }
            }

            /// <summary>
            /// 提供外部删除存放Lua脚本路径
            /// </summary>
            /// <param name="path">存放Lua脚本路径</param>
            public void RemovePath(string path){
                Init();
                if(paths.Contains(path)){
                    paths.Remove(path);
                }
            }

            /// <summary>
            /// 清空存放Lua脚本路径
            /// </summary>
            public void ClearPath(){
                Init();
                paths.Clear();
            }

            /// <summary>
            /// 提供外部添加LuaAB包
            /// </summary>
            /// <param name="abName">ab包名</param>
            public void AddABPath(string abName){
                if(!abs.Contains(abName)){
                    abs.Add(abName);
                }
            }

            /// <summary>
            /// 提供外部移出ab包
            /// </summary>
            /// <param name="abName">ab包名</param>
            public void RemoveABPath(string abName){
                Init();
                if(abs.Contains(abName)){
                    abs.Remove(abName);
                }
            }

            /// <summary>
            /// 去除所有AB包
            /// </summary>
            public void ClearABPath(){
                Init();
                abs.Clear();
            }

            /// <summary>
            /// 清除所有路径
            /// </summary>
            public void ClearAllPath(){
                ClearPath();
                ClearABPath();
            }

            /// <summary>
            /// XLua脚本路径重定向函数
            /// </summary>
            /// <param name="filePath">lua脚本名</param>
            /// <returns></returns>
            private byte[] CustomLoader(ref string filePath){
                //获取本地路径的Lua脚本
                if(!filePath.EndsWith(".lua")){
                    filePath+=".lua";
                }
                string path;
                foreach(string p in paths){
                    path=Path.Combine(p,filePath);
                    if(File.Exists(path)){
                        return File.ReadAllBytes(path);
                    }
                }
                //获取AB中的Lua脚本
                foreach(string ab in abs){
                    TextAsset lua = ABManager.Instance.LoadRes<TextAsset>(ab,filePath);
                    if(lua!=null){
                        return lua.bytes;
                    }
                }
                return null;
            }

            /// <summary>
            /// 初始化Lua管理器
            /// </summary>
            public void Init(){
                if(luaEnv!=null){
                    return;
                }
                luaEnv=new LuaEnv();
                AddPathIn();
                luaEnv.AddLoader(CustomLoader);
            }

            /// <summary>
            /// 执行Lua脚本
            /// </summary>
            /// <param name="luaFileName">Lua脚本名</param>
            public object[] DoLuaFile(string luaFileName,string chunkName="chunk",LuaTable env=null){
                Init();
                return luaEnv.DoString($"require('{luaFileName}')",chunkName,env);
            }

            /// <summary>
            /// 执行Luau语句
            /// </summary>
            /// <param name="chunk">Lua语句</param>
            public object[] DoString(string chunk,string chunkName="chunk",LuaTable env=null){
                Init();
                return luaEnv.DoString(chunk,chunkName,env);
            }

            /// <summary>
            /// 执行Lua垃圾回收
            /// </summary>
            public void Tick(){
                Init();
                luaEnv.Tick();
            }

            /// <summary>
            /// 销毁Lua解析器
            /// </summary>
            public void Dispose(){
                if(luaEnv==null){
                    return;
                }
                luaEnv.Dispose();
                luaEnv=null;
            }
    }
}
