using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBFramework.Mono;

namespace TBFramework.AssetBundles
{
    /// <summary>
    /// AB包的一个套系
    /// </summary>
    public class ABSingle
    {
        private AssetBundle mainAB = null;//该套系的主包
        private AssetBundleManifest abManifest = null;//该套系的依赖
        private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();//该套系已加载的AB包
        private List<string> abAsyncList = new List<string>();//该套系正在异步加载的AB包
        private int asyncCount = 0;//该套系中正在异步加载的数量
        public int AsyncCount { get => asyncCount; }

        private string mainName;//该套系的主包名

        private string pathURL;//该套系存放的路径

        public ABSingle(string pathURL, string mainName)
        {
            this.pathURL = pathURL;
            this.mainName = mainName;
        }

        /// <summary>
        /// 加载多个AB包
        /// </summary>
        /// <param name="abNames">AB包名数组</param>
        public void LoadABs(params string[] abNames)
        {
            foreach (string abName in abNames)
            {
                LoadAB(abName);
            }
        }

        /// <summary>
        /// 加载AB包
        /// </summary>
        /// <param name="abName">AB包名</param>
        public void LoadAB(string abName)
        {
            //如果还没有加载主包,则先加载主包
            if (mainAB == null)
            {
                //判断异步加载是否已经在加载主包,如果异步加载正在加载主包,则直接使用异步加载包,防止重复加载
                if (!abAsyncList.Contains(mainName))
                {
                    mainAB = AssetBundle.LoadFromFile(System.IO.Path.Combine(pathURL, mainName));
                    abManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                }
                else
                {
                    LoadABAsync(abName);
                    return;
                }
            }
            //如果主包加载了,但是依赖信息没有,则加载依赖信息
            if (abManifest == null)
            {
                abManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            AssetBundle ab;
            //获取加载包的所有依赖包名
            string[] abDependence = abManifest.GetAllDependencies(abName);
            for (int i = 0; i < abDependence.Length; i++)
            {
                if (!abDic.ContainsKey(abDependence[i]))
                {
                    //判断异步加载是否已经在加载依赖包,如果异步加载正在加载依赖包,则直接使用异步加载包,防止重复加载
                    if (!abAsyncList.Contains(abDependence[i]))
                    {
                        ab = AssetBundle.LoadFromFile(System.IO.Path.Combine(pathURL, abDependence[i]));
                        abDic.Add(abDependence[i], ab);
                    }
                    else
                    {
                        LoadABAsync(abName);
                        return;
                    }
                }
            }
            if (!abDic.ContainsKey(abName))
            {
                //判断异步加载是否已经在加载AB包,如果异步加载正在加载AB包,则直接使用异步加载包,防止重复加载
                if (!abAsyncList.Contains(abName))
                {
                    ab = AssetBundle.LoadFromFile(System.IO.Path.Combine(pathURL, abName));
                    abDic.Add(abName, ab);
                }
                else
                {
                    LoadABAsync(abName);
                    return;
                }
            }
        }

        /// <summary>
        /// 同步加载资源,不指定类型的方法
        /// </summary>
        /// <param name="abName">AB包的名字</param>
        /// <param name="resName">资源的名字</param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName)
        {
            LoadAB(abName);
            if (abDic.ContainsKey(abName))
            {
                return abDic[abName].LoadAsset(resName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 同步加载,用type指定类型的方法
        /// </summary>
        /// <param name="abName">包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="type">资源类型</param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName, System.Type type)
        {
            LoadAB(abName);
            if (abDic.ContainsKey(abName))
            {
                return abDic[abName].LoadAsset(resName, type);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 同步加载,用泛型指定类型的方法
        /// </summary>
        /// <param name="abName">包名</param>
        /// <param name="resName">资源名</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public T LoadRes<T>(string abName, string resName) where T : Object
        {
            LoadAB(abName);
            if (abDic.ContainsKey(abName))
            {
                return abDic[abName].LoadAsset<T>(resName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 异步加载多个AB包
        /// </summary>
        /// <param name="abNames">AB包名数组</param>
        public void LoadABsAsync(params string[] abNames)
        {
            foreach (string abName in abNames)
            {
                LoadABAsync(abName);
            }
        }

        /// <summary>
        /// 提供给外部异步加载AB包的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        public void LoadABAsync(string abName, System.Action<AssetBundle> callBack = null)
        {
            MonoManager.Instance.StartCoroutine(ReallyLoadABAsync(abName, callBack));
        }

        /// <summary>
        /// 真正异步加载AB包的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <returns></returns>
        private IEnumerator ReallyLoadABAsync(string abName, System.Action<AssetBundle> callBack)
        {
            asyncCount++;
            while (mainAB == null)
            {
                //防止其他异步程序加载主包,导致重复加载
                if (!abAsyncList.Contains(mainName))
                {
                    abAsyncList.Add(mainName);
                    AssetBundleCreateRequest main = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(pathURL, mainName));
                    yield return main;
                    mainAB = main.assetBundle;
                    abAsyncList.Remove(mainName);
                    AssetBundleRequest manifest = mainAB.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                    yield return manifest;
                    abManifest = manifest.asset as AssetBundleManifest;
                }
                yield return null;
            }
            //如果主包加载了,但是依赖信息没有,则加载依赖信息
            if (abManifest == null)
            {
                AssetBundleRequest manifest = mainAB.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                yield return manifest;
                abManifest = manifest.asset as AssetBundleManifest;
            }
            AssetBundleCreateRequest ab;
            AssetBundle rAB;
            //获取加载包的所有依赖包名
            string[] abDependence = abManifest.GetAllDependencies(abName);
            for (int i = 0; i < abDependence.Length; i++)
            {
                while (!abDic.ContainsKey(abDependence[i]))
                {
                    if (!abAsyncList.Contains(abDependence[i]))
                    {
                        abAsyncList.Add(abDependence[i]);
                        ab = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(pathURL, abDependence[i]));
                        yield return ab;
                        rAB = ab.assetBundle;
                        abDic.Add(abDependence[i], rAB);
                        abAsyncList.Remove(abDependence[i]);
                    }
                    yield return null;
                }
            }
            while (!abDic.ContainsKey(abName))
            {
                if (!abAsyncList.Contains(abName))
                {
                    abAsyncList.Add(abName);
                    ab = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(pathURL, abName));
                    yield return ab;
                    rAB = ab.assetBundle;
                    abDic.Add(abName, rAB);
                    abAsyncList.Remove(abName);
                }
                yield return null;
            }
            callBack?.Invoke(abDic[abName]);
            asyncCount--;
        }

        /// <summary>
        /// 提供给外部不指定类型的异步加载AB包资源的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        public void LoadResAsync(string abName, string resName, System.Action<Object> callBack, bool isABAsync = true)
        {
            MonoManager.Instance.StartCoroutine(ReallyLoadResAsync(abName, resName, callBack, isABAsync));
        }

        /// <summary>
        /// 真正不指定类型的异步加载AB包资源的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        /// <returns></returns>
        private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Action<Object> callBack, bool isABAsync)
        {
            if (isABAsync)
            {
                yield return MonoManager.Instance.StartCoroutine(ReallyLoadABAsync(abName, null));
            }
            else
            {
                LoadAB(abName);
            }
            while (!abDic.ContainsKey(abName))
            {
                yield return null;
            }
            AssetBundleRequest abRequest = abDic[abName].LoadAssetAsync(resName);
            yield return abRequest;
            callBack(abRequest.asset);
        }

        /// <summary>
        /// 提供给外部使用type指定类型的异步加载AB包资源的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="type">资源类型</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        public void LoadResAsync(string abName, string resName, System.Type type, System.Action<Object> callBack, bool isABAsync = true)
        {
            MonoManager.Instance.StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack, isABAsync));
        }

        /// <summary>
        /// 真正使用type指定类型的异步加载AB包资源的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="type">资源类型</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        /// <returns></returns>
        private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, System.Action<Object> callBack, bool isABAsync)
        {
            if (isABAsync)
            {
                yield return MonoManager.Instance.StartCoroutine(ReallyLoadABAsync(abName, null));
            }
            else
            {
                LoadAB(abName);
            }
            while (!abDic.ContainsKey(abName))
            {
                yield return null;
            }
            AssetBundleRequest abRequest = abDic[abName].LoadAssetAsync(resName, type);
            yield return abRequest;
            callBack(abRequest.asset);
        }

        /// <summary>
        /// 提供给外部使用泛型指定类型的异步加载AB包资源的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void LoadResAsync<T>(string abName, string resName, System.Action<T> callBack, bool isABAsync = true) where T : Object
        {
            MonoManager.Instance.StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack, isABAsync));
        }

