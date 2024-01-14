using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.AI.BT
{
    public class BTRandomSelector : BTSelector
    {
        private System.Random random=new System.Random();
        public override E_BTNodeState Evaluate()
        {
            List<BTNode> shuffledNodes=new List<BTNode>(nodes);
            BTNode temp;
            for(int i=shuffledNodes.Count-1;i>0;i--){
                int k=random.Next(i);
                temp=shuffledNodes[k];
                shuffledNodes[k]=shuffledNodes[i];
                shuffledNodes[i]=temp;
            }
            foreach(BTNode node in shuffledNodes){
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
