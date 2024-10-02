
using System;
using System.Collections.Generic;

namespace TBFramework.Pool
{
    public class CPoolManager : Singleton<CPoolManager>
    {
        private Dictionary<Type, I_PoolData> cDict = new Dictionary<Type, I_PoolData>();

        /// <summary>
        /// 从缓存池取出脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Pop<T>(E_PoolMaxType maxType = E_PoolMaxType.InPool, int max = PoolSet.POOL_MAX_NUMBER) where T : CBase, new()
        {
            if (cDict.ContainsKey(typeof(T)))
            {
                if ((cDict[typeof(T)] as CPoolData<T>).CanPop)
                {
                    return (cDict[typeof(T)] as CPoolData<T>).Pop();
                }
                else
                {
                    CPoolData<T> cPool = cDict[typeof(T)] as CPoolData<T>;
                    T c = new T();
                    cPool.AddUse(c);
                    return c;
                }
            }
            else
            {
                cDict.Add(typeof(T), new CPoolData<T>(maxType, max));
                CPoolData<T> cPool = cDict[typeof(T)] as CPoolData<T>;
                T c = new T();
                cPool.AddUse(c);
                return c;
            }
        }

        /// <summary>
        /// 将脚本压回缓存池
        /// </summary>
        /// <param name="c"></param>
        /// <typeparam name="T"></typeparam>
        public void Push<T>(T c, E_PoolMaxType maxType = E_PoolMaxType.InPool, int max = PoolSet.POOL_MAX_NUMBER) where T : CBase, new()
        {
            c?.Reset();
            if (!cDict.ContainsKey(typeof(T)))
            {
                cDict.Add(typeof(T), new CPoolData<T>(maxType, max));
            }
            (cDict[typeof(T)] as CPoolData<T>).Push(c);
        }

        public void Clear()
        {
            cDict.Clear();
        }


        /// <summary>
        /// 设置单个缓存池的最大容量
        /// </summary>
        /// <param name="max">最大容量数</param>
        /// <typeparam name="T">什么类的缓存池</typeparam>
        public void SetPoolMaxNumber<T>(int max) where T : CBase, new()
        {
            if (cDict.ContainsKey(typeof(T)))
            {
                cDict[typeof(T)].SetMaxNumber(max);
            }
        }

        /// <summary>
        /// 设置所有缓存池的最大容量
        /// </summary>
        /// <param name="max">最大容量数</param>
        public void SetAllPoolMaxNumber(int max)
        {
            foreach (I_PoolData pool in cDict.Values)
            {
                pool.SetMaxNumber(max);
            }
        }
    }
}