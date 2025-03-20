using System;
using TBFramework.Pool;
using UnityEngine.SceneManagement;

namespace TBFramework.Load.LoadInfo
{
    public class CustomLoadData<T> : BaseLoadData<T> where T : UnityEngine.Object
    {
        public Action<Action<T>, bool, LoadSceneParameters> load;
        public Action<Action, bool, UnloadSceneOptions> unload;

        public CustomLoadData() { }

        public CustomLoadData(Action<Action<T>, bool, LoadSceneParameters> load, Action<Action, bool, UnloadSceneOptions> unload)
        {
            this.Init(load, unload);
        }

        public void Init(Action<Action<T>, bool, LoadSceneParameters> load, Action<Action, bool, UnloadSceneOptions> unload)
        {
            this.load = load;
            this.unload = unload;
        }

        public static CustomLoadData<T> GetNew(Action<Action<T>, bool, LoadSceneParameters> load, Action<Action, bool, UnloadSceneOptions> unload)
        {
            CustomLoadData<T> data = CPoolManager.Instance.Pop<CustomLoadData<T>>();
            data.Init(load, unload);
            return data;
        }

        public override void DoLoad(Action<T> action, bool isAsync, LoadSceneParameters param)
        {
            load(action, isAsync, param);
        }

        public override void DoUnload(Action action, bool isAsync, UnloadSceneOptions options)
        {
            unload(action, isAsync, options);
        }

        public override void Reset()
        {
            load = null;
            unload = null;
        }
    }
}
