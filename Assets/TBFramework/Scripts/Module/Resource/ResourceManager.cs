using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBFramework.Mono;
using TBFramework.Pool;

namespace TBFramework.Resource
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        //用于解决两个异步加载加载同一时刻加载同一个资源的问题
        private Dictionary<string, ResEventBase> resDic = new Dictionary<string, ResEventBase>();

        /// <summary>
        /// 同步加载资源的普通方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        [Obsolete("不建议使用该方法进行资源加载，可能会存在同资源名不同类型资源随机加载的情况")]
        public UnityEngine.Object Load(string path)
        {
            //资源名
            string name = path + "_unknown";
            //判断资源是否已经加载过或正在异步加载
            return Load(name, () => Resources.Load(path));
        }

        /// <summary>
        /// 同步加载资源的使用具体类型的方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="type">资源类型</param>
        /// <returns></returns>
        [Obsolete("建议使用泛型方法进行加载资源，因为同一资源使用不同类型加载可能会导致资源加载错误")]
        public UnityEngine.Object Load(string path, Type type)
        {
            //资源名
            string name = path + "_" + type.Name;
            //判断资源是否已经加载过或正在异步加载
            return Load(name, () => Resources.Load(path, type));
        }

        /// <summary>
        /// 同步加载资源的泛型方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            string name = path + "_" + typeof(T).Name;
            return Load<T>(name, () => Resources.Load<T>(path));
        }

        /// <summary>
        /// 同步加载方法相同的部分
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T Load<T>(string name, Func<T> func) where T : UnityEngine.Object
        {
            T obj;
            if (resDic.ContainsKey(name))
            {
                if (resDic[name] != null)
                {
                    ResEvent<T> re_ = resDic[name] as ResEvent<T>;
                    re_.AddRef();
                    if (re_.asset != null)
                    {
                        obj = re_.asset;
                    }
                    else
                    {
                        MonoConManager.Instance.StopCoroutine(re_.coroutine);
                        obj = func?.Invoke();
                        re_.Invoke(obj);
                        re_.SetAsset(obj);
                    }
                    return obj;
                }
                else
                {
                    resDic.Remove(name);
                }
            }
            obj = func?.Invoke();
            ResEvent<T> re = CPoolManager.Instance.Pop<ResEvent<T>>();
            re.SetName(name);
            re.SetAsset(obj);
            resDic.Add(name, re);
            re.AddRef();
            return obj;
        }

        /// <summary>
        /// 异步加载资源的普通方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="callBack">回调函数</param>
        [Obsolete("不建议使用该方法进行资源加载，可能会存在同资源名不同类型资源随机加载的情况")]
        public void LoadAsync(string path, Action<UnityEngine.Object> callBack)
        {
            string name = path + "_unknown";
            LoadAsync(name, callBack, () => MonoConManager.Instance.StartCoroutine(ReallyLoadAsync(path)));
        }

        /// <summary>
        /// 真的异步加载资源的普通方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        private IEnumerator ReallyLoadAsync(string path)
        {
            string name = path + "_unknown";
            ResourceRequest rr = Resources.LoadAsync(path);
            yield return rr;
            AfterLoadResAsync(name, rr.asset);
        }

        /// <summary>
        /// 异步加载资源的使用具体类型的方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="type">资源类型</param>
        /// <param name="callBack">回调函数</param>
        [Obsolete("建议使用泛型方法进行加载资源，因为同一资源使用不同类型加载可能会导致资源加载错误")]
        public void LoadAsync(string path, Type type, Action<UnityEngine.Object> callBack)
        {
            string name = path + "_" + type.Name;
            LoadAsync(name, callBack, () => MonoConManager.Instance.StartCoroutine(ReallyLoadAsync(path, type)));
        }

        /// <summary>
        /// 真的异步加载资源的使用具体类型的方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="type">资源类型</param>
        /// <returns></returns>
        private IEnumerator ReallyLoadAsync(string path, Type type)
        {
            string name = path + "_" + type.Name;
            ResourceRequest rr = Resources.LoadAsync(path, type);
            yield return rr;
            AfterLoadResAsync(name, rr.asset);
        }

        /// <summary>
        /// 异步加载资源的泛型方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="callBack">回调函数</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void LoadAsync<T>(string path, Action<T> callBack) where T : UnityEngine.Object
        {
            string name = path + "_" + typeof(T).Name;
            LoadAsync<T>(name, callBack, () => MonoConManager.Instance.StartCoroutine(ReallyLoadAsync<T>(path)));
        }

        /// <summary>
        /// 真的异步加载资源的泛型方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        private IEnumerator ReallyLoadAsync<T>(string path) where T : UnityEngine.Object
        {
            string name = path + "_" + typeof(T).Name;
            ResourceRequest rr = Resources.LoadAsync<T>(path);
            yield return rr;
            AfterLoadResAsync<T>(name, rr.asset as T);
        }

        /// <summary>
        /// 异步加载方法相同的部分
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callBack"></param>
        /// <param name="func"></param>
        /// <typeparam name="T"></typeparam>
        private void LoadAsync<T>(string name, System.Action<T> callBack, System.Func<Coroutine> func) where T : UnityEngine.Object
        {
            if (resDic.ContainsKey(name))
            {
                if (resDic[name] != null)
                {
                    if (resDic[name] is ResEvent<T>)
                    {
                        ResEvent<T> re_ = resDic[name] as ResEvent<T>;
                        re_.AddRef();
                        if (re_.asset != null)
                        {
                            callBack?.Invoke(re_.asset);
                        }
                        else
                        {
                            re_.actions += callBack;
                        }
                    }
                    else
                    {
                        ResEvent<UnityEngine.Object> re_ = resDic[name] as ResEvent<UnityEngine.Object>;
                        re_.AddRef();
                        if (re_.asset != null)
                        {
                            callBack?.Invoke(re_.asset as T);
                        }
                        else
                        {
                            re_.actions += (obj) => callBack?.Invoke(obj as T);
                        }
                    }
                    return;
                }
                else
                {
                    resDic.Remove(name);
                }
            }
            ResEvent<T> re = CPoolManager.Instance.Pop<ResEvent<T>>();
            re.SetName(name);
            re.actions += callBack;
            resDic.Add(name, re);
            re.AddRef();
            Coroutine c = func?.Invoke();
            re.coroutine = c;
        }

        /// <summary>
        /// 异步加载完成后执行的方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="asset"></param>
        /// <typeparam name="T"></typeparam>
        private void AfterLoadResAsync<T>(string name, T asset) where T : UnityEngine.Object
        {
            if (resDic.ContainsKey(name))
            {
                if (resDic[name] != null)
                {
                    ResEvent<T> re_ = resDic[name] as ResEvent<T>;
                    re_.Invoke(asset);
                    re_.SetAsset(asset);
                    if (re_.RefCount <= 0)
                    {
                        UnloadAssetInRes<T>(name, null, re_.isDel, false);
                    }
                    return;
                }
                else
                {
                    resDic.Remove(name);
                }
            }
            ResEvent<T> re = CPoolManager.Instance.Pop<ResEvent<T>>();
            re.SetName(name);
            re.SetAsset(asset);
            resDic.Add(name, re);
        }

        /// <summary>
        /// 卸载资源的普通方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="callBack">卸载资源的回调函数</param>
        /// <param name="isDel">引用计数清零，是否马上卸载</param>
        /// <param name="isSub">执行这个方法，是否引用计数减一</param>
        public void UnloadAsset(string path, Action<UnityEngine.Object> callBack = null, bool isDel = false, bool isSub = true)
        {
            string name = path + "_unknown";
            UnloadAssetInRes(name, callBack, isDel, isSub);
        }

        /// <summary>
        /// 通过type卸载资源的方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="type">资源类型</param>
        /// <param name="callBack">卸载资源的回调函数</param>
        /// <param name="isDel">引用计数清零，是否马上卸载</param>
        /// <param name="isSub">执行这个方法，是否引用计数减一</param>
        public void UnloadAsset(string path, Type type, Action<UnityEngine.Object> callBack = null, bool isDel = false, bool isSub = true)
        {
            string name = path + "_" + type.Name;
            UnloadAssetInRes(name, callBack, isDel, isSub);
        }

        /// <summary>
        /// 通过泛型卸载资源的方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="callBack">卸载资源的回调函数</param>
        /// <param name="isDel">引用计数清零，是否马上卸载</param>
        /// <param name="isSub">执行这个方法，是否引用计数减一</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void UnloadAsset<T>(string path, Action<T> callBack = null, bool isDel = false, bool isSub = true) where T : UnityEngine.Object
        {
            string name = path + "_" + typeof(T).Name;
            UnloadAssetInRes(name, callBack, isDel, isSub);
        }

        /// <summary>
        /// 卸载方法的相同代码
        /// </summary>
        /// <param name="re">资源信息</param>
        /// <param name="name">获取资源信息的索引名</param>
        /// <param name="callBack">卸载资源的回调函数</param>
        /// <param name="isDel">引用计数清零，是否马上卸载</param>
        /// <param name="isSub">执行这个方法，是否引用计数减一</param>
        /// <typeparam name="T">资源类型</typeparam>
        private void UnloadAssetInRes<T>(string name, Action<T> callBack, bool isDel, bool isSub) where T : UnityEngine.Object
        {
            if (resDic.ContainsKey(name))
            {
                if (resDic[name] != null)
                {
                    ResEvent<T> re = resDic[name] as ResEvent<T>;
                    re.isDel = isDel;
                    if (isSub)
                    {
                        re.SubRef();
                    }
                    if (re.asset != null && re.RefCount <= 0 && re.isDel)
                    {
                        resDic.Remove(name);
                        Resources.UnloadAsset(re.asset);
                        re.SetAsset(null);
                        CPoolManager.Instance.Push(re);
                    }
                    else if (re.asset == null && callBack != null)
                    {
                        re.actions -= callBack;
                    }
                }
                else
                {
                    resDic.Remove(name);
                }
            }
        }

        /// <summary>
        /// 卸载所有不使用的资源
        /// </summary>
        /// <param name="action">回调函数</param>
        public void UnloadUnusedAssets(Action action)
        {
            MonoConManager.Instance.StartCoroutine(ReallyUnloadUnusedAssets(action));
        }

        /// <summary>
        /// 真的卸载所有不使用的资源
        /// </summary>
        /// <param name="action">回调函数</param>
        /// <returns></returns>
        private IEnumerator ReallyUnloadUnusedAssets(Action action)
        {
            List<string> removeList = new List<string>();
            foreach (string name in resDic.Keys)
            {
                if (resDic[name] == null || resDic[name].RefCount <= 0)
                {
                    removeList.Add(name);
                }
            }
            foreach (string name in removeList)
            {
                CPoolManager.Instance.Push(resDic[name]);
                resDic.Remove(name);
            }
            AsyncOperation ao = Resources.UnloadUnusedAssets();
            yield return ao;
            action?.Invoke();
        }

        /// <summary>
        /// 获取引用计数的普通方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        public int GetRefCount(string path)
        {
            string name = path + "_unknown";
            if (resDic.ContainsKey(name))
            {
                return resDic[name].RefCount;
            }
            return 0;
        }

        /// <summary>
        /// 通过type获取引用计数的方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="type">资源类型</param>
        /// <returns></returns>
        public int GetRefCount(string path, Type type)
        {
            string name = path + "_" + type.Name;
            if (resDic.ContainsKey(name))
            {
                return resDic[name].RefCount;
            }
            return 0;
        }

        /// <summary>
        /// 通过泛型获取引用计数的方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public int GetRefCount<T>(string path)
        {
            string name = path + "_" + typeof(T).Name;
            if (resDic.ContainsKey(name))
            {
                return resDic[name].RefCount;
            }
            return 0;
        }

        /// <summary>
        /// 清理资源字典
        /// </summary>
        /// <param name="action">回调函数</param>
        public void ClearDic(Action action)
        {
            MonoConManager.Instance.StartCoroutine(ReallyClearDic(action));
        }

        /// <summary>
        /// 真正清理资源字典的方法
        /// </summary>
        /// <param name="action">回调函数</param>
        /// <returns></returns>
        private IEnumerator ReallyClearDic(Action action)
        {
            foreach (string name in resDic.Keys)
            {
                CPoolManager.Instance.Push(resDic[name]);
            }
            resDic.Clear();
            AsyncOperation ao = Resources.UnloadUnusedAssets();
            yield return ao;
            action?.Invoke();
        }
    }
}