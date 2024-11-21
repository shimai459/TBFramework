using System;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDAction<T> : FSMDBaseAction
    {
        public T param;
        public event Action<T> actionWithSelf;

        public event Action<BaseContext> actionWithOut;

        public FSMDAction() { }

        public void Set(T param, Action<T> action)
        {
            this.type = E_FSMDActionType.SelfParam;
            this.param = param;
            this.actionWithSelf = action;
        }

        public void Set(Action<BaseContext> action)
        {
            this.type = E_FSMDActionType.OutParam;
            this.actionWithOut = action;
        }

        public override void Invoke(BaseContext context)
        {
            switch (type)
            {
                case E_FSMDActionType.SelfParam:
                    actionWithSelf?.Invoke(param);
                    break;
                case E_FSMDActionType.OutParam:
                    if (context is T)
                    {
                        actionWithOut?.Invoke(context);
                    }
                    break;
            }
        }

        public override void Reset()
        {
            base.Reset();
            actionWithSelf = null;
            actionWithOut = null;
            param = default(T);
        }
    }
}
