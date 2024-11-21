

namespace TBFramework.AI
{
    public class ContextWithTParam<T> : BaseContext
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

        public override object GetValue(string valueName)
        {
            return param.GetType().GetProperty(valueName).GetValue(param);
        }

        public override V GetValue<V>(string valueName)
        {
            object value = GetValue(valueName);
            if (value is V v)
            {
                return v;
            }
            return default(V);
        }

        public override void SetValue(string valueName, object value)
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
