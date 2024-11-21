

namespace TBFramework.AI.BT
{
    public class BTAction : BTNode
    {
        private event System.Func<BaseContext, E_BTNodeState> action;

        private E_BTNodeState nullReturnState;

        public void Set(System.Func<BaseContext, E_BTNodeState> action, E_BTNodeState nullReturnState)
        {
            this.action = action;
            this.nullReturnState = nullReturnState;
        }

        public override E_BTNodeState Evaluate(BaseContext context)
        {
            if (action != null)
            {
                return action.Invoke(context);
            }
            else
            {
                return nullReturnState;
            }

        }

        public void AddAction(System.Func<BaseContext, E_BTNodeState> action)
        {
            this.action += action;
        }

        public void RemoveAction(System.Func<BaseContext, E_BTNodeState> action)
        {
            this.action -= action;
        }

        public override void Reset()
        {
            action = null;
        }
    }
}
