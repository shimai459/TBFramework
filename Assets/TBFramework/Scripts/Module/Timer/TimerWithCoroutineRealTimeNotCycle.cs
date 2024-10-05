
using System;
using System.Collections;
using TBFramework.Mono;
using UnityEngine;

namespace TBFramework.Timer
{
    public class TimerWithCoroutineRealTimeNotCycle<T> : BaseTimer<T>
    {
        private Coroutine coroutine;

        public TimerWithCoroutineRealTimeNotCycle()
        {
            type = E_TimerType.CoroutineRealTime;
        }

        public TimerWithCoroutineRealTimeNotCycle(int uniqueKey, int intervalTime, Action<T> action, T param) : base(uniqueKey, intervalTime, action, param)
        {
            type = E_TimerType.CoroutineRealTime;
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
                yield return new WaitForSecondsRealtime(intervalTime / 1000f);
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