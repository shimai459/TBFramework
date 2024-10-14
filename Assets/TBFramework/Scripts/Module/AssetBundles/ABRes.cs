
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace TBFramework.AssetBundles
{
    public class ABRes<T> : ABResBase where T : UnityEngine.Object
    {

        public T asset;
        public event Action<T> actions;

        public Coroutine coroutine;

        public bool isDel = false;

        public ABRes() { }

        public ABRes(T asset, string name) : base(name)
        {
            SetAsset(asset);
        }

        public ABRes(Action<T> action, string name) : base(name)
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