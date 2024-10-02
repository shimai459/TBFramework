using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

namespace TBFramework.Mono
{
    public class MonoManager : Singleton<MonoManager>
    {
        private MonoController controller;

        public MonoManager()
        {
            controller = new GameObject("MonoController", typeof(MonoController)).GetComponent<MonoController>();
        }

        public void AddUpdateListener(Action action)
        {
            controller.updateEvent += action;
        }

        public void RemoveUpdateListener(Action action)
        {
            controller.updateEvent -= action;
        }

        public void AddFixedUpdateListener(Action action)
        {
            controller.fixedUpdateEvent += action;
        }

        public void RemoveFixedUpdateListener(Action action)
        {
            controller.fixedUpdateEvent -= action;
        }

        public void AddLateUpdateListener(Action action)
        {
            controller.lateUpdateEvent += action;
        }

        public void RemoveLateUpdateListener(Action action)
        {
            controller.lateUpdateEvent -= action;
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return controller?.StartCoroutine(routine);
        }

        public void StopAllCoroutines()
        {
            controller?.StopAllCoroutines();
        }

        public void StopCoroutine(IEnumerator routine)
        {
            controller?.StopCoroutine(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            controller?.StopCoroutine(routine);
        }
    }
}
