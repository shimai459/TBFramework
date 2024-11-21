using System;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDActionWithContext : FSMDBaseAction
    {
        public event Action<BaseContext> action;


        public void Set(Action<BaseContext> action)
        {
            this.action = action;
        }


        public override void Invoke(BaseContext context)
        {
            action?.Invoke(context);
        }

        public override void Reset()
        {
            base.Reset();
            action = null;
        }
    }
}