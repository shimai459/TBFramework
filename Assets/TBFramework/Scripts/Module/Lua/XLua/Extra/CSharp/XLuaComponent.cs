using System.Collections.Generic;
using TBFramework.Pool;
using UnityEngine;
using XLua;

namespace TBFramework.Lua.XLua
{
    public class XLuaComponent : CBase
    {
        public string name;
        public bool enabled = false;

        public bool isAwake = false;
        public bool isStart = false;
        public LuaTable luaTable;
        private Dictionary<string, LuaFunction> functions = new Dictionary<string, LuaFunction>();
        private GameObject gameObject;
        private XLuaBase mounter;

        public XLuaComponent() { }

        public XLuaComponent(LuaTable luaTable, string name, GameObject gameObject, XLuaBase mounter)
        {
            this.Init(luaTable, name, gameObject, mounter);
        }

        public void Init(LuaTable luaTable, string name, GameObject gameObject, XLuaBase mounter)
        {
            this.luaTable = luaTable;
            this.name = name;
            this.gameObject = gameObject;
            this.mounter = mounter;
        }

        public void Destroy()
        {
            ChangeEnabled(false);
            DoFunction(LuaComponentFunctionName.OnDestroy);
            CPoolManager.Instance.Push(this);
        }

        public void ChangeEnabled(bool enabled)
        {
            if (this.enabled != enabled)
            {
                this.enabled = enabled;
                if (this.gameObject.activeSelf && this.mounter.enabled)
                {
                    if (enabled)
                    {
                        if (!isAwake)
                        {
                            DoFunction(LuaComponentFunctionName.Awake);
                            isAwake = true;
                        }
                        DoFunction(LuaComponentFunctionName.OnEnable);
                        if (!isStart)
                        {
                            DoFunction(LuaComponentFunctionName.Start);
                            isStart = true;
                        }
                    }
                    else
                    {

                        DoFunction(LuaComponentFunctionName.OnDisable);

                    }
                }
            }
        }

        public void DoFunction(string name, params object[] args)
        {
            if (functions.ContainsKey(name))
            {
                functions[name]?.Call(luaTable, gameObject, args);
                return;
            }
            LuaFunction func = luaTable.Get<LuaFunction>(name);
            if (func != null)
            {
                functions.Add(name, func);
                func.Call(luaTable, gameObject, args);
            }
        }

        public override void Reset()
        {
            name = default;
            enabled = false;
            isAwake = false;
            isStart = false;
            luaTable = null;
            gameObject = null;
            mounter = null;
            functions.Clear();
        }

    }
}