using System.Collections.Generic;
using System.Linq;
using TBFramework.Pool;
using UnityEngine;

namespace TBFramework.AssetBundles
{
    /// <summary>
    /// AssetBundles资源加载管理类
    /// </summary>
    public class ABManager : Singleton<ABManager>
    {
        /// <summary>
        /// 存储不同套系的AB包,确保同一套系AB包存储在同一个文件夹下,用套系的存储地址+主包名区别套系
        /// </summary>
        /// <typeparam name="string">套系的存储地址+主包名</typeparam>
        /// <typeparam name="ABSingle">一套AB包</typeparam>
        /// <returns></returns>
        private Dictionary<string, ABSingle> abs = new Dictionary<string, ABSingle>();


        /// <summary>
        /// 增加一套AB包
        /// </summary>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        public void AddSingleAB(string pathURL, string mainName)
        {
            if (!abs.ContainsKey(pathURL + mainName))
            {
                ABSingle ab = CPoolManager.Instance.Pop<ABSingle>();
                ab.SetPathAndMainName(pathURL, mainName);
                abs.Add(pathURL + mainName, ab);
            }
        }

        /// <summary>
        /// 同步加载默认套系下的多个AB包
        /// </summary>
        /// <param name="abNames">多个AB包名</param>
        public void LoadABs(params string[] abNames)
        {
            LoadABs(ABSet.pathURL, ABSet.mainName, abNames);
        }

        /// <summary>
        /// 同步加载指定套系下的多个AB包
        /// </summary>
        /// <param name="pathURL">套系存储路径</param>
        /// <param name="mainName">主包名</param>
        /// <param name="abNames">多个AB包名</param>
        public void LoadABs(string pathURL, string mainName, params string[] abNames)
        {
            AddSingleAB(pathURL, mainName);
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].LoadABs(abNames);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        /// <summary>
        /// 同步加载默认套系中的一个AB包
        /// </summary>
        /// <param name="abName">AB包名</param>
        public void LoadAB(string abName)
        {
            LoadAB(abName, ABSet.pathURL, ABSet.mainName);
        }

        /// <summary>
        /// 同步加载指定套系的一个AB包
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        public void LoadAB(string abName, string pathURL, string mainName)
        {
            AddSingleAB(pathURL, mainName);
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].LoadAB(abName);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        /// <summary>
        /// 同步加载默认套系下一个AB包中的资源,不指定类型的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName)
        {
            return LoadRes(abName, resName, ABSet.pathURL, ABSet.mainName);
        }

        /// <summary>
        /// 同步加载指定套系下一个AB包的资源,不指定类型的方法
        /// </summary>
        /// <param name="abName">AB包的名字</param>
        /// <param name="resName">资源的名字</param>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName, string pathURL, string mainName)
        {
            AddSingleAB(pathURL, mainName);
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    return abs[pathURL + mainName].LoadRes(abName, resName);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
            return null;
        }

        /// <summary>
        /// 同步加载默认套系下一个AB包中的资源,用type指定类型的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="type">资源类型</param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName, System.Type type)
        {
            return LoadRes(abName, resName, type, ABSet.pathURL, ABSet.mainName);
        }

        /// <summary>
        /// 同步加载指定套系下一个AB包的资源,用type指定类型的方法
        /// </summary>
        /// <param name="abName">包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="type">资源类型</param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName, System.Type type, string pathURL, string mainName)
        {
            AddSingleAB(pathURL, mainName);
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    return abs[pathURL + mainName].LoadRes(abName, resName, type);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
            return null;
        }

        /// <summary>
        /// 同步加载默认套系下一个AB包中的资源,用泛型指定类型的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public T LoadRes<T>(string abName, string resName) where T : Object
        {
            return LoadRes<T>(abName, resName, ABSet.pathURL, ABSet.mainName);
        }

