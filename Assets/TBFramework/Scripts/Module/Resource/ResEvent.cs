

using System;
using UnityEngine;

namespace TBFramework.Resource
{
    public class ResEvent<T> : ResEventBase where T : UnityEngine.Object
    {
        public T asset;
        public event Action<T> actions;

        public Coroutine coroutine;

        public bool isDel = false;

        public ResEvent() { }

        public ResEvent(T asset, string name) : base(name)
        {
            SetAsset(asset);
        }

        public ResEvent(Action<T> action, string name) : base(name)
        {
            actions += action;
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

        public override void Reset()
        {
            this.name = null;
            this.refCount = 0;
            this.asset = null;
            this.actions = null;
            this.coroutine = null;
            this.isDel = false;
        }
    }
}
