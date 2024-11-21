namespace TBFramework.AI.BT
{
    public class BTReturnFailure : BTDecoration
    {
        public override E_BTNodeState Evaluate(BaseContext context)
        {
            switch (node.Evaluate(context))
            {
                case E_BTNodeState.Success:
                case E_BTNodeState.Failure:
                    return E_BTNodeState.Failure;
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
