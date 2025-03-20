
using System;
using TBFramework.Pool;

namespace TBFramework.Load.AssetBundles.Load
{
    public abstract class ABResBase : CBase
    {
        protected string name;
        protected int refCount = 0;

        public ABResBase() { }

        public ABResBase(string name)
        {
            SetName(name);
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public int RefCount
        {
            get { return refCount; }
        }

        public void AddRef()
        {
            refCount++;
        }

        public void SubRef()
        {
            refCount--;
            if (refCount < 0)
            {
                UnityEngine.Debug.LogError($"{name}的引用计数小于0！");
            }
        }

        public abstract UnityEngine.Object GetAsset();
        public abstract void Invoke(UnityEngine.Object obj);

        public abstract void StopCoroutine();

        public abstract void SetAsset(UnityEngine.Object obj);

        public abstract void AddAction(Action<UnityEngine.Object> action);

        public abstract void Print();

    }
}