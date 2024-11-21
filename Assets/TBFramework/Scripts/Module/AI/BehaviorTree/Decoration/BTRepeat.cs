using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.AI.BT
{
    public class BTRepeat : BTDecoration
    {
        private int repeatCount = 0;

        public void SetRepeat(int repeatCount)
        {
            this.repeatCount = repeatCount;
        }

        public override E_BTNodeState Evaluate(BaseContext context)
        {
            int count = 0;
            while (count < repeatCount)
            {
                switch (node.Evaluate(context))
                {
                    case E_BTNodeState.Running:
                        return E_BTNodeState.Running;
                    case E_BTNodeState.Failure:
                        return E_BTNodeState.Failure;
                    case E_BTNodeState.Success:
                        count++;
                        break;
                }
            }
            return E_BTNodeState.Success;
        }

        public override void Reset()
        {
            base.Reset();
            this.repeatCount = 0;
        }
    }
}
