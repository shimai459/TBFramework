
using System.Collections;
using System.Collections.Generic;

namespace TBFramework.Pool
{
    public class CPoolData<T> : I_PoolData where T : CBase, new()
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

        public CPoolData(E_PoolMaxType maxType, int max)
        {
            poolStack = new Stack<T>();
            useList = new List<T>();
            this.maxType = maxType;
            maxNumber = max;
        }

        public T Pop()
        {
            T c;
            if (maxType == E_PoolMaxType.Sum && useList.Count + poolStack.Count >= maxNumber && poolStack.Count <= 0)
            {
                c = useList[0];
                useList.RemoveAt(0);
            }
            else
            {
                c = poolStack.Pop();
            }
            useList.Add(c);
            return c;
        }

        public void Push(T c)
        {
            if (poolStack.Contains(c) || c == null)
            {
                return;
            }
            useList.Remove(c);
            if (poolStack.Count > maxNumber)
            {
                for (int i = 0; i < poolStack.Count - maxNumber; i++)
                {
                    Pop();
                }
            }
            else if (poolStack.Count < maxNumber)
            {
                poolStack.Push(c);
            }
        }

        public void AddUse(T c)
        {
            useList.Add(c);
        }
    }
}