using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBFramework.Mono;
using TBFramework.Pool;
using UnityEngine.SceneManagement;

namespace TBFramework.Load.AssetBundles.Load
{
    /// <summary>
    /// AB包的一个套系
    /// </summary>
    public class ABSingle : CBase
    {
        private AssetBundle mainAB = null;//该套系的主包
        private AssetBundleManifest abManifest = null;//该套系的依赖
        private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();//该套系已加载的AB包
        private List<string> abAsyncList = new List<string>();//该套系正在异步加载的AB包
        private List<string> asyncLoadList = new List<string>();//该套系中需要异步加载的AB包
        public int AsyncCount { get => asyncLoadList.Count; }

        private string mainName;//该套系的主包名

        private string pathURL;//该套系存放的路径

        private Dictionary<string, Dictionary<string, ABResBase>> resDic = new Dictionary<string, Dictionary<string, ABResBase>>();//资源

        public ABSingle() { }

        public ABSingle(string pathURL, string mainName)
        {
            SetPathAndMainName(pathURL, mainName);
        }

        /// <summary>
        /// 设置套系信息
        /// </summary>
        /// <param name="pathURL"></param>
        /// <param name="mainName"></param>
        public void SetPathAndMainName(string pathURL, string mainName)
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
            //资源名
            string name = GetName(resName);
            return LoadRes(abName, name, () =>
            {
                if (abDic.ContainsKey(abName))
                {
                    return abDic[abName].LoadAsset(resName);
                }
                else
                {
                    return null;
                }
            });
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
            //资源名
            string name = GetName(resName, type);
            return LoadRes(abName, name, () =>
            {
                if (abDic.ContainsKey(abName))
                {
                    return abDic[abName].LoadAsset(resName, type);
                }
                else
                {
                    return null;
                }
            }); ;
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
            //资源名
            string name = GetName<T>(resName);
            return LoadRes(abName, name, () =>
            {
                if (abDic.ContainsKey(abName))
                {
                    return abDic[abName].LoadAsset<T>(resName);
                }
                else
                {
                    return null;
                }
            }); ;
        }

        /// <summary>
        /// 同步加载方法中相同的部分
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T LoadRes<T>(string abName, string name, System.Func<T> func) where T : Object
        {
            LoadAB(abName);
            T obj = null;
            //判断资源是否已经加载过或正在异步加载
            if (resDic.ContainsKey(abName))
            {
                if (resDic[abName] != null)
                {
                    if (resDic[abName].ContainsKey(name))
                    {
                        if (resDic[abName][name] != null)
                        {
                            ABResBase re = resDic[abName][name];
                            //资源计数+1
                            re.AddRef();
                            Object o = re.GetAsset();
                            if (o != null)//如果资源已经加载过，直接获取
                            {
                                obj = o as T;
                            }
                            else
                            {
                                if (abDic.ContainsKey(abName))
                                {
                                    //正在异步加载中，直接同步加载，并且将异步的协程停止
                                    obj = func?.Invoke();
                                    re.Invoke(obj);
                                    re.StopCoroutine();
                                    re.SetAsset(obj);
                                }
                            }
                            return obj;
                        }
                        else
                        {
                            resDic[abName].Remove(name);
                        }
                    }
                }
                else
                {
                    resDic[abName] = new Dictionary<string, ABResBase>();
                }
            }
            else
            {
                resDic.Add(abName, new Dictionary<string, ABResBase>());
            }
            if (abDic.ContainsKey(abName))
            {
                //资源没有加载过，直接同步加载
                obj = func?.Invoke();
                ABRes<T> re = CPoolManager.Instance.Pop<ABRes<T>>();
                re.SetName(name);
                re.SetAsset(obj);
                resDic[abName].Add(name, re);
                re.AddRef();
            }
            return obj;
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
            MonoConManager.Instance.StartCoroutine(ReallyLoadABAsync(abName, callBack));
        }

