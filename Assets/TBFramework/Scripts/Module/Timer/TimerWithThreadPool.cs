
using System;
using System.Threading;

namespace TBFramework.Timer
{
    public class TimerWithThreadPool<T> : BaseTimer<T>
    {
        public TimerWithThreadPool()
        {
            type = E_TimerType.ThreadPool;
        }

        public TimerWithThreadPool(int uniqueKey, int intervalTime, Action<T> action, T param) : base(uniqueKey, intervalTime, action, param)
        {
            type = E_TimerType.ThreadPool;
        }

        public override void Start()
        {
            base.Start();
            ThreadPool.QueueUserWorkItem(StartTimer);
        }

        private void StartTimer(object o)
        {
            while (_isRunning)
            {
                Thread.Sleep(intervalTime);
                action?.Invoke(param);
            }
        }
    }
}
