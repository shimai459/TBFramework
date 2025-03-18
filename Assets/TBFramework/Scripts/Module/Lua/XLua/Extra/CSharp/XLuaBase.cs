
using UnityEngine;


namespace TBFramework.Lua.XLua
{
    public abstract class XLuaBase : MonoBehaviour
    {
        public abstract void DoAllLiftCycleFunction(string functionName, params object[] args);

        void Awake()
        {
            DoAllLiftCycleFunction(LuaComponentFunctionName.Awake);
        }

        void Start()
        {
            DoAllLiftCycleFunction(LuaComponentFunctionName.Start);
        }

        void OnEnable()
        {
            DoAllLiftCycleFunction(LuaComponentFunctionName.OnEnable);
        }

        void FixedUpdate()
        {
            DoAllLiftCycleFunction(LuaComponentFunctionName.FixedUpdate);
        }

        void Update()
        {
            DoAllLiftCycleFunction(LuaComponentFunctionName.Update);
        }

        void LateUpdate()
        {
            DoAllLiftCycleFunction(LuaComponentFunctionName.LateUpdate);
        }

        void OnDisable()
        {
            DoAllLiftCycleFunction(LuaComponentFunctionName.OnDisable);
        }

        void OnApplicationFocus(bool focus)
        {
            DoAllLiftCycleFunction(LuaComponentFunctionName.OnApplicationFocus, focus);
        }

        void OnApplicationPause(bool pause)
        {
            DoAllLiftCycleFunction(LuaComponentFunctionName.OnApplicationPause, pause);
        }

        void OnApplicationQuit()
        {
            DoAllLiftCycleFunction(LuaComponentFunctionName.OnApplicationQuit);
        }

        protected abstract void OnDestroy();
    }
}
