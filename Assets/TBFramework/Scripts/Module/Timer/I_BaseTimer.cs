

using System;
using TBFramework.Pool;

namespace TBFramework.Timer
{
    public abstract class I_BaseTimer : CBase
    {
        /// <summary>
        /// 唯一Key
        /// </summary>
        protected int uniqueKey = -1;

        /// <summary>
        /// 获取唯一Key
        /// </summary>
        /// <value></value>
        public int UniqueKey
        {
            get => uniqueKey;
        }

        protected E_TimerType type;

        public E_TimerType Type
        {
            get => type;
        }

        protected int intervalTime; //间隔时间

        protected bool _isRunning;

        public I_BaseTimer() { }

        public I_BaseTimer(int uniqueKey, int intervalTime)
        {
            SetValue(uniqueKey, intervalTime);
        }

        public void SetValue(int uniqueKey, int intervalTime)
        {
            this.uniqueKey = uniqueKey;
            this.intervalTime = intervalTime;
            Start();
        }

        public void SetIntervalTime(int intervalTime)
        {
            this.intervalTime = intervalTime;
        }



        public virtual void Start()
        {
            _isRunning = true;
        }

        public virtual void Stop()
        {
            _isRunning = false;
        }

        public override void Reset()
        {
            this.uniqueKey = -1;
            intervalTime = 0;
            Stop();
        }
    }
}
