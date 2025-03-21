
namespace TBFramework.AI.BT
{
    public class BTInvert : BTDecoration
    {
        public override E_BTNodeState Evaluate(BaseContext context)
        {
            switch (node.Evaluate(context))
            {
                case E_BTNodeState.Success:
                    return E_BTNodeState.Failure;
                case E_BTNodeState.Failure:
                    return E_BTNodeState.Success;
                default:
                    return E_BTNodeState.Running;
            }
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}
