
using System;
using TBFramework.Pool;

namespace TBFramework.Timer
{

    public abstract class BaseTimer<T> : I_BaseTimer
    {
        protected Action<T> action;

        protected T param;

        public BaseTimer() { }

        public BaseTimer(int uniqueKey, int intervalTime, Action<T> action, T param) : base(uniqueKey, intervalTime)
        {
            this.action = action;
            this.param = param;
        }

        public void SetAction(Action<T> action)
        {
            this.action = action;
        }

        public void AddAction(Action<T> action)
        {
            this.action += action;
        }

        public void RemoveAction(Action<T> action)
        {
            this.action -= action;
        }

        public void SetParam(T param)
        {
            this.param = param;
        }

        public override void Reset()
        {
            action = null;
            param = default(T);
            base.Reset();
        }
    }
}
