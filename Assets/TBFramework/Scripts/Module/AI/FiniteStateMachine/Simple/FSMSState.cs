
using System;
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Simple
{
    /// <summary>
    /// AI状态基类
    /// </summary>
    public class FSMSState<T> : FSMSBaseState
    {
        public void Set(Action<BaseContext> enter, Action<BaseContext> update, Action<BaseContext> lateUpdate, Action<BaseContext> fixedUpdate, Action<BaseContext> exit, Func<BaseContext, T> change)
        {
            this.enter = enter;
            this.update = update;
            this.lateUpdate = lateUpdate;
            this.fixedUpdate = fixedUpdate;
            this.exit = exit;
            this.change = change;
        }

        /// <summary>
        /// 进入该AI状态要进行的逻辑操作
        /// </summary>
        public Action<BaseContext> enter;

        /// <summary>
        /// 更新该AI状态要进行的逻辑操作
        /// </summary>
        public Action<BaseContext> update;

        /// <summary>
        /// 晚更新该AI状态要进行的逻辑操作
        /// </summary>
        public Action<BaseContext> lateUpdate;

        /// <summary>
        /// 固定更新该AI状态要进行的逻辑操作
        /// </summary>
        public Action<BaseContext> fixedUpdate;

        /// <summary>
        /// 退出该AI状态要进行的逻辑操作
        /// </summary>
        public Action<BaseContext> exit;

        /// <summary>
        /// 检查是否切换其他状态
        /// </summary>
        public Func<BaseContext, T> change;

        public override void Reset()
        {
            base.Reset();
            enter = null;
            update = null;
            lateUpdate = null;
            fixedUpdate = null;
            exit = null;
            change = null;
        }
    }
}
