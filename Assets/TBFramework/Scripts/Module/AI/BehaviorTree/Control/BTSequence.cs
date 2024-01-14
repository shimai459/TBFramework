using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.AI.BT
{
    public class BTSequence : BTControl
    {
        public override E_BTNodeState Evaluate()
        {
            bool anyChildRunning=false;
            foreach(BTNode node in nodes){
                switch(node.Evaluate()){
                    case E_BTNodeState.Failure:
                        return E_BTNodeState.Failure;
                    case E_BTNodeState.Running:
                        anyChildRunning=true;
                        break;
                    default:
                        continue;
                }
            }
            return anyChildRunning?E_BTNodeState.Running:E_BTNodeState.Success;
        }
    }
}
