using System;

namespace TBFramework.AI.FSM.Simple
{
    public abstract class FSMSBaseLogic : KeyBase
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
