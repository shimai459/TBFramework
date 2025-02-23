using System;
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Simple
{
    public abstract class FSMSBaseLogic : KeyBase
    {
        protected BaseContext context;

        public virtual void SetContext(BaseContext context)
        {
            if (this.context != null)
                FSMSManager.Instance.contexts.Destory(this.context.key);
            if (context != null)
                FSMSManager.Instance.contexts.AddUse(context.key);
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


        public abstract void StartLogic();

        public abstract void StopLogic();

        public abstract void ToDefault();

        public override void Reset()
        {
            base.Reset();
            if (context != null)
            {
                FSMSManager.Instance.contexts.Destory(context.key);
            }
            context = default(BaseContext);
        }
    }
}
