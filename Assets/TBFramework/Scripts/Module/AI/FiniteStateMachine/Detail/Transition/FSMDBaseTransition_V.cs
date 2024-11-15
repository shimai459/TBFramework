

using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{

    public abstract class FSMDBaseTransition<V> : FSMDBaseTransition
    {
        public abstract V Change(FSMDBaseContext context);
    }
}