        /// <summary>
        /// 真正异步加载AB包的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <returns></returns>
        private IEnumerator ReallyLoadABAsync(string abName, System.Action<AssetBundle> callBack)
        {
            if (!asyncLoadList.Contains(abName))
            {
                asyncLoadList.Add(abName);
                yield return new WaitForSeconds(5f);
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
                asyncLoadList.Remove(abName);
            }
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
            string name = GetName(resName);
            LoadResAsync(abName, name, callBack, isABAsync, () => MonoConManager.Instance.StartCoroutine(ReallyLoadResAsync(abName, resName, isABAsync)));
        }

        /// <summary>
        /// 真正不指定类型的异步加载AB包资源的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        /// <returns></returns>
        private IEnumerator ReallyLoadResAsync(string abName, string resName, bool isABAsync)
        {
            while (!abDic.ContainsKey(abName))
            {
                if (!asyncLoadList.Contains(abName))
                {
                    if (isABAsync)
                    {
                        yield return MonoConManager.Instance.StartCoroutine(ReallyLoadABAsync(abName, null));
                    }
                    else
                    {
                        LoadAB(abName);
                    }
                }
                yield return null;
            }
            AssetBundleRequest abRequest = abDic[abName].LoadAssetAsync(resName);
            yield return abRequest;
            string name = GetName(resName);
            AfterLoadResAsync(abName, name, abRequest.asset);
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
            string name = GetName(resName, type);
            LoadResAsync(abName, name, callBack, isABAsync, () => MonoConManager.Instance.StartCoroutine(ReallyLoadResAsync(abName, resName, type, isABAsync)));
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
        private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, bool isABAsync)
        {
            while (!abDic.ContainsKey(abName))
            {
                if (!asyncLoadList.Contains(abName))
                {
                    if (isABAsync)
                    {
                        yield return MonoConManager.Instance.StartCoroutine(ReallyLoadABAsync(abName, null));
                    }
                    else
                    {
                        LoadAB(abName);
                    }
                }
                yield return null;
            }
            AssetBundleRequest abRequest = abDic[abName].LoadAssetAsync(resName, type);
            yield return abRequest;
            string name = GetName(resName, type);
            AfterLoadResAsync(abName, name, abRequest.asset);
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
            string name = GetName<T>(resName);
            LoadResAsync(abName, name, callBack, isABAsync, () => MonoConManager.Instance.StartCoroutine(ReallyLoadResAsync<T>(abName, resName, isABAsync)));
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
        private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, bool isABAsync) where T : Object
        {
            while (!abDic.ContainsKey(abName))
            {
                if (!asyncLoadList.Contains(abName))
                {
                    if (isABAsync)
                    {
                        yield return MonoConManager.Instance.StartCoroutine(ReallyLoadABAsync(abName, null));
                    }
                    else
                    {
                        LoadAB(abName);
                    }
                }
                yield return null;
            }
            AssetBundleRequest abRequest = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abRequest;
            string name = GetName<T>(resName);
            AfterLoadResAsync<T>(abName, name, abRequest.asset as T);
        }

        /// <summary>
        /// 异步加载方法相同的部分
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="name"></param>
        /// <param name="callBack"></param>
        /// <param name="isABAsync"></param>
        /// <param name="func"></param>
        /// <typeparam name="T"></typeparam>
        private void LoadResAsync<T>(string abName, string name, System.Action<T> callBack, bool isABAsync, System.Func<Coroutine> func) where T : Object
        {
            if (isABAsync)
            {
                LoadABAsync(abName, null);
            }
            else
            {
                LoadAB(abName);
            }
            if (resDic.ContainsKey(abName))
            {
                if (resDic[abName] != null)
                {
                    if (resDic[abName].ContainsKey(name))
                    {
                        if (resDic[abName][name] != null)
                        {
                            ABResBase res = resDic[abName][name];
                            res.AddRef();
                            Object o = res.GetAsset();
                            if (o != null)
                            {

                                callBack?.Invoke(o as T);
                            }
                            else
                            {
                                res.AddAction((obj) => callBack?.Invoke(obj as T));
                            }
                            return;
                        }
                        else
                        {
                            resDic[abName].Remove(name);
                        }
                    }
                }
                else
                {
                    resDic[abName] = new Dictionary<string, ABResBase>();
                }
            }
            else
            {
                resDic.Add(abName, new Dictionary<string, ABResBase>());
            }
            ABRes<T> re = CPoolManager.Instance.Pop<ABRes<T>>();
            re.SetName(name);
            re.actions += callBack;
            resDic[abName].Add(name, re);
            re.AddRef();
            Coroutine c = func?.Invoke();
            re.coroutine = c;
        }

