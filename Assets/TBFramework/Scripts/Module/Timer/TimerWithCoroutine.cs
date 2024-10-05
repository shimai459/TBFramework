
using System;
using System.Collections;
using TBFramework.Mono;
using UnityEngine;

namespace TBFramework.Timer
{
    public class TimerWithCoroutine<T> : BaseTimer<T>
    {
        private Coroutine coroutine;

        public TimerWithCoroutine()
        {
            type = E_TimerType.Coroutine;
        }

        public TimerWithCoroutine(int uniqueKey, int intervalTime, Action<T> action, T param) : base(uniqueKey, intervalTime, action, param)
        {
            type = E_TimerType.Coroutine;
        }

        public override void Start()
        {
            if (!_isRunning)
            {
                base.Start();
                coroutine = MonoConManager.Instance.StartCoroutine(StartTimer());
            }
        }

        private IEnumerator StartTimer()
        {
            while (_isRunning)
            {
                yield return new WaitForSeconds(intervalTime / 1000f);
                action?.Invoke(param);
            }
        }

        public override void Stop()
        {
            base.Stop();
            if (coroutine != null)
            {
                MonoConManager.Instance.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public override void Reset()
        {
            base.Reset();
            coroutine = null;
        }
    }

}