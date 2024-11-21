
using TBFramework.Pool;

namespace TBFramework.AI.BT
{
    public abstract class BTNode : KeyBase
    {
        public abstract E_BTNodeState Evaluate(BaseContext context);
    }
}
