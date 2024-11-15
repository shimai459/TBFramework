
using System;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDAction : FSMDBaseAction
    {
        public event Action action;

        public FSMDAction() { }

        public void Set(Action action)
        {
            this.type = E_FSMDActionType.NoParam;
            this.action = action;
        }

        public override void Invoke<T>(FSMDContext<T> context)
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
