using System;
using System.Collections.Generic;
using UnityEngine;
using TBFramework.Resource;

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
        /// <param name="pathName">游戏对象加载路径,也作为游戏对象名和该游戏对象的缓存池名</param>
        /// <param name="callBack">获取到游戏对象后要对游戏对象进行的操作</param>
        /// <param name="fatherObj">获取游戏对象的父物体,默认为null</param>
        public void Pop(string pathName, Action<GameObject> callBack, Transform fatherObj = null, E_PoolMaxType maxType = E_PoolMaxType.InPool, int max = PoolSet.POOL_MAX_NUMBER)
        {
            if (poolObj == null && PoolSet.POOL_FINISH_OPEN)
            {
                poolObj = new GameObject(PoolSet.POOL_OBJECT_NAME);
            }
            //如果缓存池容器中存在该对象的缓存池并且缓存池中还有对象,则从缓存池中取
            if (poolDict.ContainsKey(pathName) && poolDict[pathName].CanPop)
            {
                callBack(poolDict[pathName].Pop(fatherObj));
            }
            else//如果没有则生成一个对象
            {
                ResourceManager.Instance.LoadAsync<GameObject>(pathName, (o) =>
                {
                    GameObject obj =GameObject.Instantiate(o);
                    obj.name = pathName;
                    if (!poolDict.ContainsKey(pathName))
                    {
                        poolDict.Add(pathName, new PoolData(obj as GameObject, poolObj, true, maxType, max));
                    }else{
                        poolDict[pathName].AddUse(obj as GameObject);
                    }
                    callBack(obj as GameObject);
                });
            }
        }
        /// <summary>
        /// 提供给外部调用,将不用的游戏对象重新压入缓存池
        /// </summary>
        /// <param name="pathName">游戏对象加载路径,也作为游戏对象名和该游戏对象的缓存池名</param>
        /// <param name="obj">要压入缓存池的游戏对象</param>
        public void Push(string pathName, GameObject obj, E_PoolMaxType maxType = E_PoolMaxType.InPool, int max = PoolSet.POOL_MAX_NUMBER)
        {
            //在第一次压入游戏对象的时候,创建缓存池根节点
            if (poolObj == null && PoolSet.POOL_FINISH_OPEN)
            {
                poolObj = new GameObject(PoolSet.POOL_OBJECT_NAME);
            }
            //如果有该游戏对象的缓存池,就直接压入,否则添加一个该游戏对象的缓存池
            if (poolDict.ContainsKey(pathName))
            {
                poolDict[pathName].Push(obj);
            }
            else
            {
                poolDict.Add(pathName, new PoolData(obj, poolObj, false, maxType, max));
            }

        }

        /// <summary>
        /// 设置对象池最大容量
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="max"></param>
        public void SetPoolMaxNumber(string pathName, int max)
        {
            if (poolDict.ContainsKey(pathName))
            {
                poolDict[pathName].SetMaxNumber(max);
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

        public void SetPoolMaxType(string pathName, E_PoolMaxType maxType)
        {
            if (poolDict.ContainsKey(pathName))
            {
                poolDict[pathName].SetMaxType(maxType);
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
    }
}