        /// <summary>
        /// 真正使用泛型指定类型的异步加载AB包资源的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, System.Action<T> callBack, bool isABAsync) where T : Object
        {
            if (isABAsync)
            {
                yield return MonoManager.Instance.StartCoroutine(ReallyLoadABAsync(abName, null));
            }
            else
            {
                LoadAB(abName);
            }
            while (!abDic.ContainsKey(abName))
            {
                yield return null;
            }
            AssetBundleRequest abRequest = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abRequest;
            callBack(abRequest.asset as T);
        }

        /// <summary>
        /// 卸载AB包
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="unloadRes">是否卸载资源</param>
        public void unloadAB(string abName, bool unloadRes)
        {
            if (abDic.ContainsKey(abName))
            {
                abDic[abName].Unload(unloadRes);
                abDic.Remove(abName);
            }
        }

        /// <summary>
        /// 异步卸载AB包
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="unloadRes">是否卸载资源</param>
        /// <param name="callBack">回调函数</param>
        public void UnloadABAsync(string abName, bool unloadRes, System.Action callBack = null)
        {
            MonoManager.Instance.StartCoroutine(ReallyUnloadABAsync(abName, unloadRes, callBack));
        }

        /// <summary>
        /// 真正异步卸载AB包的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="unloadRes">是否卸载资源</param>
        /// <param name="callBack">回调函数</param>
        /// <returns></returns>
        private IEnumerator ReallyUnloadABAsync(string abName, bool unloadRes, System.Action callBack)
        {
            if (asyncCount == 0 && abDic.ContainsKey(abName))
            {
                AsyncOperation ao = abDic[abName].UnloadAsync(unloadRes);
                yield return ao;
                abDic.Remove(abName);
                if (callBack != null)
                {
                    callBack.Invoke();
                }
            }
        }

