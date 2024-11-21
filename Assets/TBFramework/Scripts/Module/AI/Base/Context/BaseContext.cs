
using TBFramework.Pool;

namespace TBFramework.AI
{
    public abstract class BaseContext : KeyBase
    {
        public abstract void SetValue(string valueName, object value);

        public abstract object GetValue(string valueName);

        public abstract T GetValue<T>(string valueName);
    }
}
