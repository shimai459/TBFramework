using System.Collections;
using System.Collections.Generic;
using TBFramework.Pool;
using UnityEngine;
namespace TBFramework.AI.BT
{
    public abstract class BTDecoration : BTNode
    {
        protected BTNode node;

        public void SetNode(BTNode node)
        {
            this.node = node;
        }

        public override void Reset()
        {
            BTManager.Instance.nodes.Destory(node);
        }
    }
}