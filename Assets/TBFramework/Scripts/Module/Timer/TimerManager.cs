
using System;
using System.Collections.Generic;
using System.Reflection;
using TBFramework.Pool;
using TBFramework.Util;

namespace TBFramework.Timer
{
    public class TimerManager : Singleton<TimerManager>
    {
        private Dictionary<int, I_BaseTimer> timers = new Dictionary<int, I_BaseTimer>();

        private List<int> uniqueKeys = new List<int>();

        public BaseTimer<T> CreateTimer<T>(E_TimerType type, int intervalTime, Action<T> action, T param)
        {
            BaseTimer<T> timer = null;
            int key = UniqueKeyUtil.GetUnusedKey(uniqueKeys);
            switch (type)
            {
                case E_TimerType.Async:
                    timer = CPoolManager.Instance.Pop<TimerWithAsync<T>>();
                    break;
                case E_TimerType.Coroutine:
                    timer = CPoolManager.Instance.Pop<TimerWithCoroutine<T>>();
                    break;
                case E_TimerType.Task:
                    timer = CPoolManager.Instance.Pop<TimerWithTask<T>>();
                    break;
                case E_TimerType.Thread:
                    timer = CPoolManager.Instance.Pop<TimerWithThread<T>>();
                    break;
                case E_TimerType.ThreadPool:
                    timer = CPoolManager.Instance.Pop<TimerWithThreadPool<T>>();
                    break;
                case E_TimerType.CoroutineRealTime:
                    timer = CPoolManager.Instance.Pop<TimerWithCoroutineRealTime<T>>();
                    break;
                case E_TimerType.CoroutineNotCycle:
                    timer = CPoolManager.Instance.Pop<TimerWithCoroutineNotCycle<T>>();
                    break;
                case E_TimerType.CoroutineRealTimeNotCycle:
                    timer = CPoolManager.Instance.Pop<TimerWithCoroutineRealTimeNotCycle<T>>();
                    break;
            }
            timer.SetValue(key, intervalTime);
            timer.SetAction(action);
            timer.SetParam(param);
            if (timer != null)
            {
                timers.Add(key, timer);
                uniqueKeys.Add(key);
            }
            return timer;
        }

        public void RemoveTimer(int key)
        {
            if (uniqueKeys.Contains(key))
            {
                uniqueKeys.Remove(key);
            }
            if (timers.ContainsKey(key))
            {
                CPoolManager.Instance.Push(timers[key]);
                timers.Remove(key);
            }
        }

        public void RemoveTimer(I_BaseTimer timer)
        {
            RemoveTimer(timer.UniqueKey);
        }

        public void Clear()
        {
            foreach (I_BaseTimer timer in timers.Values)
            {
                CPoolManager.Instance.Push(timer);
            }
            timers.Clear();
            uniqueKeys.Clear();
        }
    }
}
