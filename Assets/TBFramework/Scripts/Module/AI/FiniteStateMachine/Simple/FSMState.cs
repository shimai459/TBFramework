
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Simple
{
    /// <summary>
    /// AI状态基类
    /// </summary>
    public abstract class FSMSState : CBase
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

        /// <summary>
        /// 检查是否切换其他状态
        /// </summary>
        public abstract void CheckChange();
    }
}
