using System;
using TBFramework.Pool;

namespace TBFramework.Delay
{
    public abstract class BaseTimeEvent : CBase
    {
        /// <summary>
        /// 唯一Key
        /// </summary>
        protected int uniqueKey = -1;

        /// <summary>
        /// 获取唯一Key
        /// </summary>
        /// <value></value>
        public int UniqueKey
        {
            get => uniqueKey;
        }

        /// <summary>
        /// 推迟执行的时间（ms）
        /// </summary>
        protected int delayTime = 0;

        /// <summary>
        /// 提供外界获取延迟执行的时间（ms）
        /// </summary>
        /// <value></value>
        public int DelayTime
        {
            get => delayTime;
        }

        /// <summary>
        /// 用于标记的统一标志
        /// </summary> <summary>
        protected string myStr = null;

        /// <summary>
        /// 提供外部获取标识
        /// </summary>
        /// <value></value>
        public string MyStr
        {
            get => myStr;
        }

        /// <summary>
        /// 添加延时事件的时间
        /// </summary>
        protected long startTime;

        /// <summary>
        /// 提供外部获取添加延时事件的时间
        /// </summary>
        /// <value></value>
        public long StartTime
        {
            get => startTime;
        }

        /// <summary>
        /// 是否已经执行过
        /// </summary>
        protected bool isOver;

        /// <summary>
        /// 提供外部获取是否已经执行过
        /// </summary>
        /// <value></value>
        public bool IsOver
        {
            get => isOver;
        }

        /// <summary>
        /// 事件过期时间
        /// </summary>
        /// <value></value>
        public long ExpiredTime
        {
            get => startTime + delayTime * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public BaseTimeEvent()
        {

        }

        /// <summary>
        /// 延时事件构造函数
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="delayTime">延时时间</param>
        /// <param name="startTime">添加延时事件的时间</param>
        /// <param name="myStr">标识类型</param>
        public BaseTimeEvent(int uniqueKey, int delayTime, long startTime, string myStr)
        {
            this.delayTime = delayTime;
            this.uniqueKey = uniqueKey;
            this.myStr = myStr;
            this.startTime = startTime;
            isOver = false;
        }

        /// <summary>
        /// 增加延时时间
        /// </summary>
        /// <param name="add">延时时间</param>
        public void AddDelayTime(int add)
        {
            this.delayTime += add;
        }

        /// <summary>
        /// 改变延时时间
        /// </summary>
        /// <param name="delayTime">延时时间</param>
        public void ChangeDelayTime(int delayTime)
        {
            this.delayTime = delayTime;
        }

        /// <summary>
        /// 执行延时事件
        /// </summary>
        public abstract void Invoke();

        /// <summary>
        /// 还原回收
        /// </summary>
        public override void Reset()
        {
            uniqueKey = -1;
            delayTime = 0;
            startTime = 0;
            myStr = null;
            isOver = true;
        }
    }
}
