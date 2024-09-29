
namespace TBFramework.Input
{
    public class BaseParam<T> : I_BaseParam
    {
        public T param;

        public BaseParam(E_InputType type, T param) : base(type)
        {
            this.param = param;
        }
    }
}