        /// <summary>
        /// 同步加载指定套系下一个AB包的资源,用泛型指定类型的方法
        /// </summary>
        /// <param name="abName">包名</param>
        /// <param name="resName">资源名</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        /// <returns></returns>
        public T LoadRes<T>(string abName, string resName, string pathURL, string mainName) where T : Object
        {
            AddSingleAB(pathURL, mainName);
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    return abs[pathURL + mainName].LoadRes<T>(abName, resName);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
            return null;
        }

        /// <summary>
        /// 异步加载默认套系下的多个AB包
        /// </summary>
        /// <param name="abNames">多个AB包名</param>
        public void LoadABsAsync(params string[] abNames)
        {
            LoadABsAsync(ABSet.pathURL, ABSet.mainName, abNames);
        }

        /// <summary>
        /// 异步加载指定套系下的多个AB包
        /// </summary>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        /// <param name="abNames">多个AB包名</param>
        public void LoadABsAsync(string pathURL, string mainName, params string[] abNames)
        {
            AddSingleAB(pathURL, mainName);
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].LoadABsAsync(abNames);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        /// <summary>
        /// 异步加载默认套系下的一个AB包的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="callBack">回调函数</param>
        public void LoadABAsync(string abName, System.Action<AssetBundle> callBack = null)
        {
            LoadABAsync(abName, ABSet.pathURL, ABSet.mainName, callBack);
        }

        /// <summary>
        /// 异步加载指定套系下的一个AB包的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        /// <param name="callBack">回调函数</param>
        public void LoadABAsync(string abName, string pathURL, string mainName, System.Action<AssetBundle> callBack = null)
        {
            AddSingleAB(pathURL, mainName);
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].LoadABAsync(abName, callBack);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        /// <summary>
        /// 异步加载默认套系下一个AB包中的资源,不指定类型的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        public void LoadResAsync(string abName, string resName, System.Action<Object> callBack, bool isABAsync = true)
        {
            LoadResAsync(abName, resName, ABSet.pathURL, ABSet.mainName, callBack, isABAsync);
        }


        /// <summary>
        /// 同步加载指定套系下一个AB包的资源,不指定类型的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        public void LoadResAsync(string abName, string resName, string pathURL, string mainName, System.Action<Object> callBack, bool isABAsync = true)
        {
            AddSingleAB(pathURL, mainName);
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].LoadResAsync(abName, resName, callBack, isABAsync);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        /// <summary>
        /// 异步加载默认套系下一个AB包中的资源,用type指定类型的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="type">资源类型</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        public void LoadResAsync(string abName, string resName, System.Type type, System.Action<Object> callBack, bool isABAsync = true)
        {
            LoadResAsync(abName, resName, type, ABSet.pathURL, ABSet.mainName, callBack, isABAsync);
        }

