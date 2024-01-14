using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.AI.BT
{
    public class BTSelector : BTControl
    {
        public override E_BTNodeState Evaluate()
        {
            foreach(BTNode node in nodes){
                switch(node.Evaluate()){
                    case E_BTNodeState.Success:
                        return E_BTNodeState.Success;
                    case E_BTNodeState.Running:
                        return E_BTNodeState.Running;
                    default:
                        continue;
                }
            }
            return E_BTNodeState.Failure;
        }
    }
}
