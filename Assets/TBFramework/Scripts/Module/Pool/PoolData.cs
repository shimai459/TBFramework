using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.Pool
{
    /// <summary>
    /// 一个GameObject缓存池的结构类
    /// </summary>
    public class PoolData : I_PoolData
    {
        private Stack<GameObject> poolStack;//存没有被使用的存储容器,这里使用了栈(也可以使用队列和泛型List)
        private List<GameObject> useList;//存被使用的对象的存储容器,这里使用了队列(也可以使用泛型List)
        public bool CanPop
        {
            get
            {
                return (maxType == E_PoolMaxType.Sum && useList.Count + poolStack.Count >= maxNumber && useList.Count > 0) || poolStack.Count > 0;
            }
        }

        /// <summary>
        /// 首次创建一种对象的缓存池
        /// </summary>
        /// <param name="firObj">第一个加入缓存池的对像</param>
        /// <param name="poolObj">缓存池的根节点</param>
        public PoolData(GameObject firObj, GameObject poolObj, bool isPop, E_PoolMaxType maxType, int max)
        {
            if (PoolSet.POOL_FINISH_OPEN)
            {
                fatherObj = new GameObject(firObj.name + " " + PoolSet.POOL_SINGLE_PARENT_EXTENSION);
                fatherObj.transform.parent = poolObj.transform;
            }
            poolStack = new Stack<GameObject>();
            useList = new List<GameObject>();
            this.maxType = maxType;
            maxNumber = max;
            if (isPop)
            {
                useList.Add(firObj);
            }
            else
            {
                Push(firObj);
            }
        }
        //从缓存池容器取对象
        public GameObject Pop(Transform fatherObj)
        {
            GameObject obj;
            if (maxType == E_PoolMaxType.Sum && useList.Count + poolStack.Count >= maxNumber && poolStack.Count <= 0)
            {
                obj = useList[0];
                useList.RemoveAt(0);
            }
            else
            {
                obj = poolStack.Pop();
            }
            useList.Add(obj);
            obj.transform.parent = fatherObj;
            obj.SetActive(true);
            return obj;
        }

        //将对象压入缓存池容器,如果超过最大容量,就将对象销毁
        public void Push(GameObject obj)
        {
            if (poolStack.Contains(obj))
            {
                return;
            }
            useList.Remove(obj);
            if (poolStack.Count > maxNumber)
            {
                GameObject.Destroy(obj);
                for (int i = 0; i < poolStack.Count - maxNumber; i++)
                {
                    GameObject.Destroy(Pop(null));
                }
            }
            else if (poolStack.Count == maxNumber)
            {

                GameObject.Destroy(obj);
            }
            else
            {
                poolStack.Push(obj);
                obj.SetActive(false);
                if (PoolSet.POOL_FINISH_OPEN)
                {
                    obj.transform.parent = fatherObj.transform;
                }

            }
        }

        public void AddUse(GameObject obj)
        {
            useList.Add(obj);
        }

    }
}