        /// <summary>
        /// 同步加载指定套系下一个AB包的资源,用type指定类型的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="type">资源类型</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        public void LoadResAsync(string abName, string resName, System.Type type, string pathURL, string mainName, System.Action<Object> callBack, bool isABAsync = true)
        {
            AddSingleAB(pathURL, mainName);
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].LoadResAsync(abName, resName, type, callBack, isABAsync);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }

        }

        /// <summary>
        /// 异步加载默认套系下一个AB包中的资源,用泛型指定类型的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void LoadResAsync<T>(string abName, string resName, System.Action<T> callBack, bool isABAsync = true) where T : Object
        {
            LoadResAsync<T>(abName, resName, ABSet.pathURL, ABSet.mainName, callBack, isABAsync);
        }

        /// <summary>
        /// 同步加载指定套系下一个AB包的资源,用泛型指定类型的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isABAsync">是否使用异步加载包</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        public void LoadResAsync<T>(string abName, string resName, string pathURL, string mainName, System.Action<T> callBack, bool isABAsync = true) where T : Object
        {
            AddSingleAB(pathURL, mainName);
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].LoadResAsync<T>(abName, resName, callBack, isABAsync);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }

        }

        /// <summary>
        /// 同步卸载默认套系下一个AB包的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="unloadRes">是否卸载加载的资源</param>
        public void UnloadAB(string abName, bool unloadRes)
        {
            UnloadAB(abName, ABSet.pathURL, ABSet.mainName, unloadRes);
        }

        /// <summary>
        /// 同步卸载指定套系下一个AB包的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="unloadRes">是否卸载加载的资源</param>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        public void UnloadAB(string abName, string pathURL, string mainName, bool unloadRes)
        {
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].UnloadAB(abName, unloadRes);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        /// <summary>
        /// 异步卸载默认套系下一个AB包的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="unloadRes">是否卸载加载的资源</param>
        /// <param name="callBack">回调函数</param>
        public void UnloadABAsync(string abName, bool unloadRes, System.Action callBack = null)
        {
            UnloadABAsync(abName, ABSet.pathURL, ABSet.mainName, unloadRes, callBack);
        }

        /// <summary>
        /// 异步卸载指定套系下一个AB包的方法
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        /// <param name="unloadRes">是否卸载加载的资源</param>
        /// <param name="callBack">回调函数</param>
        public void UnloadABAsync(string abName, string pathURL, string mainName, bool unloadRes, System.Action callBack = null)
        {
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].UnloadABAsync(abName, unloadRes, callBack);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        /// <summary>
        /// 同步卸载默认套系所有AB包的方法
        /// </summary>
        /// <param name="unloadRes">是否卸载加载的资源</param>
        public void UnloadOneSingleAB(bool unloadRes)
        {
            UnloadOneSingleAB(ABSet.pathURL, ABSet.mainName, unloadRes);
        }

        /// <summary>
        /// 同步卸载指定套系所有AB包的方法
        /// </summary>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        /// <param name="unloadRes">是否卸载加载的资源</param>
        public void UnloadOneSingleAB(string pathURL, string mainName, bool unloadRes)
        {
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].UnloadAllAB(unloadRes);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        /// <summary>
        /// 异步卸载默认套系所有AB包的方法
        /// </summary>
        /// <param name="unloadRes">是否卸载加载的资源</param>
        /// <param name="callBack">回调函数</param>
        public void UnloadOneSingleABAsync(bool unloadRes, System.Action callBack = null)
        {
            UnloadOneSingleABAsync(ABSet.pathURL, ABSet.mainName, unloadRes, callBack);
        }

        /// <summary>
        /// 异步卸载指定套系所有AB包的方法
        /// </summary>
        /// <param name="pathURL">套系的存储地址</param>
        /// <param name="mainName">主包名</param>
        /// <param name="unloadRes">是否卸载加载的资源</param>
        /// <param name="callBack">回调函数</param>
        public void UnloadOneSingleABAsync(string pathURL, string mainName, bool unloadRes, System.Action callBack = null)
        {
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].UnloadAllABAsync(unloadRes, callBack);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }


        /// <summary>
        /// 同步卸载所有套系的AB包
        /// </summary>
        /// <param name="unloadRes">是否卸载加载的资源</param>
        public void UnloadAllAB(bool unloadRes)
        {
            //只有在没有任何加载的情况下,才能去卸载所有包
            List<string> names = new List<string>();
            foreach (string name in abs.Keys)
            {
                if (abs[name] != null)
                {
                    if (abs[name].AsyncCount <= 0)
                    {
                        abs[name].UnloadAllAB(unloadRes);
                    }
                }
                else
                {
                    names.Add(name);
                }
            }
            foreach (string name in names)
            {
                abs.Remove(name);
            }
        }

        /// <summary>
        /// 异步卸载所有套系的AB包
        /// </summary>
        /// <param name="unloadRes">是否卸载加载的资源</param>
        /// <param name="callBack">回调函数</param>
        public void UnloadAllABAsync(bool unloadRes, System.Action callBack = null)
        {
            //只有在没有任何加载的情况下,才能去卸载所有包
            List<string> names = new List<string>();
            foreach (string name in abs.Keys)
            {
                if (abs[name] != null)
                {
                    if (abs[name].AsyncCount <= 0)
                    {
                        abs[name].UnloadAllABAsync(unloadRes, callBack);
                    }
                }
                else
                {
                    names.Add(name);
                }
            }
            foreach (string name in names)
            {
                abs.Remove(name);
            }
        }

        public void UnloadSet(string pathURL, string mainName)
        {
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    if (abs[pathURL + mainName].AsyncCount <= 0)
                    {
                        UnloadOneSingleAB(pathURL, mainName, true);
                        CPoolManager.Instance.Push(abs[pathURL + mainName]);
                        abs.Remove(pathURL + mainName);
                    }
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        public void UnloadSetAsync(string pathURL, string mainName, System.Action callBack = null)
        {
            UnloadOneSingleABAsync(pathURL, mainName, true, () =>
            {
                CPoolManager.Instance.Push(abs[pathURL + mainName]);
                abs.Remove(pathURL + mainName);
                callBack?.Invoke();
            });
        }

        public void UnloadAllSet()
        {
            List<string> remove = new List<string>();
            foreach (string name in abs.Keys)
            {
                ABSingle ab = abs[name];
                if (ab != null)
                {
                    if (ab.AsyncCount <= 0)
                    {
                        ab.UnloadAllAB(true);
                        CPoolManager.Instance.Push(ab);
                        remove.Add(name);
                    }
                }
                else
                {
                    remove.Add(name);
                }
            }
            foreach (string name in remove)
            {
                abs.Remove(name);
            }
        }

        public void UnloadAllSetAsync(System.Action callBack = null)
        {
            string[] names = abs.Keys.ToArray();
            List<string> remove = new List<string>();
            int number = 0;
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                ABSingle s = abs[name];
                if (s != null)
                {
                    if (s.AsyncCount <= 0)
                    {
                        s.UnloadAllABAsync(true, () =>
                        {
                            CPoolManager.Instance.Push(s);
                            number++;
                            remove.Add(name);
                            if (number == names.Length - 1)
                            {
                                foreach (string name in remove)
                                {
                                    abs.Remove(name);
                                }
                                callBack?.Invoke();
                            }
                        });
                    }
                }
                else
                {
                    number++;
                    remove.Add(name);
                }
            }
        }

        public void UnloadRes(string abName, string resName, System.Action<Object> callBack, bool isDel = false, bool isSub = true)
        {
            UnloadRes(abName, resName, ABSet.pathURL, ABSet.mainName, callBack, isDel, isSub);
        }

        public void UnloadRes(string abName, string resName, System.Type type, System.Action<Object> callBack, bool isDel = false, bool isSub = true)
        {
            UnloadRes(abName, resName, ABSet.pathURL, ABSet.mainName, type, callBack, isDel, isSub);

        }

        public void UnloadRes<T>(string abName, string resName, System.Action<Object> callBack, bool isDel = false, bool isSub = true) where T : Object
        {
            UnloadRes<T>(abName, resName, ABSet.pathURL, ABSet.mainName, callBack, isDel, isSub);
        }

        public void UnloadRes(string abName, string resName, string pathURL, string mainName, System.Action<Object> callBack, bool isDel = false, bool isSub = true)
        {
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].UnloadRes(abName, resName, callBack, isDel, isSub);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        public void UnloadRes(string abName, string resName, string pathURL, string mainName, System.Type type, System.Action<Object> callBack, bool isDel = false, bool isSub = true)
        {
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].UnloadRes(abName, resName, type, callBack, isDel, isSub);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

        public void UnloadRes<T>(string abName, string resName, string pathURL, string mainName, System.Action<Object> callBack, bool isDel = false, bool isSub = true) where T : Object
        {
            if (abs.ContainsKey(pathURL + mainName))
            {
                if (abs[pathURL + mainName] != null)
                {
                    abs[pathURL + mainName].UnloadRes<T>(abName, resName, callBack, isDel, isSub);
                }
                else
                {
                    abs.Remove(pathURL + mainName);
                }
            }
        }

    }
}