
using System;
using System.Collections;
using TBFramework.Mono;
using UnityEngine;

namespace TBFramework.Timer
{
    public class TimerWithCoroutineRealTime<T> : BaseTimer<T>
    {
        private Coroutine coroutine;

        public TimerWithCoroutineRealTime()
        {
            type = E_TimerType.CoroutineRealTime;
        }

        public TimerWithCoroutineRealTime(int uniqueKey, int intervalTime, Action<T> action, T param) : base(uniqueKey, intervalTime, action, param)
        {
            type = E_TimerType.CoroutineRealTime;
        }

        public override void Start()
        {
            base.Start();
            coroutine = MonoManager.Instance.StartCoroutine(StartTimer());
        }

        private IEnumerator StartTimer()
        {
            while (_isRunning)
            {
                yield return new WaitForSecondsRealtime(intervalTime / 1000f);
                action?.Invoke(param);
            }
        }

        public override void Stop()
        {
            base.Stop();
            if (coroutine != null)
            {
                MonoManager.Instance.StopCoroutine(coroutine);
            }
        }

        public override void Reset()
        {
            base.Reset();
            coroutine = null;
        }
    }

}