

using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{

    public abstract class FSMDBaseTransition<T> : FSMDBaseTransition
    {
        public abstract T Change(BaseContext context);
    }
}
