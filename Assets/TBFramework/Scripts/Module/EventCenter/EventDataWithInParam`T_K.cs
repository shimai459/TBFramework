using System;

namespace TBFramework.Event
{
    public class EventDataWithInParam<T, K> : EventDataWithOutParamBase<K>
    {
        public T param;
        public event Action<T, K> actions;

        public EventDataWithInParam() { }

        public EventDataWithInParam(Action<T, K> action, T param)
        {
            this.Init(action, param);
        }

        // 定义一个公共方法 Init，用于初始化或添加一个委托到 actions 链中
        public void Init(Action<T, K> action, T param)
        {
            this.param = param;
            this.actions += action;
        }

        public bool CheckActionSame(Action<T, K> action)
        {
            return this.actions == action;
        }

        public bool CheckSame(Action<T, K> action, T param)
        {
            return this.actions == action && this.param.Equals(param);
        }

        public override void Invoke(K info)
        {
            if (actions != null)
            {
                actions.Invoke(param, info);
            }
        }

        public override void Reset()
        {
            param = default(T);
            actions = null;
        }

    }
}