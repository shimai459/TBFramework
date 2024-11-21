using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.AI.BT
{
    public class BTSequence : BTControl
    {
        public override E_BTNodeState Evaluate(BaseContext context)
        {
            if (isRandom && index == 0)
            {
                Random();
            }
            if (index < 0 && index >= nodes.Count)
            {
                index = 0;
            }
            BTNode node = nodes[index];
            switch (node.Evaluate(context))
            {
                case E_BTNodeState.Success:
                    if (index >= nodes.Count - 1)
                    {
                        index = 0;
                        return E_BTNodeState.Success;
                    }
                    else
                    {
                        index++;
                        return E_BTNodeState.Running;
                    }
                case E_BTNodeState.Running:
                    if (this.memoryType == E_BTControlMemoryType.AllReStart)
                    {
                        index = 0;
                    }
                    return E_BTNodeState.Running;
                case E_BTNodeState.Failure:
                    if (this.memoryType != E_BTControlMemoryType.AllMemory)
                    {
                        index = 0;
                    }
                    return E_BTNodeState.Failure;
            }
            return E_BTNodeState.Failure;
        }

    }
}
