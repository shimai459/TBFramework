using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TBFramework.AI.BT
{
    public abstract class BTNode
    {
        //该节点的准入条件
        public BTPrecondition precondition;
        public abstract E_BTNodeState Evaluate();
    }
}
