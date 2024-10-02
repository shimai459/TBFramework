
using System;
using System.Collections.Generic;
using System.Reflection;
using TBFramework.Pool;

namespace TBFramework.Timer
{
    public class TimerManager : Singleton<TimerManager>
    {
        private Dictionary<int, I_BaseTimer> timers = new Dictionary<int, I_BaseTimer>();

        private List<int> uniqueKeys = new List<int>();

        public BaseTimer<T> CreateTimer<T>(E_TimerType type, int intervalTime, Action<T> action, T param)
        {
            BaseTimer<T> timer = null;
            int key = GetUnusedKey();
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
                PushToPool(timers[key]);
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
                PushToPool(timer);
            }
            timers.Clear();
            uniqueKeys.Clear();
        }

        private int GetUnusedKey()
        {
            uniqueKeys.Sort();
            int key = 1;
            foreach (int use in uniqueKeys)
            {
                if (use != key)
                {
                    break;
                }
                key++;
            }
            return key;
        }

        private void PushToPool(I_BaseTimer timer)
        {
            MethodInfo methodInfo = typeof(CPoolManager).GetMethod("Push");
            Type[] typeArguments = new Type[] { timer.GetType() };
            if (methodInfo != null && methodInfo.IsGenericMethodDefinition)
            {
                MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeArguments);
                object[] parameters = new object[] { timer, E_PoolMaxType.InPool, PoolSet.POOL_MAX_NUMBER };
                genericMethodInfo.Invoke(CPoolManager.Instance, parameters);
            }
        }
    }
}
