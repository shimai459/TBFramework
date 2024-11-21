using System;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDTransitionWithInParam<V, T> : FSMDBaseTransition<T>
    {
        public V param;
        public event Func<V, T> func;


        public FSMDTransitionWithInParam() { }

        public void Set(V param, Func<V, T> func)
        {
            this.param = param;
            this.func = func;
        }


        public override T Change(BaseContext context)
        {
            if (func != null)
            {
                return func.Invoke(param);
            }
            return default(T);

        }

        public override void Reset()
        {
            base.Reset();
            param = default(V);
            func = null;
        }
    }
}
