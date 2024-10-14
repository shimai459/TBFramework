
using TBFramework.Pool;

namespace TBFramework.AssetBundles
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

    }
}