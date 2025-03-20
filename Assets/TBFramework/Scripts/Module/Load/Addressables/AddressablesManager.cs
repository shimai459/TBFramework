
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TBFramework.Pool;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TBFramework.Load.Addressables
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
    public class AddressablesManager : Singleton<AddressablesManager>
    {
        public Dictionary<string, AddressablesRes> resDic = new Dictionary<string, AddressablesRes>();

        public void LoadAsync(string resName, System.Type type, System.Action<AsyncOperationHandle> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null)
        {
            MethodInfo methodInfo = typeof(AddressablesManager).GetMethod("LoadAsync", 1, new System.Type[] { typeof(string), typeof(System.Action<AsyncOperationHandle>), typeof(System.Action<AsyncOperationHandle>), typeof(System.Action<AsyncOperationHandle>) });
            if (methodInfo != null && methodInfo.IsGenericMethodDefinition)
            {
                System.Type[] typeArguments = new System.Type[] { type };
                MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeArguments);
                object[] parameters = new object[] { resName, completed, destroyed, completedTypeless };
                genericMethodInfo.Invoke(this, parameters);
            }
        }

        public void LoadAsync(string resName, System.Type type, System.Action<Object> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null)
        {
            MethodInfo[] methods = typeof(AddressablesManager).GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (method.Name == "LoadAsync" && method.IsGenericMethodDefinition)
                {
                    // 检查方法的参数类型
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 4 &&
                        parameters[0].ParameterType == typeof(string) &&
                        parameters[1].ParameterType.ToString() == "System.Action`1[T]" &&
                        parameters[2].ParameterType == typeof(System.Action<AsyncOperationHandle>) &&
                        parameters[3].ParameterType == typeof(System.Action<AsyncOperationHandle>))
                    {
                        object[] param = new object[] { resName, completed, destroyed, completedTypeless };
                        method.Invoke(this, param);
                    }
                }
            }
        }

        public void LoadsAsync(System.Type type, UnityEngine.AddressableAssets.Addressables.MergeMode mode, System.Action<AsyncOperationHandle> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null, params string[] keys)
        {
            MethodInfo methodInfo = typeof(AddressablesManager).GetMethod("LoadsAsync", 1, new System.Type[] { typeof(UnityEngine.AddressableAssets.Addressables.MergeMode), typeof(System.Action<AsyncOperationHandle>), typeof(System.Action<AsyncOperationHandle>), typeof(System.Action<AsyncOperationHandle>), typeof(string[]) });
            if (methodInfo != null && methodInfo.IsGenericMethodDefinition)
            {
                System.Type[] typeArguments = new System.Type[] { type };
                MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeArguments);
                object[] parameters = new object[] { mode, completed, destroyed, completedTypeless, keys };
                genericMethodInfo.Invoke(this, parameters);
            }
        }

        public void LoadsAsync(System.Type type, UnityEngine.AddressableAssets.Addressables.MergeMode mode, System.Action<Object> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null, params string[] keys)
        {
            MethodInfo[] methods = typeof(AddressablesManager).GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (method.Name == "LoadsAsync" && method.IsGenericMethodDefinition)
                {
                    // 检查方法的参数类型
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 5 &&
                        parameters[0].ParameterType == typeof(UnityEngine.AddressableAssets.Addressables.MergeMode) &&
                        parameters[1].ParameterType.ToString() == "System.Action`1[T]" &&
                        parameters[2].ParameterType == typeof(System.Action<AsyncOperationHandle>) &&
                        parameters[3].ParameterType == typeof(System.Action<AsyncOperationHandle>) &&
                        parameters[4].ParameterType == typeof(string[]))
                    {
                        object[] param = new object[] { mode, completed, destroyed, completedTypeless, keys };
                        method.Invoke(this, param);
                    }
                }
            }
        }

        public void LoadAsync<T>(string resName, System.Action<AsyncOperationHandle<T>> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null)
        {
            if (typeof(T) == typeof(SceneInstance))
            {
                this.LoadSceneAsync(resName,
                (AsyncOperationHandle<SceneInstance> a) =>
                {
                    AsyncOperationHandle b = a;
                    completed(b.Convert<T>());
                },
                destroyed,
                completedTypeless);
                return;
            }
            string keyName = GetKeyName(resName, typeof(T));
            AsyncOperationHandle<T> handle;
            if (resDic.ContainsKey(keyName))
            {
                handle = resDic[keyName].handle.Convert<T>();
                if (handle.IsDone)
                {
                    CompleteCheckLoadIsSuccess(handle);
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    CompleteTypelessCheckLoadIsSuccess(handle);
                }
                else
                {
                    handle.Completed += CompleteCheckLoadIsSuccess;
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                }
            }
            else
            {
                handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(resName);
                handle.Completed += CompleteCheckLoadIsSuccess;
                if (destroyed != null)
                    handle.Destroyed += destroyed;
                handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                AddressablesRes res = CPoolManager.Instance.Pop<AddressablesRes>();
                res.Init(keyName, handle, typeof(T));
                resDic.Add(keyName, res);
            }
            resDic[keyName].AddRef();

            void CompleteCheckLoadIsSuccess(AsyncOperationHandle<T> h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadImmediately<T>(resName);
                }
                if (completed != null)
                {
                    completed(h);
                }
            }

            void CompleteTypelessCheckLoadIsSuccess(AsyncOperationHandle h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadImmediately<T>(resName);
                }
                if (completedTypeless != null)
                {
                    completedTypeless(h);
                }
            }
        }

        public void LoadAsync<T>(string resName, System.Action<AsyncOperationHandle> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null)
        {
            if (typeof(T) == typeof(SceneInstance))
            {
                this.LoadSceneAsync(resName,
                (AsyncOperationHandle<SceneInstance> a) =>
                {
                    AsyncOperationHandle b = a;
                    completed(b.Convert<T>());
                },
                destroyed,
                completedTypeless);
                return;
            }
            string keyName = GetKeyName(resName, typeof(T));
            AsyncOperationHandle<T> handle;
            if (resDic.ContainsKey(keyName))
            {
                handle = resDic[keyName].handle.Convert<T>();
                if (handle.IsDone)
                {
                    CompleteCheckLoadIsSuccess(handle);
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    CompleteTypelessCheckLoadIsSuccess(handle);
                }
                else
                {
                    handle.Completed += CompleteCheckLoadIsSuccess;
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                }
            }
            else
            {
                handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(resName);
                handle.Completed += CompleteCheckLoadIsSuccess;
                if (destroyed != null)
                    handle.Destroyed += destroyed;
                handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                AddressablesRes res = CPoolManager.Instance.Pop<AddressablesRes>();
                res.Init(keyName, handle, typeof(T));
                resDic.Add(keyName, res);
            }
            resDic[keyName].AddRef();

            void CompleteCheckLoadIsSuccess(AsyncOperationHandle<T> h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadImmediately<T>(resName);
                }
                if (completed != null)
                {
                    completed(h);
                }
            }

            void CompleteTypelessCheckLoadIsSuccess(AsyncOperationHandle h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadImmediately<T>(resName);
                }
                if (completedTypeless != null)
                {
                    completedTypeless(h);
                }
            }
        }

        public void LoadAsync<T>(string resName, System.Action<T> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null)
        {
            if (typeof(T) == typeof(SceneInstance))
            {
                this.LoadSceneAsync(resName,
                (AsyncOperationHandle<SceneInstance> a) =>
                {
                    AsyncOperationHandle b = a;
                    completed(b.Convert<T>().Result);
                },
                destroyed,
                completedTypeless);
                return;
            }
            string keyName = GetKeyName(resName, typeof(T));
            AsyncOperationHandle<T> handle;
            if (resDic.ContainsKey(keyName))
            {
                handle = resDic[keyName].handle.Convert<T>();
                if (handle.IsDone)
                {
                    CompleteCheckLoadIsSuccess(handle);
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    CompleteTypelessCheckLoadIsSuccess(handle);
                }
                else
                {
                    handle.Completed += CompleteCheckLoadIsSuccess;
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                }
            }
            else
            {
                handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(resName);
                handle.Completed += CompleteCheckLoadIsSuccess;
                if (destroyed != null)
                    handle.Destroyed += destroyed;
                handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                AddressablesRes res = CPoolManager.Instance.Pop<AddressablesRes>();
                res.Init(keyName, handle, typeof(T));
                resDic.Add(keyName, res);
            }
            resDic[keyName].AddRef();

            void CompleteCheckLoadIsSuccess(AsyncOperationHandle<T> h)
            {
                if (h.Status == AsyncOperationStatus.Succeeded)
                {
                    if (completed != null)
                    {
                        completed(h.Result);
                    }
                }
                else
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadImmediately<T>(resName);
                }
            }

            void CompleteTypelessCheckLoadIsSuccess(AsyncOperationHandle h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadImmediately<T>(resName);
                }
                if (completedTypeless != null)
                {
                    completedTypeless(h);
                }
            }
        }

        public void LoadsAsync<T>(UnityEngine.AddressableAssets.Addressables.MergeMode mode, System.Action<AsyncOperationHandle<IList<T>>> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null, params string[] keys)
        {
            string keyName = GetKeyName(typeof(T), mode, keys);
            AsyncOperationHandle<IList<T>> handle;
            if (resDic.ContainsKey(keyName))
            {
                handle = resDic[keyName].handle.Convert<IList<T>>();
                if (handle.IsDone)
                {
                    CompleteCheckLoadIsSuccess(handle);
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    CompleteTypelessCheckLoadIsSuccess(handle);
                }
                else
                {
                    handle.Completed += CompleteCheckLoadIsSuccess;
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                }

            }
            else
            {
                //不存在做什么
                handle = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<T>(keys.ToList<string>(), null, mode);
                handle.Completed += CompleteCheckLoadIsSuccess;
                if (destroyed != null)
                    handle.Destroyed += destroyed;
                handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                AddressablesRes res = CPoolManager.Instance.Pop<AddressablesRes>();
                res.Init(keyName, handle, typeof(T));
                resDic.Add(keyName, res);
            }
            resDic[keyName].AddRef();

            void CompleteCheckLoadIsSuccess(AsyncOperationHandle<IList<T>> h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadsImmediately<T>(mode, keys);
                }
                if (completed != null)
                {
                    completed(h);
                }
            }

            void CompleteTypelessCheckLoadIsSuccess(AsyncOperationHandle h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadsImmediately<T>(mode, keys);
                }
                if (completedTypeless != null)
                {
                    completedTypeless(h);
                }
            }
        }

        public void LoadsAsync<T>(UnityEngine.AddressableAssets.Addressables.MergeMode mode, System.Action<AsyncOperationHandle> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null, params string[] keys)
        {
            string keyName = GetKeyName(typeof(T), mode, keys);
            AsyncOperationHandle<IList<T>> handle;
            if (resDic.ContainsKey(keyName))
            {
                handle = resDic[keyName].handle.Convert<IList<T>>();
                if (handle.IsDone)
                {
                    CompleteCheckLoadIsSuccess(handle);
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    CompleteTypelessCheckLoadIsSuccess(handle);
                }
                else
                {
                    handle.Completed += CompleteCheckLoadIsSuccess;
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                }

            }
            else
            {
                //不存在做什么
                handle = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<T>(keys.ToList<string>(), null, mode);
                handle.Completed += CompleteCheckLoadIsSuccess;
                if (destroyed != null)
                    handle.Destroyed += destroyed;
                handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                AddressablesRes res = CPoolManager.Instance.Pop<AddressablesRes>();
                res.Init(keyName, handle, typeof(T));
                resDic.Add(keyName, res);
            }
            resDic[keyName].AddRef();

            void CompleteCheckLoadIsSuccess(AsyncOperationHandle<IList<T>> h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadsImmediately<T>(mode, keys);
                }
                if (completed != null)
                {
                    completed(h);
                }
            }

            void CompleteTypelessCheckLoadIsSuccess(AsyncOperationHandle h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadsImmediately<T>(mode, keys);
                }
                if (completedTypeless != null)
                {
                    completedTypeless(h);
                }
            }
        }

        public void LoadsAsync<T>(UnityEngine.AddressableAssets.Addressables.MergeMode mode, System.Action<T> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null, params string[] keys)
        {
            string keyName = GetKeyName(typeof(T), mode, keys);
            AsyncOperationHandle<IList<T>> handle;
            if (resDic.ContainsKey(keyName))
            {
                handle = resDic[keyName].handle.Convert<IList<T>>();
                if (handle.IsDone)
                {
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    foreach (T item in handle.Result)
                    {
                        CompleteCheckLoadIsSuccess(handle);
                        CompleteTypelessCheckLoadIsSuccess(handle);
                    }
                }
                else
                {
                    handle.Completed += CompleteCheckLoadIsSuccess;
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                }

            }
            else
            {
                //不存在做什么
                handle = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<T>(keys.ToList<string>(), null, mode);
                handle.Completed += CompleteCheckLoadIsSuccess;
                if (destroyed != null)
                    handle.Destroyed += destroyed;
                handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                AddressablesRes res = CPoolManager.Instance.Pop<AddressablesRes>();
                res.Init(keyName, handle, typeof(T));
                resDic.Add(keyName, res);
            }
            resDic[keyName].AddRef();

            void CompleteCheckLoadIsSuccess(AsyncOperationHandle<IList<T>> h)
            {
                if (h.Status == AsyncOperationStatus.Succeeded)
                {
                    if (completed != null)
                    {
                        foreach (T item in h.Result)
                        {
                            completed(item);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadsImmediately<T>(mode, keys);
                }
            }

            void CompleteTypelessCheckLoadIsSuccess(AsyncOperationHandle h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadsImmediately<T>(mode, keys);
                }
                if (completedTypeless != null)
                {
                    completedTypeless(h);
                }
            }
        }

        public void Unload(string resName, System.Type type, bool isDestroy = true)
        {
            Unload(GetKeyName(resName, type), isDestroy);
        }

        public void Unloads(System.Type type, UnityEngine.AddressableAssets.Addressables.MergeMode mode, bool isDestroy = true, params string[] keys)
        {
            Unload(GetKeyName(type, mode, keys), isDestroy);
        }

        public void Unload<T>(string resName, bool isDestroy = true)
        {
            Unload(resName, typeof(T), isDestroy);
        }

        public void Unloads<T>(UnityEngine.AddressableAssets.Addressables.MergeMode mode, bool isDestroy = true, params string[] keys)
        {
            Unloads(typeof(T), mode, isDestroy, keys);
        }

        public void Unload(string keyName, bool isDestroy = true)
        {
            if (resDic.ContainsKey(keyName))
            {
                resDic[keyName].SubRef();
                if (resDic[keyName].RefCount == 0 && isDestroy)
                {
                    //取出字典里面的对象
                    UnityEngine.AddressableAssets.Addressables.Release(resDic[keyName].handle);
                    CPoolManager.Instance.Push(resDic[keyName]);
                    resDic.Remove(keyName);
                }
            }
        }

        public void UnloadImmediately(string resName, System.Type type)
        {
            UnloadImmediately(GetKeyName(resName, type));
        }

        public void UnloadsImmediately(System.Type type, UnityEngine.AddressableAssets.Addressables.MergeMode mode, params string[] keys)
        {
            UnloadImmediately(GetKeyName(type, mode, keys));
        }

        public void UnloadImmediately<T>(string resName)
        {
            UnloadImmediately(resName, typeof(T));
        }

        public void UnloadsImmediately<T>(UnityEngine.AddressableAssets.Addressables.MergeMode mode, params string[] keys)
        {
            UnloadsImmediately(typeof(T), mode, keys);
        }

        public void UnloadImmediately(string keyName)
        {
            if (resDic.ContainsKey(keyName))
            {
                UnityEngine.AddressableAssets.Addressables.Release(resDic[keyName].handle);
                CPoolManager.Instance.Push(resDic[keyName]);
                resDic.Remove(keyName);
            }
        }

        public void LoadSceneAsync(string key, System.Action<AsyncOperationHandle<SceneInstance>> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null, LoadSceneMode mode = LoadSceneMode.Single, bool activeOnLoad = true, int priority = 100)
        {
            string keyName = GetKeyName(key, typeof(SceneInstance));
            AsyncOperationHandle<SceneInstance> handle;
            if (resDic.ContainsKey(keyName))
            {
                handle = resDic[keyName].handle.Convert<SceneInstance>();
                if (handle.IsDone)
                {
                    CompleteCheckLoadIsSuccess(handle);
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    CompleteTypelessCheckLoadIsSuccess(handle);
                }
                else
                {
                    handle.Completed += CompleteCheckLoadIsSuccess;
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                }
            }
            else
            {
                handle = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(key, mode, activeOnLoad, priority);
                handle.Completed += CompleteCheckLoadIsSuccess;
                if (destroyed != null)
                    handle.Destroyed += destroyed;
                handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                AddressablesRes res = CPoolManager.Instance.Pop<AddressablesRes>();
                res.Init(keyName, handle, typeof(SceneInstance));
                resDic.Add(keyName, res);
            }
            resDic[keyName].AddRef();

            void CompleteCheckLoadIsSuccess(AsyncOperationHandle<SceneInstance> h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadSceneImmediately(key);
                }
                if (completed != null)
                {
                    completed(h);
                }
            }

            void CompleteTypelessCheckLoadIsSuccess(AsyncOperationHandle h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadSceneImmediately(key);
                }
                if (completedTypeless != null)
                {
                    completedTypeless(h);
                }
            }

        }

        public void LoadSceneAsync(string key, System.Action<SceneInstance> completed, System.Action<AsyncOperationHandle> destroyed = null, System.Action<AsyncOperationHandle> completedTypeless = null, LoadSceneMode mode = LoadSceneMode.Single, bool activeOnLoad = true, int priority = 100)
        {
            string keyName = GetKeyName(key, typeof(SceneInstance));
            AsyncOperationHandle<SceneInstance> handle;

            if (resDic.ContainsKey(keyName))
            {
                handle = resDic[keyName].handle.Convert<SceneInstance>();
                if (handle.IsDone)
                {
                    CompleteCheckLoadIsSuccess(handle);
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    CompleteTypelessCheckLoadIsSuccess(handle);
                }
                else
                {
                    handle.Completed += CompleteCheckLoadIsSuccess;
                    if (destroyed != null)
                        handle.Destroyed += destroyed;
                    handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                }
            }
            else
            {
                handle = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(key, mode, activeOnLoad, priority);
                handle.Completed += CompleteCheckLoadIsSuccess;
                if (destroyed != null)
                    handle.Destroyed += destroyed;
                handle.CompletedTypeless += CompleteTypelessCheckLoadIsSuccess;
                AddressablesRes res = CPoolManager.Instance.Pop<AddressablesRes>();
                res.Init(keyName, handle, typeof(SceneInstance));
                resDic.Add(keyName, res);
            }
            resDic[keyName].AddRef();

            void CompleteCheckLoadIsSuccess(AsyncOperationHandle<SceneInstance> h)
            {
                if (h.Status == AsyncOperationStatus.Succeeded)
                {
                    if (completed != null)
                    {
                        completed(h.Result);
                    }
                }
                else
                {
                    Debug.LogWarning(keyName + "场景资源加载失败");
                    UnloadSceneImmediately(key);
                }
            }

            void CompleteTypelessCheckLoadIsSuccess(AsyncOperationHandle h)
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning(keyName + "资源加载失败");
                    UnloadSceneImmediately(key);
                }
                if (completedTypeless != null)
                {
                    completedTypeless(h);
                }
            }

        }

        public void UnloadScene(string key, UnloadSceneOptions options = UnloadSceneOptions.None, bool isDestroy = true)
        {
            string keyName = GetKeyName(key, typeof(SceneInstance));
            if (resDic.ContainsKey(keyName))
            {
                //释放时 引用计数-1
                resDic[keyName].SubRef();
                //如果引用计数为0  才真正的释放
                if (resDic[keyName].RefCount == 0 && isDestroy)
                {
                    UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(resDic[keyName].handle, options);
                    CPoolManager.Instance.Push(resDic[keyName]);
                    resDic.Remove(keyName);
                }
            }
        }

        public void UnloadSceneImmediately(string key, UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            string keyName = GetKeyName(key, typeof(SceneInstance));
            if (resDic.ContainsKey(keyName))
            {
                UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(resDic[keyName].handle, options);
                CPoolManager.Instance.Push(resDic[keyName]);
                resDic.Remove(keyName);
            }
        }

        public string GetKeyName(string resName, System.Type type)
        {

            return resName + "_" + type.Name;
        }

        public string GetKeyName(System.Type type, UnityEngine.AddressableAssets.Addressables.MergeMode mode, params string[] keys)
        {
            string keyName = "";
            for (int i = 0; i < keys.Length; i++)
                keyName += keys[i] + "_";
            keyName += type.Name + "_" + mode.ToString();
            return keyName;
        }

        public void Clear()
        {
            foreach (var item in resDic.Values)
            {
                if (item.type == typeof(SceneInstance))
                {
                    UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(item.handle);
                }
                else
                {
                    UnityEngine.AddressableAssets.Addressables.Release(item.handle);
                }
                CPoolManager.Instance.Push(item);

            }
            resDic.Clear();
        }
    }
}

