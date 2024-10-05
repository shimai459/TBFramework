using System;
using System.Threading;
using System.Threading.Tasks;

namespace TBFramework.Timer
{
    public class TimerWithTask<T> : BaseTimer<T>
    {
        Task task;

        public TimerWithTask()
        {
            type = E_TimerType.Task;
        }

        public TimerWithTask(int uniqueKey, int intervalTime, Action<T> action, T param) : base(uniqueKey, intervalTime, action, param)
        {
            type = E_TimerType.Task;
        }

        public override void Start()
        {
            if (!_isRunning)
            {
                base.Start();
                task = Task.Run(StartTimer);
            }
        }

        private void StartTimer()
        {
            while (_isRunning)
            {
                Thread.Sleep(intervalTime);
                action?.Invoke(param);
            }
        }

        public override void Reset()
        {
            base.Reset();
            task = null;
        }
    }
}