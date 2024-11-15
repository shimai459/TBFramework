using System;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDTransition<V> : FSMDBaseTransition<V>
    {
        public event Func<V> func;

        public FSMDTransition() { }

        public void Set(Func<V> func)
        {
            this.type = E_FSMDTransitionType.NoParam;
            this.func = func;
        }

        public override V Change(FSMDBaseContext context)
        {
            if (func != null)
            {
                return func();
            }
            else
            {
                return default(V);
            }
        }

        public override void Reset()
        {
            base.Reset();
            func = null;
        }
    }
}
