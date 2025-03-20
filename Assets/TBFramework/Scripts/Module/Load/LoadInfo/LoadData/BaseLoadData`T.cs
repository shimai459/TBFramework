

using System;
using UnityEngine.SceneManagement;

namespace TBFramework.Load.LoadInfo
{
    public abstract class BaseLoadData<T> : BaseLoadData where T : UnityEngine.Object
    {
        public abstract void DoLoad(Action<T> action, bool isAsync, LoadSceneParameters param);

        public abstract void DoUnload(Action action, bool isDel, UnloadSceneOptions options);
    }
}
