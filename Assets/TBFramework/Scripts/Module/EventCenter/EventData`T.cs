using System;

namespace TBFramework.Event
{
    //有一个参数的事件集合数据类
    public class EventData<T> : EventDataWithOutParamBase<T>
    {
        public event Action<T> actions;
        public EventData() { }
        public EventData(Action<T> action)
        {
            this.Init(action);
        }

        public void Init(Action<T> action)
        {
            actions += action;
        }
        public override void Invoke(T info)
        {
            if (actions != null)
            {
                actions.Invoke(info);
            }
        }

        public override void Reset()
        {
            actions = null;
        }
    }
}