using System;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDActionWithInParam<T> : FSMDBaseAction
    {
        public T param;
        public event Action<T> action;

        public FSMDActionWithInParam() { }

        public void Set(T param, Action<T> action)
        {
            this.param = param;
            this.action = action;
        }


        public override void Invoke(BaseContext context)
        {
            action?.Invoke(param);
        }

        public override void Reset()
        {
            base.Reset();
            action = null;
            param = default(T);
        }
    }
}
