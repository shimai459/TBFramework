
using System;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDActionNoParam : FSMDBaseAction
    {
        public event Action action;

        public FSMDActionNoParam() { }

        public void Set(Action action)
        {
            this.action = action;
        }

        public override void Invoke(BaseContext context)
        {
            action?.Invoke();
        }

        public override void Reset()
        {
            base.Reset();
            action = null;
        }
    }
}
