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
        private Dictionary<string,Action<UnityEngine.Object>> asyncList=new Dictionary<string, Action<UnityEngine.Object>>();

        /// <summary>
        /// 同步加载资源的普通方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        public UnityEngine.Object Load(string path,bool instantiate=true){
            UnityEngine.Object obj = Resources.Load(path);
            if(instantiate&&obj is GameObject){
                return GameObject.Instantiate(obj);
            }else{
                return obj;
            }
        }

        /// <summary>
        /// 同步加载资源的泛型方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public T Load<T>(string path,bool instantiate=true)where T:UnityEngine.Object
        {
            T obj = Resources.Load<T>(path);
                if(instantiate&&obj is GameObject){
                    return GameObject.Instantiate(obj) ;
                }else{
                    return obj;
                }
        }


        /// <summary>
        /// 异步加载资源的普通方法
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="callBack">回调函数</param>
        public void LoadAsync(string path,Action<UnityEngine.Object> callBack,bool instantiate=true){
            if(asyncList.ContainsKey(path)){
                asyncList[path]+=callBack;
            }else{
                MonoManager.Instance.StartCoroutine(ReallyLoadAsync(path,callBack,instantiate));
            }
        }

        private IEnumerator ReallyLoadAsync(string path,Action<UnityEngine.Object> callBack,bool instantiate){
            asyncList.Add(path,callBack);
            ResourceRequest rr = Resources.LoadAsync(path);
            yield return rr;
            if(callBack!=null){
                if(instantiate&&rr.asset is GameObject){
                    callBack.Invoke(GameObject.Instantiate(rr.asset));
                }else{
                    callBack.Invoke(rr.asset);
                }
            }
            asyncList.Remove(path);
        }

        public void LoadAsync<T>(string path,Action<UnityEngine.Object> callBack,bool instantiate=true)where T:UnityEngine.Object
        {
            if(asyncList.ContainsKey(path)){
                asyncList[path]+=callBack;
            }else{
                MonoManager.Instance.StartCoroutine(ReallyLoadAsync<T>(path,callBack,instantiate));
            }
        }

        private IEnumerator ReallyLoadAsync<T>(string path,Action<UnityEngine.Object> callBack,bool instantiate)where T:UnityEngine.Object
        {
            asyncList.Add(path,callBack);
            ResourceRequest rr = Resources.LoadAsync<T>(path);
            yield return rr;
            if(callBack!=null){
                if(instantiate&&rr.asset is GameObject){
                    callBack.Invoke(GameObject.Instantiate(rr.asset));
                }else{
                    callBack.Invoke(rr.asset);
                }
            }
            asyncList.Remove(path);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="obj">需要卸载的资源对象</param>
        public void UnloadAsset(UnityEngine.Object obj){
            Resources.UnloadAsset(obj);
        }

        /// <summary>
        /// 卸载所有不使用的资源
        /// </summary>
        public void UnloadUnusedAssets(){
            Resources.UnloadUnusedAssets();
        }
    }
}