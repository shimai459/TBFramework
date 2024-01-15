using System;

namespace TBFramework.Delay.TimeWheel
{
    public class TimeEvent
    {
        /// <summary>
        /// 用于延时的事件
        /// </summary>
        private event Action<object> action = null;

        /// <summary>
        /// 事件参数
        /// </summary>
        private object param = null;

        /// <summary>
        /// 唯一Key
        /// </summary>
        private int uniqueKey = -1;

        /// <summary>
        /// 获取唯一Key
        /// </summary>
        /// <value></value>
        public int UniqueKey
        {
            get => uniqueKey;
        }

        /// <summary>
        /// 推迟执行的时间
        /// </summary>
        private int delayTime = 0;

        /// <summary>
        /// 提供外界获取延迟执行的时间
        /// </summary>
        /// <value></value>
        public int DelayTime
        {
            get => delayTime;
        }

        private string myStr = null;

        public string MyStr
        {
            get => myStr;
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="param">参数</param>
        /// <param name="uniqueKey">key</param>
        public TimeEvent(Action<object> action, object param, int delayTime, int uniqueKey)
        {
            this.action = action;
            this.param = param;
            this.delayTime = delayTime;
            this.uniqueKey = uniqueKey;
        }

        /// <summary>
        /// 执行
        /// </summary> 
        public void Invoke()
        {
            if (action != null)
            {
                action.Invoke(param);
            }
        }
    }
}