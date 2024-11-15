
using System;
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Simple
{
    /// <summary>
    /// AI状态基类
    /// </summary>
    public class FSMSState<T, V> : FSMSBaseState
    {
        public void Set(Action<V> enter, Action<V> update, Action<V> lateUpdate, Action<V> fixedUpdate, Action<V> exit, Func<V, T> change)
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
        public Action<V> enter;

        /// <summary>
        /// 更新该AI状态要进行的逻辑操作
        /// </summary>
        public Action<V> update;

        /// <summary>
        /// 晚更新该AI状态要进行的逻辑操作
        /// </summary>
        public Action<V> lateUpdate;

        /// <summary>
        /// 固定更新该AI状态要进行的逻辑操作
        /// </summary>
        public Action<V> fixedUpdate;

        /// <summary>
        /// 退出该AI状态要进行的逻辑操作
        /// </summary>
        public Action<V> exit;

        /// <summary>
        /// 检查是否切换其他状态
        /// </summary>
        public Func<V, T> change;

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
