
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDContext<T> : FSMDBaseContext
    {
        private T param;

        public void Set(T param)
        {
            this.param = param;
        }

        public T Get()
        {
            return param;
        }

        public void SetValue(string valueName, object value)
        {
            param.GetType().GetProperty(valueName).SetValue(param, value);
        }

        public override void Reset()
        {
            base.Reset();
            param = default(T);
        }
    }
}
