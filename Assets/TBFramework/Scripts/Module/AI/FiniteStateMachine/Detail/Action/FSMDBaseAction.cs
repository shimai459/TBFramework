
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{
    public abstract class FSMDBaseAction : FSMKeyBase
    {
        public E_FSMDActionType type;

        public abstract void Invoke<T>(FSMDContext<T> context);
    }
}
