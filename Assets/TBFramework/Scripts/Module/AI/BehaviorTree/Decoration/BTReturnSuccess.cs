namespace TBFramework.AI.BT
{
    public class BTReturnSuccess : BTDecoration
    {
        public override E_BTNodeState Evaluate(BaseContext context)
        {
            switch (node.Evaluate(context))
            {
                case E_BTNodeState.Success:
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
