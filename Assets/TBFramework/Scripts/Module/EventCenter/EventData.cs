using System;
using TBFramework.Pool;

namespace TBFramework.Event
{
    //没有参数的事件集合数据类
    public class EventData : EventDataNoOutParamBase
    {
        public event Action actions;

        public EventData() { }
        public EventData(Action action)
        {
            this.Init(action);
        }

        public void Init(Action action)
        {
            actions += action;
        }
        public override void Invoke()
        {
            if (actions != null)
            {
                actions.Invoke();
            }
        }

        public override void Reset()
        {
            this.actions = null;
        }
    }
}