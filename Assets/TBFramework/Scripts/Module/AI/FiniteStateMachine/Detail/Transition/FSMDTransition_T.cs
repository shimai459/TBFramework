using System;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDTransition<T, V> : FSMDBaseTransition<V>
    {
        public T param;
        public event Func<T, V> funcWithIn;

        public event Func<BaseContext, V> funcWithOut;

        public FSMDTransition() { }

        public void Set(T param, Func<T, V> func)
        {
            this.type = E_FSMDTransitionType.SelfParam;
            this.param = param;
            this.funcWithIn = func;
        }

        public void Set(Func<BaseContext, V> func)
        {
            this.type = E_FSMDTransitionType.OutParam;
            this.funcWithOut = func;
        }

        public override V Change(BaseContext context)
        {
            switch (type)
            {
                case E_FSMDTransitionType.SelfParam:
                    if (funcWithIn != null)
                    {
                        return funcWithIn.Invoke(param);
                    }
                    break;
                case E_FSMDTransitionType.OutParam:
                    if (funcWithOut != null)
                    {
                        return funcWithOut.Invoke(context);
                    }
                    break;
            }
            return default(V);

        }

        public override void Reset()
        {
            base.Reset();
            param = default(T);
            funcWithIn = null;
            funcWithOut = null;
        }
    }
}
