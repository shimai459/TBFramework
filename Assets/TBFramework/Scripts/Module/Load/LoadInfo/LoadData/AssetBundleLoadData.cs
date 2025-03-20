
using System;
using TBFramework.Load.AssetBundles.Load;
using TBFramework.Pool;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TBFramework.Load.LoadInfo
{
    public class AssetBundleLoadData<T> : BaseLoadData<T> where T : UnityEngine.Object
    {
        public string pathURL;
        public string mainName;
        public string abName;
        public string resName;

        public AssetBundleLoadData() { }

        public AssetBundleLoadData(string pathURL, string mainName, string abName, string resName)
        {
            this.Init(pathURL, mainName, abName, resName);
        }

        public void Init(string pathURL, string mainName, string abName, string resName)
        {
            this.pathURL = pathURL;
            this.mainName = mainName;
            this.abName = abName;
            this.resName = resName;
        }

        public static AssetBundleLoadData<T> GetNew(string pathURL, string mainName, string abName, string resName)
        {
            AssetBundleLoadData<T> data = CPoolManager.Instance.Pop<AssetBundleLoadData<T>>();
            data.Init(pathURL, mainName, abName, resName);
            return data;
        }

        public override void DoLoad(Action<T> action, bool isAsync, LoadSceneParameters param)
        {
            if (typeof(T).Equals(typeof(SceneInstance)))
            {
                if (isAsync)
                {
                    ABLoadManager.Instance.LoadSceneAsync(abName, resName, pathURL, mainName, param, isAsync, null, (a) => { action(null); });
                }
                else
                {
                    ABLoadManager.Instance.LoadScene(pathURL, mainName, abName, resName, param, () => { action(null); });
                }
            }
            else
            {
                if (isAsync)
                {
                    ABLoadManager.Instance.LoadResAsync<T>(abName, resName, pathURL, mainName, action, isAsync);
                }
                else
                {
                    T obj = ABLoadManager.Instance.LoadRes<T>(abName, resName, pathURL, mainName);
                    action(obj);
                }
            }
        }

        public override void DoUnload(Action action, bool isDel, UnloadSceneOptions options)
        {
            if (!typeof(T).Equals(typeof(SceneInstance)))
            {
                ABLoadManager.Instance.UnloadRes<T>(abName, resName, pathURL, mainName, action, null, isDel);
            }
            else
            {
                ABLoadManager.Instance.UnloadSceneAsync(resName, options, null, (a) => { action?.Invoke(); });
            }
        }

        public override void Reset()
        {
            this.pathURL = default;
            this.mainName = default;
            this.abName = default;
            this.resName = default;
        }


    }
}
