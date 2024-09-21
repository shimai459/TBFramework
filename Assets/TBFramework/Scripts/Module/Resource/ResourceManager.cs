using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBFramework.Mono;

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
            UnityEngine.Object obj;
            //资源名
            string name = path + "_unknown";
            //判断资源是否已经加载过或正在异步加载
            if (resDic.ContainsKey(name))
            {
                ResEvent<UnityEngine.Object> re = resDic[name] as ResEvent<UnityEngine.Object>;
                //资源计数+1
                re.AddRef();
                if (re.asset != null)//如果资源已经加载过，直接获取
                {
                    obj = re.asset;
                }
                else
                {
                    //正在异步加载中，直接同步加载，并且将异步的协程停止
                    obj = Resources.Load(path);
                    re.Invoke(obj);
                    MonoManager.Instance.StopCoroutine(re.coroutine);
                    re.SetAsset(obj);
                }

            }
            else
            {
                //资源没有加载过，直接同步加载
                obj = Resources.Load(path);
                ResEvent<UnityEngine.Object> re = new ResEvent<UnityEngine.Object>(obj, name);
                resDic.Add(name, re);
                re.AddRef();
            }
            return obj;
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
            UnityEngine.Object obj;
            //资源名
            string name = path + "_" + type.Name;
            //判断资源是否已经加载过或正在异步加载
            if (resDic.ContainsKey(name))
            {
                ResEvent<UnityEngine.Object> re = resDic[name] as ResEvent<UnityEngine.Object>;
                re.AddRef();
                if (re.asset != null)
                {
                    obj = re.asset;
                }
                else
                {
                    obj = Resources.Load(path, type);
                    re.Invoke(obj);
                    MonoManager.Instance.StopCoroutine(re.coroutine);
                    re.SetAsset(obj);
                }
            }
            else
            {
                obj = Resources.Load(path, type);
                ResEvent<UnityEngine.Object> re = new ResEvent<UnityEngine.Object>(obj, name);
                resDic.Add(name, re);
                re.AddRef();
            }
            return obj;
        }

        /// <summary>
        /// 同步加载资源的泛型方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="instantiate">是否实例化</param>
        /// <returns></returns>
        public T Load<T>(string path, bool instantiate = true) where T : UnityEngine.Object
        {
            T obj;
            string name = path + "_" + typeof(T).Name;
            if (resDic.ContainsKey(name))
            {
                ResEvent<T> re = resDic[name] as ResEvent<T>;
                re.AddRef();
                if (re.asset != null)
                {
                    obj = re.asset;
                }
                else
                {
                    MonoManager.Instance.StopCoroutine(re.coroutine);
                    obj = Resources.Load<T>(path);
                    re.Invoke(obj);
                    re.SetAsset(obj);
                }
            }
            else
            {
                obj = Resources.Load<T>(path);
                ResEvent<T> re = new ResEvent<T>(obj, name);
                resDic.Add(name, re);
                re.AddRef();
            }
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
            if (resDic.ContainsKey(name))
            {
                ResEvent<UnityEngine.Object> re = resDic[name] as ResEvent<UnityEngine.Object>;
                re.AddRef();
                if (re.asset != null)
                {
                    callBack?.Invoke(re.asset);
                }
                else
                {
                    re.actions += callBack;
                }
            }
            else
            {
                ResEvent<UnityEngine.Object> re = new ResEvent<UnityEngine.Object>(callBack, name);
                resDic.Add(name, re);
                re.AddRef();
                Coroutine c = MonoManager.Instance.StartCoroutine(ReallyLoadAsync(path));
                re.coroutine = c;
            }
        }

        /// <summary>
        /// 真的异步加载资源的普通方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="instantiate">是否实例化</param>
        /// <returns></returns>
        private IEnumerator ReallyLoadAsync(string path)
        {
            string name = path + "_unknown";
            ResourceRequest rr = Resources.LoadAsync(path);
            yield return rr;
            if (resDic.ContainsKey(name))
            {
                ResEvent<UnityEngine.Object> re = resDic[name] as ResEvent<UnityEngine.Object>;
                re.Invoke(rr.asset);
                re.SetAsset(rr.asset);
                if (re.RefCount <= 0)
                {
                    UnloadAsset(path, null, re.isDel, false);
                }
            }
            else
            {
                resDic.Add(name, new ResEvent<UnityEngine.Object>(rr.asset, name));
            }
        }

        /// <summary>
        /// 异步加载资源的使用具体类型的方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="type">资源类型</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="instantiate">是否实例化</param>
        [Obsolete("建议使用泛型方法进行加载资源，因为同一资源使用不同类型加载可能会导致资源加载错误")]
        public void LoadAsync(string path, Type type, Action<UnityEngine.Object> callBack)
        {
            string name = path + "_" + type.Name;
            if (resDic.ContainsKey(name))
            {
                ResEvent<UnityEngine.Object> re = resDic[name] as ResEvent<UnityEngine.Object>;
                re.AddRef();
                if (re.asset != null)
                {
                    callBack?.Invoke(re.asset);
                }
                else
                {
                    re.actions += callBack;
                }
            }
            else
            {
                ResEvent<UnityEngine.Object> re = new ResEvent<UnityEngine.Object>(callBack, name);
                resDic.Add(name, re);
                re.AddRef();
                Coroutine c = MonoManager.Instance.StartCoroutine(ReallyLoadAsync(path, type));
                re.coroutine = c;
            }
        }

        /// <summary>
        /// 真的异步加载资源的使用具体类型的方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="type">资源类型</param>
        /// <param name="instantiate">是否实例化</param>
        /// <returns></returns>
        private IEnumerator ReallyLoadAsync(string path, Type type)
        {
            string name = path + "_" + type.Name;
            ResourceRequest rr = Resources.LoadAsync(path, type);
            yield return rr;
            if (resDic.ContainsKey(name))
            {
                ResEvent<UnityEngine.Object> re = resDic[name] as ResEvent<UnityEngine.Object>;
                re.Invoke(rr.asset);
                re.SetAsset(rr.asset);
                if (re.RefCount <= 0)
                {
                    UnloadAsset(path, null, re.isDel, false);
                }
            }
            else
            {
                resDic.Add(name, new ResEvent<UnityEngine.Object>(rr.asset, name));
            }
        }

        /// <summary>
        /// 异步加载资源的泛型方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="instantiate">是否实例化</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void LoadAsync<T>(string path, Action<T> callBack) where T : UnityEngine.Object
        {
            string name = path + "_" + typeof(T).Name;
            if (resDic.ContainsKey(name))
            {
                ResEvent<T> re = resDic[name] as ResEvent<T>;
                re.AddRef();
                if (re.asset != null)
                {
                    callBack?.Invoke(re.asset);
                }
                else
                {
                    re.actions += callBack;
                }
            }
            else
            {
                ResEvent<T> re = new ResEvent<T>(callBack, name);
                resDic.Add(name, re);
                re.AddRef();
                Coroutine c = MonoManager.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
                re.coroutine = c;
            }
        }

        /// <summary>
        /// 真的异步加载资源的泛型方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="instantiate">是否实例化</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        private IEnumerator ReallyLoadAsync<T>(string path) where T : UnityEngine.Object
        {
            string name = path + "_" + typeof(T).Name;
            ResourceRequest rr = Resources.LoadAsync<T>(path);
            yield return rr;
            if (resDic.ContainsKey(name))
            {
                ResEvent<T> re = resDic[name] as ResEvent<T>;
                re.Invoke(rr.asset as T);
                re.SetAsset(rr.asset as T);
                if (re.RefCount <= 0)
                {
                    UnloadAsset(path, null, re.isDel, false);
                }
            }
            else
            {
                resDic.Add(name, new ResEvent<T>(rr.asset as T, name));
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="obj">需要卸载的资源对象</param>
        public void UnloadAsset(string path, Action<UnityEngine.Object> callBack = null, bool isDel = false, bool isSub = true)
        {
            string name = path + "_unknown";
            if (resDic.ContainsKey(name))
            {
                ResEvent<UnityEngine.Object> re = resDic[name] as ResEvent<UnityEngine.Object>;
                UnloadAssetInRes(re, name, callBack, isDel, isSub);
            }
        }

        public void UnloadAsset(string path, Type type, Action<UnityEngine.Object> callBack = null, bool isDel = false, bool isSub = true)
        {
            string name = path + "_" + type.Name;
            if (resDic.ContainsKey(name))
            {
                ResEvent<UnityEngine.Object> re = resDic[name] as ResEvent<UnityEngine.Object>;
                UnloadAssetInRes(re, name, callBack, isDel, isSub);
            }
        }

        public void UnloadAsset<T>(string path, Action<T> callBack = null, bool isDel = false, bool isSub = true) where T : UnityEngine.Object
        {
            string name = path + "_" + typeof(T).Name;
            if (resDic.ContainsKey(name))
            {
                ResEvent<T> re = resDic[name] as ResEvent<T>;
                UnloadAssetInRes(re, name, callBack, isDel, isSub);
            }
        }

        private void UnloadAssetInRes<T>(ResEvent<T> re, string name, Action<T> callBack, bool isDel, bool isSub) where T : UnityEngine.Object
        {
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
            }
            else if (re.asset == null && callBack != null)
            {
                re.actions -= callBack;
            }

        }

        /// <summary>
        /// 卸载所有不使用的资源
        /// </summary>
        /// <param name="action">回调函数</param>
        public void UnloadUnusedAssets(Action action)
        {
            MonoManager.Instance.StartCoroutine(ReallyUnloadUnusedAssets(action));
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
                if (resDic[name].RefCount <= 0)
                {
                    removeList.Add(name);
                }
            }
            foreach (string name in removeList)
            {
                resDic.Remove(name);
            }
            AsyncOperation ao = Resources.UnloadUnusedAssets();
            yield return ao;
            action?.Invoke();
        }


        public int GetRefCount(string path)
        {
            string name = path + "_unknown";
            if (resDic.ContainsKey(name))
            {
                return resDic[name].RefCount;
            }
            return 0;
        }

        public int GetRefCount(string path, Type type)
        {
            string name = path + "_" + type.Name;
            if (resDic.ContainsKey(name))
            {
                return resDic[name].RefCount;
            }
            return 0;
        }

        public int GetRefCount<T>(string path)
        {
            string name = path + "_" + typeof(T).Name;
            if (resDic.ContainsKey(name))
            {
                return resDic[name].RefCount;
            }
            return 0;
        }
    
        public void ClearDic(Action action){
            MonoManager.Instance.StartCoroutine(ReallyClearDic(action));
        }

        private IEnumerator ReallyClearDic(Action action)
        {
            resDic.Clear();
            AsyncOperation ao = Resources.UnloadUnusedAssets();
            yield return ao;
            action?.Invoke();
        }
    }
}