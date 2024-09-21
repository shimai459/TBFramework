

using System;
using UnityEngine;

namespace TBFramework.Resource
{
    public class ResEvent<T> : ResEventBase where T : UnityEngine.Object
    {
        private string name;
        public T asset;
        public event Action<T> actions;

        public Coroutine coroutine;

        public bool isDel = false;

        public ResEvent(T asset,string name)
        {
            SetAsset(asset);
            this.name = name;
        }

        public ResEvent(Action<T> action,string name)
        {
            actions += action;
            this.name = name;
        }

        public void Invoke(T obj)
        {
            actions?.Invoke(obj);
        }

        public void SetAsset(T asset)
        {
            this.asset = asset;
            this.actions = null;
            this.coroutine = null;
        }


        public void AddRef()
        {
            refCount++;
        }

        public void SubRef()
        {
            refCount--;
            if(refCount<0){
                Debug.LogError($"{name}的引用计数小于0！");
            }
        }
    }
}
