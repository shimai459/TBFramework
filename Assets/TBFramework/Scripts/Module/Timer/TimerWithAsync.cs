using System;
using System.Threading.Tasks;

namespace TBFramework.Timer
{
    public class TimerWithAsync<T> : BaseTimer<T>
    {
        public TimerWithAsync()
        {
            type = E_TimerType.Async;
        }
        public TimerWithAsync(int uniqueKey, int intervalTime, Action<T> action, T param) : base(uniqueKey, intervalTime, action, param)
        {
            type = E_TimerType.Async;
        }

        public override void Start()
        {
            if (!_isRunning)
            {
                base.Start();
                StartTimer();
            }

        }

        private async void StartTimer()
        {
            while (_isRunning)
            {
                await Task.Delay(intervalTime);
                action?.Invoke(param);
            }
        }
    }
}
