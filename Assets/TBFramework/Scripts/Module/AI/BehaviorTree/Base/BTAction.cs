using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.AI.BT
{
    public class BTAction : BTNode
    {
        private System.Func<E_BTNodeState> action;

        public BTAction(System.Func<E_BTNodeState> action){
            this.action=action;
        }

        public override E_BTNodeState Evaluate()
        {
            return action.Invoke();
        }

        public void AddAction(System.Func<E_BTNodeState> action){
            this.action+=action;
        }

        public void RemoveAction(System.Func<E_BTNodeState> action){
            this.action-=action;
        }
    }
}
