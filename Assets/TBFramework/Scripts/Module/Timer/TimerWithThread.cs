
using System;
using System.Threading;


namespace TBFramework.Timer
{
    public class TimerWithThread<T> : BaseTimer<T>
    {
        Thread thread;

        public TimerWithThread()
        {
            type = E_TimerType.Thread;
        }

        public TimerWithThread(int uniqueKey, int intervalTime, Action<T> action, T param) : base(uniqueKey, intervalTime, action, param)
        {
            type = E_TimerType.Thread;
        }

        public override void Start()
        {
            base.Start();
            thread = new Thread(StartTimer);
            thread.IsBackground = true;
            thread.Start();
        }

        private void StartTimer()
        {
            while (_isRunning)
            {
                Thread.Sleep(intervalTime);
                action?.Invoke(param);
            }
        }

        public override void Stop()
        {
            base.Stop();
            try
            {
                thread.Abort();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex);
            }
        }

        public override void Reset()
        {
            base.Reset();
            thread = null;
        }
    }

}