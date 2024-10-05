using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TBFramework.Mono;
using TBFramework.AssetBundles;

namespace TBFramework.Scene
{
    public class SceneManager : Singleton<SceneManager>
    {
        /// <summary>
        /// 同步加载场景,加载场景后执行程序逻辑
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="action">加载完场景后执行的逻辑</param>
        public void LoadScene(string sceneName, Action action = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, loadSceneMode);
            if (action != null)
            {
                action.Invoke();
            }
        }

        public void LoadScene(int sceneIndex, Action action = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, loadSceneMode);
            if (action != null)
            {
                action.Invoke();
            }
        }

        public void LoadScene(string abName, string sceneName, Action action = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            ABManager.Instance.LoadAB(abName);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, loadSceneMode);
            if (action != null)
            {
                action.Invoke();
            }
        }

        public void LoadScene(string pathURL, string mainName, string abName, string sceneName, Action action = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            ABManager.Instance.LoadAB(abName, pathURL, mainName);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, loadSceneMode);
            if (action != null)
            {
                action.Invoke();
            }
        }

        public void LoadSceneAsync(string sceneName, Action action = null, Action<AsyncOperation> loading = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            MonoConManager.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneName, action, loading, loadSceneMode));
        }

        private IEnumerator ReallyLoadSceneAsync(string sceneName, Action action, Action<AsyncOperation> loading, LoadSceneMode loadSceneMode)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            while (!ao.isDone)
            {
                loading?.Invoke(ao);
                yield return null;
            }
            action?.Invoke();
        }

        public void LoadSceneAsync(int sceneIndex, Action action = null, Action<AsyncOperation> loading = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            MonoConManager.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneIndex, action, loading, loadSceneMode));
        }

        private IEnumerator ReallyLoadSceneAsync(int sceneIndex, Action action, Action<AsyncOperation> loading, LoadSceneMode loadSceneMode)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);
            while (!ao.isDone)
            {
                loading?.Invoke(ao);
                yield return null;
            }
            action?.Invoke();
        }

        public void LoadSceneAsync(string abName, string sceneName, bool isAsync = false, Action action = null, Action<AsyncOperation> loading = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (isAsync)
            {
                ABManager.Instance.LoadABAsync(abName, (ab) =>
                {
                    LoadSceneAsync(sceneName, action, loading, loadSceneMode);
                });
            }
            else
            {
                ABManager.Instance.LoadAB(abName);
                LoadSceneAsync(sceneName, action, loading, loadSceneMode);
            }

        }

        public void LoadSceneAsync(string abName, string pathURL, string mainName, string sceneName, bool isAsync = false, Action action = null, Action<AsyncOperation> loading = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (isAsync)
            {
                ABManager.Instance.LoadABAsync(abName, pathURL, mainName, (ab) =>
                {
                    LoadSceneAsync(sceneName, action, loading, loadSceneMode);
                });
            }
            else
            {
                ABManager.Instance.LoadAB(abName, pathURL, mainName);
                LoadSceneAsync(sceneName, action, loading, loadSceneMode);
            }
        }


    }
}
