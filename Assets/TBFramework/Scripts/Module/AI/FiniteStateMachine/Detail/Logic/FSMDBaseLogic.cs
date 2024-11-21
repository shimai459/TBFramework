using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{
    public abstract class FSMDBaseLogic : KeyBase
    {
        protected BaseContext context;

        public virtual void SetContext(BaseContext context)
        {
            this.context = context;
        }

        public BaseContext GetContext()
        {
            return context;
        }

        public void SetContextOneValue(string valueName, object value)
        {
            context.SetValue(valueName, value);
        }

        public FSMDBaseTransition transition;

        public void SetTransition(FSMDBaseTransition transition)
        {
            this.transition = transition;
        }

        public abstract void StartLogic();

        public abstract void StopLogic();

        public abstract void ToDefault();

        public override void Reset()
        {
            base.Reset();
            if (context != null)
            {
                FSMDManager.Instance.contexts.Destory(context.key);
            }
            context = default(BaseContext);
            if (transition != null)
            {
                FSMDManager.Instance.transitions.Destory(transition.key);
            }
            transition = default(FSMDBaseTransition);
        }

    }
}
