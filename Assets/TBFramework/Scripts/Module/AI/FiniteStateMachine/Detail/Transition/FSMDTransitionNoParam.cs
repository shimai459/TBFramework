using System;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDTransitionNoParam<T> : FSMDBaseTransition<T>
    {
        public event Func<T> func;

        public void Set(Func<T> func)
        {
            this.func = func;
        }

        public override T Change(BaseContext context)
        {
            if (func != null)
            {
                return func();
            }
            else
            {
                return default(T);
            }
        }

        public override void Reset()
        {
            base.Reset();
            func = null;
        }
    }
}
