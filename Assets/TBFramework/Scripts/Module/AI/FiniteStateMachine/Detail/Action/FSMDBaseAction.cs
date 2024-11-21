
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{
    public abstract class FSMDBaseAction : KeyBase
    {
        public abstract void Invoke(BaseContext context);
    }
}
