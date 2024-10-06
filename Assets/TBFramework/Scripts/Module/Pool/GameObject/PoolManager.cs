using System;
using System.Collections.Generic;
using UnityEngine;
using TBFramework.Resource;
using UnityEditor;
using TBFramework.LoadInfo;
using TBFramework.AssetBundles;

namespace TBFramework.Pool
{
    /// <summary>
    /// 缓存池管理器
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<string, PoolData> poolDict = new Dictionary<string, PoolData>();//使用字典用键值对方式去存缓存池,键是缓存池中对象的加载地址

        private GameObject poolObj;//所有缓存池的根节点

        /// <summary>
        /// 提供给外部,从缓存池取游戏对象的方法
        /// </summary>
        /// <param name="poolName">游戏对象名和该游戏对象的缓存池名</param>
        /// <param name="callBack">获取到游戏对象后要对游戏对象进行的操作</param>
        /// <param name="fatherObj">获取游戏对象的父物体,默认为null</param>
        public void Pop(string poolName, Action<GameObject> callBack, Transform fatherObj = null, Action<string, Action<GameObject>> createNewObj = null, E_PoolMaxType maxType = E_PoolMaxType.InPool, int max = PoolSet.POOL_MAX_NUMBER)
        {
            if (poolObj == null && PoolSet.POOL_FINISH_OPEN)
            {
                poolObj = new GameObject(PoolSet.POOL_OBJECT_NAME);
            }
            //如果缓存池容器中存在该对象的缓存池并且缓存池中还有对象,则从缓存池中取
            if (poolDict.ContainsKey(poolName) && poolDict[poolName].CanPop)
            {
                callBack(poolDict[poolName].Pop(fatherObj));
            }
            else//如果没有则生成一个对象
            {
                Action<string, Action<GameObject>> createNew = createNewObj;
                if (createNew == null)
                {
                    createNew = DefaultCreate;
                }
                createNew(poolName, (o) =>
                {
                    GameObject obj = o;
                    //判断是否是预制体，是就实例化
                    if (PrefabUtility.GetPrefabInstanceStatus(o) != PrefabInstanceStatus.NotAPrefab || PrefabUtility.GetPrefabAssetType(o) != PrefabAssetType.NotAPrefab)
                    {
                        obj = GameObject.Instantiate(o);
                    }
                    obj.name = poolName;
                    obj.transform.SetParent(fatherObj);
                    if (!poolDict.ContainsKey(poolName))
                    {
                        poolDict.Add(poolName, new PoolData(obj as GameObject, poolObj, true, maxType, max));
                    }
                    else
                    {
                        poolDict[poolName].AddUse(obj as GameObject);
                    }
                    callBack(obj as GameObject);
                });
            }
        }
        /// <summary>
        /// 提供给外部调用,将不用的游戏对象重新压入缓存池
        /// </summary>
        /// <param name="poolName">游戏对象名和该游戏对象的缓存池名</param>
        /// <param name="obj">要压入缓存池的游戏对象</param>
        public void Push(string poolName, GameObject obj, E_PoolMaxType maxType = E_PoolMaxType.InPool, int max = PoolSet.POOL_MAX_NUMBER)
        {
            if (obj != null)
            {
                //在第一次压入游戏对象的时候,创建缓存池根节点
                if (poolObj == null && PoolSet.POOL_FINISH_OPEN)
                {
                    poolObj = new GameObject(PoolSet.POOL_OBJECT_NAME);
                }
                //如果有该游戏对象的缓存池,就直接压入,否则添加一个该游戏对象的缓存池
                if (poolDict.ContainsKey(poolName))
                {
                    poolDict[poolName].Push(obj);
                }
                else
                {
                    poolDict.Add(poolName, new PoolData(obj, poolObj, false, maxType, max));
                }
            }

        }

        /// <summary>
        /// 设置对象池最大容量
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="max"></param>
        public void SetPoolMaxNumber(string poolName, int max)
        {
            if (poolDict.ContainsKey(poolName))
            {
                poolDict[poolName].SetMaxNumber(max);
            }
        }

        /// <summary>
        /// 设置当前已有所有缓存池的最大容量
        /// </summary>
        /// <param name="max"></param>
        public void SetAllPoolMaxNumber(int max)
        {
            foreach (PoolData pool in poolDict.Values)
            {
                pool.SetMaxNumber(max);
            }
        }

        public void SetPoolMaxType(string poolName, E_PoolMaxType maxType)
        {
            if (poolDict.ContainsKey(poolName))
            {
                poolDict[poolName].SetMaxType(maxType);
            }
        }

        /// <summary>
        /// 提供给外部调用,清除缓存池的方法
        /// 一般用于场景切换时
        /// </summary>
        public void Clear()
        {
            poolDict.Clear();
            poolObj = null;
        }

        private void DefaultCreate(string poolName, Action<GameObject> action)
        {
            string name = LoadInfoManager.Instance.GetName(poolName, typeof(GameObject));
            LoadInfoManager.Instance.DoLoad<GameObject>(name, action, true);
        }
    }
}
