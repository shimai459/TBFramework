using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TBFramework.Addressables
{
    #region 使用前提
        /*
        1.使用较新的Unity版本导入Addressables包
        2.创建Addressables相关的配置文件
            法一:Windows/Asset Management/Addressables/Groups的窗口中点击Create Addressables Setting按钮创建配置文件
            法二:在Inspector窗口中为资源勾选Addressable选项,此时会自动创建配置文件
        */
    #endregion

    #region 注意
        /*
        1.Resources文件夹下资源如果变为寻址资源,会移入Resources_moved文件夹下,因为Resources下的资源会被打包出去,而可寻址资源使用addressables去管理
        2.C#不可作为可寻址资源
        3.在低版本,如果要使用指定标识类的泛型模式,需要自己申明类去继承泛型标识类才能使用
        */
    #endregion
    /// <summary>
    /// 
    /// </summary>
    public class AddressablesManager:Singleton<AddressablesManager>
    {
        public Dictionary<string,IEnumerator> resDic=new Dictionary<string, IEnumerator>();
        public void LoadAsync<T>(string resName,System.Action<AsyncOperationHandle<T>> completed,System.Action<AsyncOperationHandle> destroyed=null,System.Action<AsyncOperationHandle> completedTypeless=null){
            string keyName=resName+"_"+typeof(T).Name;
            AsyncOperationHandle<T> handle;
            if(resDic.ContainsKey(keyName)){
                handle=(AsyncOperationHandle<T>)resDic[keyName];
                if(handle.IsDone){
                   if(completed!=null){
                        completed(handle);
                    }
                    if(destroyed!=null){
                        handle.Destroyed+=destroyed;
                    }
                    if(completedTypeless!=null){
                        completedTypeless(handle);
                    }
                }else{
                    if(completed!=null){
                        handle.Completed+=CompleteCheckLoadIsSuccess;
                    }
                    if(destroyed!=null){
                        handle.Destroyed+=DestroyCheckLoadIsSuccess;
                    }
                    if(completedTypeless!=null){
                        handle.CompletedTypeless+=CompleteTypelessCheckLoadIsSuccess;
                    }
                    return;
                }
                handle=UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(resName);
                if(completed!=null){
                    handle.Completed+=CompleteCheckLoadIsSuccess;
                }
                if(destroyed!=null){
                    handle.Destroyed+=DestroyCheckLoadIsSuccess;
                }
                if(completedTypeless!=null){
                    handle.CompletedTypeless+=CompleteTypelessCheckLoadIsSuccess;
                }
                resDic.Add(keyName,handle);
            }

            void CompleteCheckLoadIsSuccess(AsyncOperationHandle<T> h){
                if(h.Status==AsyncOperationStatus.Succeeded){
                    if(completed!=null){
                        completed(h);
                    }
                }else{
                    Debug.LogWarning(keyName+"资源加载失败");
                    if(resDic.ContainsKey(keyName)){
                        resDic.Remove(keyName);
                    }
                }
            }
            void DestroyCheckLoadIsSuccess(AsyncOperationHandle h){
                if(h.Status==AsyncOperationStatus.Succeeded){
                    if(destroyed!=null){
                        destroyed(h);
                    }
                }else{
                    Debug.LogWarning(keyName+"资源加载失败");
                    if(resDic.ContainsKey(keyName)){
                        resDic.Remove(keyName);
                    }
                }
            }
            void CompleteTypelessCheckLoadIsSuccess(AsyncOperationHandle h){
                if(h.Status==AsyncOperationStatus.Succeeded){
                    if(completedTypeless!=null){
                        completedTypeless(h);
                    }
                }else{
                    Debug.LogWarning(keyName+"资源加载失败");
                    if(resDic.ContainsKey(keyName)){
                        resDic.Remove(keyName);
                    }
                }
            }
        }
    
        public void LoadAsync<T>(string resName,System.Action<T> callBack){
            string keyName=resName+"_"+typeof(T).Name;
            AsyncOperationHandle<T> handle;
            if(resDic.ContainsKey(keyName)){
                handle=(AsyncOperationHandle<T>)resDic[keyName];
                if(handle.IsDone){
                    callBack(handle.Result);
                }else{
                    
                }
            }else{

            }
        }

        public void LoadSceneAsync(){
            //UnityEngine.AddressableAssets.Addressables.LoadSceneAsync()
        }
    }
}

