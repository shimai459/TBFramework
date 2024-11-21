

namespace TBFramework.AI.BT
{
    public class BTCondition : BTNode
    {
        private event System.Func<BaseContext, E_BTNodeState> condition;

        private E_BTNodeState nullReturnState;

        public void Set(System.Func<BaseContext, E_BTNodeState> condition, E_BTNodeState nullReturnState)
        {
            this.condition = condition;
            this.nullReturnState = nullReturnState;
        }

        public override E_BTNodeState Evaluate(BaseContext context)
        {
            if (condition != null)
            {
                return condition.Invoke(context);
            }
            else
            {
                return nullReturnState;
            }

        }

        public void AddCondition(System.Func<BaseContext, E_BTNodeState> condition)
        {
            this.condition += condition;
        }

        public void RemoveCondition(System.Func<BaseContext, E_BTNodeState> condition)
        {
            this.condition -= condition;
        }
        public override void Reset()
        {
            condition = null;
        }
    }
}
