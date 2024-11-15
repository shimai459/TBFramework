
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{
    public abstract class FSMDBaseAction : FSMDKeyBase
    {
        public E_FSMDActionType type;

        public abstract void Invoke<T>(FSMDContext<T> context);
    }
}
