
using System;
using TBFramework.Mono;
using Unity.VisualScripting;
using UnityEngine;

namespace TBFramework.Load.AssetBundles.Load
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

        public override UnityEngine.Object GetAsset()
        {
            return this.asset;
        }

        public override void Invoke(UnityEngine.Object obj)
        {
            actions?.Invoke(obj as T);
        }

        public override void StopCoroutine()
        {
            if (coroutine != null)
            {
                MonoConManager.Instance.StopCoroutine(this.coroutine);
            }
        }

        public override void SetAsset(UnityEngine.Object obj)
        {
            this.asset = obj as T;
            this.actions = null;
            this.coroutine = null;
        }

        public override void AddAction(Action<UnityEngine.Object> action)
        {
            this.actions += (obj) => action(obj as T);
        }

        public override void Print()
        {
            Debug.Log(this.name + " " + this.refCount + " " + this.asset);
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