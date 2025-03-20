
using System;
using System.Collections.Generic;
using TBFramework.Pool;
using UnityEngine.SceneManagement;

namespace TBFramework.Load.LoadInfo
{
    public class LoadInfoManager : Singleton<LoadInfoManager>
    {
        public Dictionary<string, LoadInfo> loadInfoDic = new Dictionary<string, LoadInfo>();

        public string GetName(string name, Type type)
        {
            return $"{name}_{type.Name}";
        }

        public void AddLoadInfo(string name, BaseLoadData loadData)
        {
            if (!loadInfoDic.ContainsKey(name))
            {
                LoadInfo loadInfo = CPoolManager.Instance.Pop<LoadInfo>();
                loadInfo.Init(name, loadData);
                loadInfoDic.Add(name, loadInfo);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"资源加载信息名重复：{name}");
            }
        }

        public void AddLoadInfos(List<(string name, BaseLoadData loadData)> infos)
        {
            AddLoadInfos(infos.ToArray());
        }

        public void AddLoadInfos(params (string name, BaseLoadData loadData)[] infos)
        {
            foreach (var info in infos)
            {
                AddLoadInfo(info.name, info.loadData);
            }
        }

        public void AddLoadInfos(Action<Action<List<(string name, BaseLoadData loadData)>>> add)
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

        public void DoLoad<T>(string name, Action<T> action = null, bool isAsync = true) where T : UnityEngine.Object
        {
            DoLoad<T>(name, action, isAsync, new LoadSceneParameters(LoadSceneMode.Single));
        }

        public void DoLoad<T>(string name, Action<T> action, bool isAsync, LoadSceneParameters param) where T : UnityEngine.Object
        {
            LoadInfo info = GetLoadInfo(name);
            if (info != null)
            {
                (info.loadData as BaseLoadData<T>).DoLoad(action, isAsync, param);
            }
        }

        public void DoUnload<T>(string name, Action action = null, bool isDel = true, UnloadSceneOptions options = UnloadSceneOptions.None) where T : UnityEngine.Object
        {
            LoadInfo info = GetLoadInfo(name);
            if (info != null)
            {
                (info.loadData as BaseLoadData<T>).DoUnload(action, isDel, options);
            }
        }

        public void RemoveLoadInfo(string name)
        {
            if (loadInfoDic.ContainsKey(name))
            {
                CPoolManager.Instance.Push(loadInfoDic[name]);
                loadInfoDic.Remove(name);
            }
        }

        public void RemoveLoadInfos(params string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                RemoveLoadInfo(names[i]);
            }
        }

        public void RemoveLoadInfos(List<string> names)
        {
            RemoveLoadInfos(names.ToArray());
        }

        public void RemoveLoadInfos(Action<Action<List<string>>> remove)
        {
            remove?.Invoke(RemoveLoadInfos);
        }

        public void Clear()
        {
            foreach (LoadInfo info in loadInfoDic.Values)
            {
                CPoolManager.Instance.Push(info);
            }
            loadInfoDic.Clear();
        }
    }
}