        /// <summary>
        /// 异步加载资源后执行的方法
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="name"></param>
        /// <param name="asset"></param>
        /// <typeparam name="T"></typeparam>
        private void AfterLoadResAsync<T>(string abName, string name, T asset) where T : Object
        {
            if (resDic.ContainsKey(abName))
            {
                if (resDic[abName] != null)
                {
                    if (resDic[abName].ContainsKey(name))
                    {
                        if (resDic[abName][name] != null)
                        {
                            ABRes<T> re_ = resDic[abName][name] as ABRes<T>;
                            re_.Invoke(asset);
                            re_.SetAsset(asset);
                            if (re_.RefCount <= 0)
                            {
                                UnloadAssetInRes<T>(abName, name, null, null, re_.isDel, false);
                            }
                            return;
                        }
                        else
                        {
                            resDic[abName].Remove(name);
                        }
                    }
                }
                else
                {
                    resDic[abName] = new Dictionary<string, ABResBase>();
                }
            }
            else
            {
                resDic.Add(abName, new Dictionary<string, ABResBase>());
            }
            ABRes<T> re = CPoolManager.Instance.Pop<ABRes<T>>();
            re.SetName(name);
            re.SetAsset(asset);
            resDic[abName].Add(name, re);
        }

        public void UnloadMain(bool unloadRes)
        {
            if (mainAB != null)
            {
                mainAB.Unload(unloadRes);
                mainAB = null;
            }
        }

        public void UnloadMainAsync(bool unloadRes, System.Action callBack = null)
        {
            MonoConManager.Instance.StartCoroutine(ReallyUnloadMainAsync(unloadRes, callBack));
        }

        private IEnumerator ReallyUnloadMainAsync(bool unloadRes, System.Action callBack = null)
        {
            if (AsyncCount == 0 && mainAB != null)
            {
                yield return mainAB.UnloadAsync(unloadRes);
                mainAB = null;
                callBack?.Invoke();
            }
        }


