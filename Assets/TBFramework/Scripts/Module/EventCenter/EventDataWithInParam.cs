using System;

namespace TBFramework.Event
{
    public class EventDataWithInParam<T> : EventDataNoOutParamBase
    {
        public T param;
        public event Action<T> actions;

        public EventDataWithInParam() { }

        public EventDataWithInParam(Action<T> action, T param)
        {
            this.Init(action, param);
        }

        public void Init(Action<T> action, T param)
        {
            this.actions += action;
            this.param = param;
        }

        public bool CheckActionSame(Action<T> action)
        {
            return this.actions == action;
        }

        public bool CheckSame(Action<T> action, T param)
        {
            return this.actions == action && this.param.Equals(param);
        }

        public override void Invoke()
        {
            if (actions != null)
            {
                actions.Invoke(param);
            }
        }

        public override void Reset()
        {
            param = default(T);
            actions = null;
        }

    }
}