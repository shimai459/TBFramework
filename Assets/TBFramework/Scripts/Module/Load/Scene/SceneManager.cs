using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TBFramework.Mono;

namespace TBFramework.Load.Scene
{
    public class SceneManager : Singleton<SceneManager>
    {
        /// <summary>
        /// 同步加载场景,加载场景后执行程序逻辑
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="callBack">加载完场景后执行的逻辑</param>
        public void LoadScene(string sceneName, Action callBack = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, loadSceneMode);
            if (callBack != null)
            {
                callBack.Invoke();
            }
        }

        public void LoadScene(string sceneName, LoadSceneParameters param, Action callBack = null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, param);
            if (callBack != null)
            {
                callBack.Invoke();
            }
        }

        public void LoadScene(int sceneIndex, Action callBack = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, loadSceneMode);
            if (callBack != null)
            {
                callBack.Invoke();
            }
        }

        public void LoadScene(int sceneIndex, LoadSceneParameters param, Action callBack = null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, param);
            if (callBack != null)
            {
                callBack.Invoke();
            }
        }

        public void LoadSceneAsync(string sceneName, Action<AsyncOperation> loading = null, Action<AsyncOperation> callBack = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            LoadSceneParameters param = new LoadSceneParameters(loadSceneMode);
            MonoConManager.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneName, param, callBack, loading));
        }

        public void LoadSceneAsync(string sceneName, LoadSceneParameters param, Action<AsyncOperation> loading = null, Action<AsyncOperation> callBack = null)
        {
            MonoConManager.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneName, param, callBack, loading));
        }

        private IEnumerator ReallyLoadSceneAsync(string sceneName, LoadSceneParameters param, Action<AsyncOperation> loading, Action<AsyncOperation> callBack)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, param);
            while (!ao.isDone)
            {
                loading?.Invoke(ao);
                yield return null;
            }
            callBack?.Invoke(ao);
        }

        public void LoadSceneAsync(int sceneIndex, Action<AsyncOperation> loading = null, Action<AsyncOperation> callBack = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            LoadSceneParameters param = new LoadSceneParameters(loadSceneMode);
            MonoConManager.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneIndex, param, callBack, loading));
        }

        public void LoadSceneAsync(int sceneIndex, LoadSceneParameters param, Action<AsyncOperation> loading = null, Action<AsyncOperation> callBack = null)
        {
            MonoConManager.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneIndex, param, callBack, loading));
        }

        private IEnumerator ReallyLoadSceneAsync(int sceneIndex, LoadSceneParameters param, Action<AsyncOperation> loading, Action<AsyncOperation> callBack)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, param);
            while (!ao.isDone)
            {
                loading?.Invoke(ao);
                yield return null;
            }
            callBack?.Invoke(ao);
        }

        public void UnloadScene(int sceneIndex, Action<bool> callBack = null)
        {
            bool isunload = UnityEngine.SceneManagement.SceneManager.UnloadScene(sceneIndex);
            callBack?.Invoke(isunload);
        }

        public void UnloadScene(string sceneName, Action<bool> callBack = null)
        {
            bool isunload = UnityEngine.SceneManagement.SceneManager.UnloadScene(sceneName);
            callBack?.Invoke(isunload);
        }

        public void UnloadScene(UnityEngine.SceneManagement.Scene scene, Action<bool> callBack = null)
        {
            bool isunload = UnityEngine.SceneManagement.SceneManager.UnloadScene(scene);
            callBack?.Invoke(isunload);
        }

        public void UnloadSceneAsync(int sceneIndex, UnloadSceneOptions options = UnloadSceneOptions.None, Action<AsyncOperation> unloading = null, Action<AsyncOperation> callBack = null)
        {
            MonoConManager.Instance.StartCoroutine(ReallyUnloadSceneAsync(sceneIndex, options, unloading, callBack));
        }

        private IEnumerator ReallyUnloadSceneAsync(int sceneIndex, UnloadSceneOptions options, Action<AsyncOperation> unloading, Action<AsyncOperation> callback)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex, options);
            if (ao != null)
            {
                while (!ao.isDone)
                {
                    unloading?.Invoke(ao);
                    yield return null;
                }
                callback?.Invoke(ao);
            }
        }

        public void UnloadSceneAsync(string sceneName, UnloadSceneOptions options = UnloadSceneOptions.None, Action<AsyncOperation> unloading = null, Action<AsyncOperation> callBack = null)
        {
            MonoConManager.Instance.StartCoroutine(ReallyUnloadSceneAsync(sceneName, options, unloading, callBack));
        }

        private IEnumerator ReallyUnloadSceneAsync(string sceneName, UnloadSceneOptions options, Action<AsyncOperation> unloading, Action<AsyncOperation> callback)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName, options);
            if (ao != null)
            {
                while (!ao.isDone)
                {
                    unloading?.Invoke(ao);
                    yield return null;
                }
                callback?.Invoke(ao);
            }
        }

        public void UnloadSceneAsync(UnityEngine.SceneManagement.Scene scene, UnloadSceneOptions options = UnloadSceneOptions.None, Action<AsyncOperation> unloading = null, Action<AsyncOperation> callBack = null)
        {
            MonoConManager.Instance.StartCoroutine(ReallyUnloadSceneAsync(scene, options, unloading, callBack));
        }

        private IEnumerator ReallyUnloadSceneAsync(UnityEngine.SceneManagement.Scene scene, UnloadSceneOptions options, Action<AsyncOperation> unloading, Action<AsyncOperation> callback)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene, options);
            if (ao != null)
            {
                while (!ao.isDone)
                {
                    unloading?.Invoke(ao);
                    yield return null;
                }
                callback?.Invoke(ao);
            }
        }
    }
}
