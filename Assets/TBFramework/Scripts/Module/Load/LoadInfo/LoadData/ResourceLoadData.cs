using System;
using TBFramework.Pool;
using TBFramework.Load.Resource;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TBFramework.Load.LoadInfo
{
    public class ResourceLoadData<T> : BaseLoadData<T> where T : UnityEngine.Object
    {
        public string path;

        public ResourceLoadData() { }

        public ResourceLoadData(string path)
        {
            this.Init(path);
        }

        public void Init(string path)
        {
            this.path = path;
        }

        public static ResourceLoadData<T> GetNew(string path)
        {
            ResourceLoadData<T> data = CPoolManager.Instance.Pop<ResourceLoadData<T>>();
            data.Init(path);
            return data;
        }

        public override void DoLoad(Action<T> action, bool isAsync, LoadSceneParameters param)
        {
            if (typeof(T).Equals(typeof(SceneInstance)))
            {
                if (isAsync)
                {
                    TBFramework.Load.Scene.SceneManager.Instance.LoadSceneAsync(path, param, null, (a) => { action?.Invoke(null); });
                }
                else
                {
                    TBFramework.Load.Scene.SceneManager.Instance.LoadScene(path, param, () => action?.Invoke(null));
                }
            }
            if (isAsync)
            {
                ResourceManager.Instance.LoadAsync<T>(path, action);
            }
            else
            {
                T obj = ResourceManager.Instance.Load<T>(path);
                action(obj);
            }
        }

        public override void DoUnload(Action action, bool isDel, UnloadSceneOptions options)
        {
            if (!typeof(T).Equals(typeof(SceneInstance)))
            {
                ResourceManager.Instance.UnloadAsset<T>(path, null, isDel);
            }
            else
            {
                TBFramework.Load.Scene.SceneManager.Instance.UnloadSceneAsync(path, options, null, (a) => { action?.Invoke(); });
            }

        }

        public override void Reset()
        {
            this.path = null;
        }

    }
}
