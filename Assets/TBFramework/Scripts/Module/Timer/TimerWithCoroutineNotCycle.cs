using System;
using System.Collections;
using TBFramework.Mono;
using UnityEngine;

namespace TBFramework.Timer
{
    public class TimerWithCoroutineNotCycle<T> : BaseTimer<T>
    {
        private Coroutine coroutine;

        public TimerWithCoroutineNotCycle()
        {
            type = E_TimerType.CoroutineNotCycle;
        }

        public TimerWithCoroutineNotCycle(int uniqueKey, int intervalTime, Action<T> action, T param) : base(uniqueKey, intervalTime, action, param)
        {
            type = E_TimerType.CoroutineNotCycle;
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
            if (_isRunning)
            {
                yield return new WaitForSeconds(intervalTime / 1000f);
                action?.Invoke(param);
                coroutine = MonoConManager.Instance.StartCoroutine(StartTimer());
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