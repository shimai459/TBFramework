
using System;
using System.Collections.Generic;
using System.Reflection;
using TBFramework.AssetBundles;
using TBFramework.Resource;

namespace TBFramework.LoadInfo
{
    public class LoadInfoManager : Singleton<LoadInfoManager>
    {
        public Dictionary<string, LoadInfo> loadInfoDic = new Dictionary<string, LoadInfo>();

        public string GetName(string name, Type type)
        {
            return $"{name}_{type.Name}";
        }


        public void AddLoadInfo(string name, E_LoadType type, BaseLoadData loadData)
        {
            if (!loadInfoDic.ContainsKey(name))
            {
                LoadInfo loadInfo = new LoadInfo(name, type, loadData);
                loadInfoDic.Add(name, loadInfo);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"资源加载信息名重复：{name}");
            }
        }

        public void AddLoadInfo(LoadInfo loadInfo)
        {
            if (loadInfo != null)
            {
                if (!loadInfoDic.ContainsKey(loadInfo.name))
                {

                    loadInfoDic.Add(loadInfo.name, loadInfo);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"资源加载信息名重复：{loadInfo.name}");
                }
            }
        }

        public void AddLoadInfos(List<LoadInfo> loadInfos)
        {
            if (loadInfos != null)
            {
                foreach (LoadInfo loadInfo in loadInfos)
                {
                    AddLoadInfo(loadInfo);
                }
            }
        }

        public void AddLoadInfos(Action<Action<List<LoadInfo>>> add)
        {
            add?.Invoke(AddLoadInfos);
        }

        public LoadInfo GetLoadInfo(string name)
        {
            if (loadInfoDic.ContainsKey(name))
            {
                return loadInfoDic[name];
            }
            return null;
        }

        public void DoLoad<T>(string name, Action<T> action, bool isAsync) where T : UnityEngine.Object
        {
            LoadInfo info = GetLoadInfo(name);
            if (info != null)
            {
                switch (info.loadType)
                {
                    case E_LoadType.Resource:
                        ResourceLoadData rData = info.loadData as ResourceLoadData;
                        if (rData != null)
                        {
                            if (typeof(T) == rData.type)
                            {
                                if (isAsync)
                                {
                                    ResourceManager.Instance.LoadAsync<T>(rData.path, action);
                                }
                                else
                                {
                                    T res = ResourceManager.Instance.Load<T>(rData.path);
                                    action?.Invoke(res);
                                }
                            }
                        }
                        break;
                    case E_LoadType.AssetBundle:
                        AssetBundleLoadData abData = info.loadData as AssetBundleLoadData;
                        if (abData != null)
                        {
                            if (typeof(T) == abData.type)
                            {
                                if (isAsync)
                                {
                                    ABManager.Instance.LoadResAsync<T>(abData.abName, abData.resName, abData.pathURL, abData.mainName, action, true);
                                }
                                else
                                {
                                    T res = ABManager.Instance.LoadRes<T>(abData.abName, abData.resName, abData.pathURL, abData.mainName);
                                    action?.Invoke(res);
                                }
                            }
                        }
                        break;
                    case E_LoadType.Addressables:
                        break;
                    case E_LoadType.Custom:
                        CustomLoadData<T> cData = info.loadData as CustomLoadData<T>;
                        cData?.create?.Invoke(isAsync, action);
                        break;
                }
            }
        }

        public void RemoveLoadInfo(string name)
        {
            if (loadInfoDic.ContainsKey(name))
            {
                loadInfoDic.Remove(name);
            }
        }

        public void RemoveLoadInfo(LoadInfo loadInfo)
        {
            if (loadInfo != null)
            {
                if (loadInfoDic.ContainsKey(loadInfo.name))
                {
                    loadInfoDic.Remove(loadInfo.name);
                }
            }
        }

        public void RemoveLoadInfos(List<LoadInfo> loadInfos)
        {
            if (loadInfos != null)
            {
                foreach (LoadInfo loadInfo in loadInfos)
                {
                    RemoveLoadInfo(loadInfo);
                }
            }
        }

        public void RemoveLoadInfos(Action<Action<List<LoadInfo>>> remove)
        {
            remove?.Invoke(RemoveLoadInfos);
        }

        public void Clear()
        {
            loadInfoDic.Clear();
        }
    }
}
