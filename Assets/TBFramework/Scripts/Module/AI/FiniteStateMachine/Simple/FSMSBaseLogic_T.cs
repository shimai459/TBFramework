using System;

namespace TBFramework.AI.FSM.Simple
{
    public abstract class FSMSBaseLogic<T> : FSMSBaseLogic
    {
        public Func<BaseContext, T> func;

        public void SetFunc(Func<BaseContext, T> func)
        {
            this.func = func;
        }

        public override void Reset()
        {
            base.Reset();
            func = null;
        }
    }
}
