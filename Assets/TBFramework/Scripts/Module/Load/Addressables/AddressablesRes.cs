using System;
using TBFramework.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TBFramework.Load.Addressables
{
    public class AddressablesRes : CBase
    {
        protected string name;
        public AsyncOperationHandle handle;

        public Type type;

        protected int refCount = 0;

        public AddressablesRes() { }

        public AddressablesRes(string name, AsyncOperationHandle handle, Type type)
        {
            this.Init(name, handle, type);
        }

        public void Init(string name, AsyncOperationHandle handle, Type type)
        {
            this.name = name;
            this.handle = handle;
            this.type = type;
        }


        public override void Reset()
        {
            handle = default;
            refCount = 0;
            this.type = default;
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