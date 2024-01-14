using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.AI.FSM
{
    /// <summary>
    /// AI状态基类
    /// </summary>
    public abstract class FSMState
    {
        /// <summary>
        /// 进入该AI状态要进行的逻辑操作
        /// </summary>
        public abstract void EnterState();

        /// <summary>
        /// 更新该AI状态要进行的逻辑操作
        /// </summary>
        public abstract void UpdateState();

        /// <summary>
        /// 退出该AI状态要进行的逻辑操作
        /// </summary>
        public abstract void ExitState();
    }
}
