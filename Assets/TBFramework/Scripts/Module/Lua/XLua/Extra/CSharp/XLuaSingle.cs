
using UnityEngine;


namespace TBFramework.Lua.XLua
{
    public class XLuaSingle : XLuaBase
    {
        public XLuaComponent luaComponent;

        public void DoFunction(string functionName, params object[] args)
        {
            luaComponent.DoFunction(functionName, args);
        }

        public override void DoAllLiftCycleFunction(string functionName, params object[] args)
        {
            if (luaComponent != null && luaComponent.enabled)
            {
                if ((functionName == LuaComponentFunctionName.Start && luaComponent.isStart) || (functionName == LuaComponentFunctionName.Awake && luaComponent.isAwake))
                {
                    return;
                }
                luaComponent.DoFunction(functionName, args);
            }
        }

        protected override void OnDestroy()
        {
            this.luaComponent.Destroy();
        }

    }
}