        /// <summary>
        /// 卸载AB包
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="unloadRes">是否卸载资源</param>
        public void UnloadAB(string abName, bool unloadRes)
        {
            if (AsyncCount <= 0)
            {
                if (unloadRes)
                {
                    UnloadReses(abName);
                }
                if (abDic.ContainsKey(abName))
                {
                    if (abDic[abName] != null)
                    {
                        abDic[abName].Unload(unloadRes);
                    }
                    abDic.Remove(abName);
                }
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
            MonoConManager.Instance.StartCoroutine(ReallyUnloadABAsync(abName, unloadRes, callBack));
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
            if (AsyncCount == 0 && abDic.ContainsKey(abName))
            {
                if (abDic[abName] != null)
                {
                    AsyncOperation ao = abDic[abName].UnloadAsync(unloadRes);
                    yield return ao;
                }
                if (unloadRes)
                {
                    UnloadReses(abName);
                }
                abDic.Remove(abName);
                callBack?.Invoke();
            }
        }

        /// <summary>
        /// 卸载所有AB包
        /// </summary>
        /// <param name="unloadRes">是否卸载资源</param>
        public void UnloadAllAB(bool unloadRes)
        {
            if (AsyncCount == 0)
            {
                mainAB.Unload(unloadRes);
                mainAB = null;
                abManifest = null;
                foreach (string name in abDic.Keys)
                {
                    if (abDic[name] != null)
                    {
                        abDic[name].Unload(unloadRes);
                    }
                    if (unloadRes)
                    {
                        UnloadReses(name);
                    }
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
            MonoConManager.Instance.StartCoroutine(ReallyUnloadAllABAsync(unloadRes, callBack));
        }

        /// <summary>
        /// 真正异步卸载所有AB包的方法
        /// </summary>
        /// <param name="unloadRes">是否卸载资源</param>
        /// <param name="callBack">回调函数</param>
        /// <returns></returns>
        private IEnumerator ReallyUnloadAllABAsync(bool unloadRes, System.Action callBack)
        {
            if (AsyncCount == 0)
            {
                mainAB.Unload(unloadRes);
                mainAB = null;
                abManifest = null;
                foreach (string name in abDic.Keys)
                {
                    if (unloadRes)
                    {
                        UnloadReses(name);
                    }
                    if (abDic[name] != null)
                    {
                        AsyncOperation ao = abDic[name].UnloadAsync(unloadRes);
                        yield return ao;
                    }
                }
                abDic.Clear();
                callBack?.Invoke();
            }
        }

        /// <summary>
        /// 卸载单个AB包中的所有资源
        /// </summary>
        /// <param name="abName"></param>
        public void UnloadReses(string abName)
        {
            if (abDic.ContainsKey(abName) && abDic[abName] != null && resDic.ContainsKey(abName) && resDic[abName] != null)
            {
                foreach (ABResBase item in resDic[abName].Values)
                {
                    Object obj = item.GetAsset();
                    if (obj != null)
                    {
                        abDic[abName].Unload(item.GetAsset());
                    }
                    CPoolManager.Instance.Push(item);
                }
                resDic.Remove(abName);
            }
            else if (resDic.ContainsKey(abName) && resDic[abName] != null)
            {
                foreach (ABResBase item in resDic[abName].Values)
                {
                    Object obj = item.GetAsset();
                    if (obj != null)
                    {
                        Resources.UnloadAsset(item.GetAsset());
                    }
                    CPoolManager.Instance.Push(item);
                }
                resDic.Remove(abName);
            }
        }

        public void UnloadResesAsync(string abName, System.Action callBack = null)
        {
            MonoConManager.Instance.StartCoroutine(ReallyUnloadResesAsync(abName, callBack));
        }

        private IEnumerator ReallyUnloadResesAsync(string abName, System.Action callBack)
        {
            if (abDic.ContainsKey(abName) && abDic[abName] != null && resDic.ContainsKey(abName))
            {
                if (abDic[abName] != null)
                {
                    foreach (ABResBase item in resDic[abName].Values)
                    {
                        Object obj = item.GetAsset();
                        if (obj != null)
                        {
                            yield return abDic[abName].UnloadAsync(item.GetAsset());
                        }
                        CPoolManager.Instance.Push(item);
                    }
                }
                resDic.Remove(abName);
            }
            else if (resDic.ContainsKey(abName))
            {
                if (resDic[abName] != null)
                {
                    foreach (ABResBase item in resDic[abName].Values)
                    {
                        Object obj = item.GetAsset();
                        if (obj != null)
                        {
                            Resources.UnloadAsset(item.GetAsset());
                        }
                        CPoolManager.Instance.Push(item);
                    }
                }
                resDic.Remove(abName);
            }
        }

        /// <summary>
        /// 卸载资源的普通方法
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="subAction"></param>
        /// <param name="isDel"></param>
        /// <param name="isSub"></param>
        public void UnloadRes(string abName, string resName, System.Action callBack = null, System.Action<UnityEngine.Object> subAction = null, bool isDel = false, bool isSub = true)
        {
            string name = resName + "_unknown";
            UnloadAssetInRes(abName, name, callBack, subAction, isDel, isSub);
        }

        /// <summary>
        /// 卸载资源的类型方法
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <param name="subAction"></param>
        /// <param name="isDel"></param>
        /// <param name="isSub"></param>
        public void UnloadRes(string abName, string resName, System.Type type, System.Action callBack = null, System.Action<UnityEngine.Object> subAction = null, bool isDel = false, bool isSub = true)
        {
            string name = resName + "_" + type.Name;
            UnloadAssetInRes(abName, name, callBack, subAction, isDel, isSub);
        }

        /// <summary>
        /// 卸载资源的泛型方法
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="subAction"></param>
        /// <param name="isDel"></param>
        /// <param name="isSub"></param>
        /// <typeparam name="T"></typeparam>
        public void UnloadRes<T>(string abName, string resName, System.Action callBack = null, System.Action<T> subAction = null, bool isDel = false, bool isSub = true) where T : Object
        {
            string name = resName + "_" + typeof(T).Name;
            UnloadAssetInRes<T>(abName, name, callBack, subAction, isDel, isSub);
        }

        /// <summary>
        /// 卸载资源的相同步骤
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="name">资源字典获取名</param>
        /// <param name="subAction">资源加载回调函数</param>
        /// <param name="isDel">引用计数清零，是否马上卸载</param>
        /// <param name="isSub">执行这个方法，是否引用计数减一</param>
        /// <typeparam name="T">资源类型</typeparam>
        private void UnloadAssetInRes<T>(string abName, string name, System.Action callBack, System.Action<T> subAction, bool isDel, bool isSub) where T : UnityEngine.Object
        {
            if (resDic.ContainsKey(abName))
            {
                if (resDic[abName] != null)
                {
                    if (resDic[abName].ContainsKey(name))
                    {
                        ABRes<T> re = resDic[abName][name] as ABRes<T>;
                        if (re != null)
                        {
                            re.isDel = isDel;
                            if (isSub)
                            {
                                re.SubRef();
                            }
                            if (re.asset != null && re.RefCount <= 0 && re.isDel)
                            {
                                resDic.Remove(name);
                                if (abDic.ContainsKey(abName))
                                {
                                    if (abDic[abName] != null)
                                    {
                                        abDic[abName].Unload(re.asset);
                                    }
                                    else
                                    {
                                        abDic.Remove(abName);
                                    }
                                }
                                else
                                {
                                    Resources.UnloadAsset(re.asset);
                                }
                                re.SetAsset(null);
                                CPoolManager.Instance.Push(re);
                            }
                            else if (re.asset == null && subAction != null)
                            {
                                re.actions -= subAction;
                            }
                        }
                        else
                        {
                            resDic[abName].Remove(name);
                        }
                    }
                }
                else
                {
                    resDic.Remove(abName);
                }
                callBack?.Invoke();
            }

        }


        public void UnloadResAsync(string abName, string resName, System.Action callBack = null, System.Action<UnityEngine.Object> subAction = null, bool isDel = false, bool isSub = true)
        {
            string name = GetName(resName);
            MonoConManager.Instance.StartCoroutine(UnloadAssetInResAsync(abName, resName, callBack, subAction, isDel, isSub));
        }

        public void UnloadResAsync<T>(string abName, string resName, System.Action callBack = null, System.Action<T> subAction = null, bool isDel = false, bool isSub = true) where T : UnityEngine.Object
        {
            string name = GetName<T>(resName);
            MonoConManager.Instance.StartCoroutine(UnloadAssetInResAsync<T>(abName, resName, callBack, subAction, isDel, isSub));
        }

        public void UnloadResAsync(string abName, string resName, System.Type type, System.Action callBack = null, System.Action<UnityEngine.Object> subAction = null, bool isDel = false, bool isSub = true)
        {
            string name = GetName(resName, type);
            MonoConManager.Instance.StartCoroutine(UnloadAssetInResAsync(abName, resName, callBack, subAction, isDel, isSub));
        }

        private IEnumerator UnloadAssetInResAsync<T>(string abName, string name, System.Action callBack, System.Action<T> subAction, bool isDel, bool isSub) where T : UnityEngine.Object
        {
            if (resDic.ContainsKey(abName))
            {
                if (resDic[abName] != null)
                {
                    if (resDic[abName].ContainsKey(name))
                    {
                        ABRes<T> re = resDic[abName][name] as ABRes<T>;
                        if (re != null)
                        {
                            re.isDel = isDel;
                            if (isSub)
                            {
                                re.SubRef();
                            }
                            if (re.asset != null && re.RefCount <= 0 && re.isDel)
                            {
                                resDic.Remove(name);
                                if (abDic.ContainsKey(abName))
                                {
                                    if (abDic[abName] != null)
                                    {
                                        yield return abDic[abName].UnloadAsync(re.asset);
                                    }
                                    else
                                    {
                                        abDic.Remove(abName);
                                    }
                                }
                                else
                                {
                                    Resources.UnloadAsset(re.asset);
                                }
                                re.SetAsset(null);
                                CPoolManager.Instance.Push(re);
                            }
                            else if (re.asset == null && subAction != null)
                            {
                                re.actions -= subAction;
                            }
                        }
                        else
                        {
                            resDic[abName].Remove(name);
                        }
                    }
                }
                else
                {
                    resDic.Remove(abName);
                }
                callBack?.Invoke();
            }
        }

        public void LoadScene(string abName, string sceneName, System.Action action = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            LoadAB(abName);
            TBFramework.Load.Scene.SceneManager.Instance.LoadScene(sceneName, action, loadSceneMode);
        }


        public void LoadScene(string abName, string sceneName, LoadSceneParameters param, System.Action action = null)
        {
            LoadAB(abName);
            TBFramework.Load.Scene.SceneManager.Instance.LoadScene(sceneName, param, action);
        }


        public void LoadSceneAsync(string abName, string sceneName, bool isAsync = false, System.Action<AsyncOperation> loading = null, System.Action<AsyncOperation> callBack = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (isAsync)
            {
                LoadABAsync(abName, (ab) =>
                {
                    TBFramework.Load.Scene.SceneManager.Instance.LoadSceneAsync(sceneName, callBack, loading, loadSceneMode);
                });
            }
            else
            {
                LoadAB(abName);
                TBFramework.Load.Scene.SceneManager.Instance.LoadSceneAsync(sceneName, callBack, loading, loadSceneMode);
            }

        }

        public void LoadSceneAsync(string abName, string sceneName, LoadSceneParameters param, bool isAsync = false, System.Action<AsyncOperation> loading = null, System.Action<AsyncOperation> callBack = null)
        {
            if (isAsync)
            {
                LoadABAsync(abName, (ab) =>
                {
                    TBFramework.Load.Scene.SceneManager.Instance.LoadSceneAsync(sceneName, param, callBack, loading);
                });
            }
            else
            {
                LoadAB(abName);
                TBFramework.Load.Scene.SceneManager.Instance.LoadSceneAsync(sceneName, param, callBack, loading);
            }

        }

        public void UnloadScene(string sceneName, System.Action<bool> callBack = null)
        {
            TBFramework.Load.Scene.SceneManager.Instance.UnloadScene(sceneName, callBack);
        }

        public void UnloadSceneAsync(string sceneName, UnloadSceneOptions options = UnloadSceneOptions.None, System.Action<AsyncOperation> unloading = null, System.Action<AsyncOperation> callBack = null)
        {
            TBFramework.Load.Scene.SceneManager.Instance.UnloadSceneAsync(sceneName, options, unloading, callBack);
        }

        private string GetName(string resName)
        {
            return resName + "_unkown";
        }

        private string GetName<T>(string resName)
        {
            return resName + "_" + typeof(T).Name;
        }

        private string GetName(string resName, System.Type type)
        {
            return resName + "_" + type.Name;
        }

        /// <summary>
        /// 重置回归
        /// </summary>
        public override void Reset()
        {
            this.mainAB = null;
            this.abManifest = null;
            abDic.Clear();
            abAsyncList.Clear();
            asyncLoadList.Clear();
            mainName = null;
            pathURL = null;
            foreach (Dictionary<string, ABResBase> ress in resDic.Values)
            {
                foreach (ABResBase res in ress.Values)
                {
                    CPoolManager.Instance.Push(res);
                }
            }
            resDic.Clear();
        }
    }
}
