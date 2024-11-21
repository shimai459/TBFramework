using System;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDTransitionWithContext<T> : FSMDBaseTransition<T>
    {

        public event Func<BaseContext, T> func;


        public void Set(Func<BaseContext, T> func)
        {
            this.func = func;
        }

        public override T Change(BaseContext context)
        {
            if (func != null)
            {
                return func.Invoke(context);
            }
            return default(T);

        }

        public override void Reset()
        {
            base.Reset();
            func = null;
        }
    }
}
