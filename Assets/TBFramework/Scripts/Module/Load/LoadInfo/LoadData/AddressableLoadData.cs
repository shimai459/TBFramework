using System;
using System.Collections.Generic;
using TBFramework.Load.Addressables;
using TBFramework.Pool;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TBFramework.Load.LoadInfo
{
    public class AddressableLoadData<T> : BaseLoadData<T> where T : UnityEngine.Object
    {
        public string[] keys;
        UnityEngine.AddressableAssets.Addressables.MergeMode mode;
        public AddressableLoadData() { }

        public AddressableLoadData(UnityEngine.AddressableAssets.Addressables.MergeMode mode, List<string> keys)
        {
            this.Init(mode, keys);
        }

        public void Init(UnityEngine.AddressableAssets.Addressables.MergeMode mode, params string[] keys)
        {
            this.mode = mode;
            this.keys = keys;
        }

        public void Init(UnityEngine.AddressableAssets.Addressables.MergeMode mode, List<string> keys)
        {
            this.mode = mode;
            this.keys = keys.ToArray();
        }

        public static AddressableLoadData<T> GetNew(UnityEngine.AddressableAssets.Addressables.MergeMode mode, List<string> keys)
        {
            AddressableLoadData<T> data = CPoolManager.Instance.Pop<AddressableLoadData<T>>();
            data.Init(mode, keys);
            return data;
        }

        public override void DoLoad(Action<T> action, bool isAsync, LoadSceneParameters param)
        {
            if (typeof(T).Equals(typeof(SceneInstance)))
            {
                AddressablesManager.Instance.LoadSceneAsync(keys[0], action as Action<SceneInstance>, null, null, param.loadSceneMode);
            }
            else
            {
                if (keys.Length > 1)
                {
                    AddressablesManager.Instance.LoadsAsync<T>(this.mode, action, null, null, keys);
                }
                else
                {
                    AddressablesManager.Instance.LoadAsync<T>(keys[0], action);
                }
            }
        }

        public override void DoUnload(Action action, bool isDel, UnloadSceneOptions options)
        {
            if (typeof(T).Equals(typeof(SceneInstance)))
            {
                AddressablesManager.Instance.UnloadScene(keys[0], options, isDel);
            }
            else
            {
                if (keys.Length > 1)
                {
                    AddressablesManager.Instance.Unloads<T>(mode, isDel, keys);
                }
                else
                {
                    AddressablesManager.Instance.Unload<T>(keys[0], isDel);
                }
            }
            action?.Invoke();
        }

        public override void Reset()
        {
            this.keys = default;
            this.mode = default;
        }

    }
}
