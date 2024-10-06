using System;
using System.Collections.Generic;
using UnityEngine;
using TBFramework.Mono;
using System.Reflection;

namespace TBFramework.Pool
{
    public class MonoCPoolManager : Singleton<MonoCPoolManager>
    {
        private Dictionary<Type, I_PoolData> monoCDict = new Dictionary<Type, I_PoolData>();

        private GameObject cPoolObj;//所有缓存池的根节点

        public MonoCPoolManager()
        {
            MonoConManager.Instance.AddUpdateListener(CheckAndRemove);
        }
        /// <summary>
        /// 从缓存池取出脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Pop<T>(E_PoolMaxType maxType = E_PoolMaxType.InPool, int max = PoolSet.POOL_MAX_NUMBER) where T : Behaviour
        {

            if (cPoolObj == null)
            {
                cPoolObj = new GameObject("C" + PoolSet.POOL_OBJECT_NAME);
            }
            if (monoCDict.ContainsKey(typeof(T)))
            {
                if ((monoCDict[typeof(T)] as MonoCPoolData<T>).CanPop)
                {
                    return (monoCDict[typeof(T)] as MonoCPoolData<T>).Pop();
                }
                else
                {
                    MonoCPoolData<T> cPool = monoCDict[typeof(T)] as MonoCPoolData<T>;
                    T monoC = cPool.FatherObj.AddComponent<T>();
                    cPool.AddUse(monoC);
                    monoC.enabled = true;
                    return monoC;
                }
            }
            else
            {
                monoCDict.Add(typeof(T), new MonoCPoolData<T>(cPoolObj, maxType, max));
                MonoCPoolData<T> cPool = monoCDict[typeof(T)] as MonoCPoolData<T>;
                T monoC = cPool.FatherObj.AddComponent<T>();
                cPool.AddUse(monoC);
                monoC.enabled = true;
                return monoC;
            }
        }

        public object Pop(Type type, E_PoolMaxType maxType = E_PoolMaxType.InPool, int max = PoolSet.POOL_MAX_NUMBER)
        {
            object obj = null;
            MethodInfo methodInfo = typeof(MonoCPoolManager).GetMethod("Pop", 1, new Type[] { typeof(E_PoolMaxType), typeof(int) });
            Type[] typeArguments = new Type[] { type };
            if (methodInfo != null && methodInfo.IsGenericMethodDefinition)
            {
                MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeArguments);
                object[] parameters = new object[] { maxType, max };
                obj = genericMethodInfo.Invoke(Instance, parameters);
            }
            return obj;
        }

        /// <summary>
        /// 将脚本压回缓存池
        /// </summary>
        /// <param name="monoC"></param>
        /// <typeparam name="T"></typeparam>
        public void Push<T>(T monoC, E_PoolMaxType maxType = E_PoolMaxType.InPool, int max = PoolSet.POOL_MAX_NUMBER) where T : Behaviour
        {
            if (monoC != null)
            {
                if (cPoolObj == null)
                {
                    cPoolObj = new GameObject("C" + PoolSet.POOL_OBJECT_NAME);
                }
                if (!monoCDict.ContainsKey(typeof(T)))
                {
                    monoCDict.Add(typeof(T), new MonoCPoolData<T>(cPoolObj, maxType, max));
                }
                (monoCDict[typeof(T)] as MonoCPoolData<T>).Push(monoC);
            }
        }

        public void Push(Behaviour c, E_PoolMaxType maxType = E_PoolMaxType.InPool, int max = PoolSet.POOL_MAX_NUMBER)
        {
            MethodInfo methodInfo = typeof(MonoCPoolManager).GetMethod("Push", 1, new Type[] { c.GetType(), typeof(E_PoolMaxType), typeof(int) });
            Type[] typeArguments = new Type[] { c.GetType() };
            if (methodInfo != null && methodInfo.IsGenericMethodDefinition)
            {
                MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeArguments);
                object[] parameters = new object[] { c, maxType, max };
                genericMethodInfo.Invoke(Instance, parameters);
            }
        }

        public void Clear()
        {
            monoCDict.Clear();
        }

        /// <summary>
        /// 看缓存池对象的父物体是否还在场景中存在,不存在就删除这个缓存池
        /// </summary>
        private void CheckAndRemove()
        {
            List<Type> list = new List<Type>();
            foreach (Type item in monoCDict.Keys)
            {
                if (monoCDict[item].FatherObj == null)
                {
                    list.Add(item);
                }
            }
            foreach (Type item in list)
            {
                monoCDict.Remove(item);
            }
        }

        /// <summary>
        /// 设置单个缓存池的最大容量
        /// </summary>
        /// <param name="max">最大容量数</param>
        /// <typeparam name="T">什么类的缓存池</typeparam>
        public void SetPoolMaxNumber<T>(int max) where T : Behaviour
        {
            if (monoCDict.ContainsKey(typeof(T)))
            {
                monoCDict[typeof(T)].SetMaxNumber(max);
            }
        }

        /// <summary>
        /// 设置所有缓存池的最大容量
        /// </summary>
        /// <param name="max">最大容量数</param>
        public void SetAllPoolMaxNumber(int max)
        {
            foreach (I_PoolData pool in monoCDict.Values)
            {
                pool.SetMaxNumber(max);
            }
        }

        public void SetPoolMaxType<T>(E_PoolMaxType maxType)
        {
            if (monoCDict.ContainsKey(typeof(T)))
            {
                monoCDict[typeof(T)].SetMaxType(maxType);
            }
        }
    }
}