using System;

namespace TBFramework.Delay
{
    public class TimeEvent<T> : BaseTimeEvent
    {
        /// <summary>
        /// 用于延时的事件
        /// </summary>
        private event Action<T> action = null;

        /// <summary>
        /// 事件参数
        /// </summary>
        private T param;

        /// <summary>
        /// 无参构造方法
        /// </summary>
        public TimeEvent()
        {

        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="param">参数</param>
        /// <param name="uniqueKey">key</param>
        public TimeEvent(int uniqueKey, int delayTime, long startTime, Action<T> action, T param, string myStr) : base(uniqueKey, delayTime, startTime, myStr)
        {
            this.action = action;
            this.param = param;
        }

        /// <summary>
        /// 给缓存池取出使用的初始化方法
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="delayTime">延时时间</param>
        /// <param name="startTime">添加延时事件的时间</param>
        /// <param name="action">延时事件的具体方法</param>
        /// <param name="param">延时事件参数</param>
        /// <param name="myStr">延时事件标识类型</param>
        public void SetValue(int uniqueKey, int delayTime, long startTime, Action<T> action, T param, string myStr)
        {
            this.action = action;
            this.param = param;
            this.delayTime = delayTime;
            this.startTime = startTime;
            this.uniqueKey = uniqueKey;
            this.myStr = myStr;
            isOver = false;
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="action">事件</param>
        public void AddEvent(Action<T> action)
        {
            this.action += action;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="action">事件</param>
        public void RemoveEvent(Action<T> action)
        {
            this.action -= action;
        }

        /// <summary>
        /// 设置事件
        /// </summary>
        /// <param name="action">事件</param>
        public void SetEvent(Action<T> action)
        {
            this.action = action;
        }

        /// <summary>
        /// 改变参数
        /// </summary>
        /// <param name="param">延时参数</param>
        public void SetParam(T param)
        {
            this.param = param;
        }

        /// <summary>
        /// 执行
        /// </summary> 
        public override void Invoke()
        {
            isOver = true;
            if (action != null)
            {
                action.Invoke(param);
            }
        }

        /// <summary>
        /// 还原回收
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            action = null;
            param = default(T);
        }
    }
}