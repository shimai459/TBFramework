using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.Pool
{
    public class CPoolData<T> : I_PoolData where T : Behaviour
    {
        private Stack<T> poolStack;

        private List<T> useList;

        public bool CanPop
        {
            get
            {
                return (maxType == E_PoolMaxType.Sum && useList.Count + poolStack.Count >= maxNumber && useList.Count > 0) || poolStack.Count > 0;
            }
        }

        public CPoolData(GameObject cPoolObj, E_PoolMaxType maxType, int max)
        {
            fatherObj = new GameObject(typeof(T).Name + " " + PoolSet.POOL_SINGLE_PARENT_EXTENSION);
            fatherObj.transform.SetParent(cPoolObj.transform);
            //GameObject.DontDestroyOnLoad(fatherObj);
            poolStack = new Stack<T>();
            useList = new List<T>();
            this.maxType = maxType;
            maxNumber = max;
        }

        public T Pop()
        {
            T monoC;
            if (maxType == E_PoolMaxType.Sum && useList.Count + poolStack.Count >= maxNumber && poolStack.Count <= 0)
            {
                monoC = useList[0];
                useList.RemoveAt(0);
            }
            else
            {
                monoC = poolStack.Pop();
            }
            useList.Add(monoC);
            monoC.enabled = true;
            return monoC;
        }

        public void Push(T monoC)
        {
            if (poolStack.Contains(monoC))
            {
                return;
            }
            useList.Remove(monoC);
            if (poolStack.Count > maxNumber)
            {
                GameObject.Destroy(monoC);
                for (int i = 0; i < poolStack.Count - maxNumber; i++)
                {
                    GameObject.Destroy(Pop());
                }
            }
            else if (poolStack.Count == maxNumber)
            {
                GameObject.Destroy(monoC);
            }
            else
            {
                poolStack.Push(monoC);
                monoC.enabled = false;
            }
        }

        public void AddUse(T monoC)
        {
            useList.Add(monoC);
        }
    }
}