        /// <summary>
        /// 卸载所有AB包
        /// </summary>
        /// <param name="unloadRes">是否卸载资源</param>
        public void UnloadAllAB(bool unloadRes)
        {
            if (asyncCount == 0)
            {
                mainAB.Unload(unloadRes);
                mainAB = null;
                abManifest = null;
                foreach (AssetBundle ab in abDic.Values)
                {
                    ab.Unload(unloadRes);
                }
                abDic.Clear();
            }
        }

        /// <summary>
        /// 异步卸载所有AB包
        /// </summary>
        /// <param name="unloadRes">是否卸载资源</param>
        /// <param name="callBack">回调函数</param>
        public void UnloadAllABAsync(bool unloadRes, System.Action callBack = null)
        {
            MonoManager.Instance.StartCoroutine(ReallyUnloadAllABAsync(unloadRes, callBack));
        }

        /// <summary>
        /// 真正异步卸载所有AB包的方法
        /// </summary>
        /// <param name="unloadRes">是否卸载资源</param>
        /// <param name="callBack">回调函数</param>
        /// <returns></returns>
        private IEnumerator ReallyUnloadAllABAsync(bool unloadRes, System.Action callBack)
        {
            if (asyncCount == 0)
            {
                mainAB.Unload(unloadRes);
                mainAB = null;
                abManifest = null;
                foreach (AssetBundle ab in abDic.Values)
                {
                    AsyncOperation ao = ab.UnloadAsync(unloadRes);
                    yield return ao;
                }
                abDic.Clear();
                if (callBack != null)
                {
                    callBack.Invoke();
                }
            }
        }
    }
}
