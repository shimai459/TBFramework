using System.Collections.Generic;
using TBFramework.Pool;

namespace TBFramework.Delay.TimeWheel
{
    public class SingleTimeSlot : CBase
    {
        /// <summary>
        /// 延时事件列表
        /// </summary>
        /// <typeparam name="BaseTimeEvent"></typeparam>
        /// <returns></returns>
        public List<BaseTimeEvent> eventList = new List<BaseTimeEvent>();


        public SingleTimeSlot() { }

        /// <summary>
        /// 添加延时事件
        /// </summary>
        /// <param name="timeEvent">延时事件</param>
        public void AddEvent(BaseTimeEvent timeEvent)
        {
            if (timeEvent != null)
            {
                eventList.Add(timeEvent);
            }
        }

        /// <summary>
        /// 移除延时事件
        /// </summary>
        /// <param name="timeEvent">延时事件</param>
        public void RemoveEvent(BaseTimeEvent timeEvent)
        {

            if (timeEvent != null && eventList.Contains(timeEvent))
            {
                eventList.Remove(timeEvent);
            }
        }

        /// <summary>
        /// 通过唯一Key移除延时事件
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        public void RemoveEvent(int uniqueKey)
        {
            BaseTimeEvent te = this.FindByKey(uniqueKey);
            RemoveEvent(te);
        }

        /// <summary>
        /// 通过事件标识类型移除延时事件
        /// </summary>
        /// <param name="myStr"></param>
        public void RemoveEvent(string myStr)
        {
            List<BaseTimeEvent> list = new List<BaseTimeEvent>();
            foreach (BaseTimeEvent timeEvent in eventList)
            {
                if (timeEvent.MyStr == myStr)
                {
                    list.Add(timeEvent);
                }
            }
            foreach (BaseTimeEvent timeEvent in list)
            {
                RemoveEvent(timeEvent);
            }
        }

        /// <summary>
        /// 清理所有延时事件
        /// </summary>
        public void Clear()
        {
            eventList.Clear();
        }

        /// <summary>
        /// 执行当前时间槽的所有事件
        /// </summary>
        public void Invoke()
        {
            foreach (BaseTimeEvent timeEvent in eventList)
            {
                if (timeEvent != null)
                {
                    timeEvent.Invoke();
                }
            }
        }

        /// <summary>
        /// 重置回归
        /// </summary>
        public override void Reset()
        {
            Clear();
        }

        /// <summary>
        /// 通过Key找到延时事件
        /// </summary>
        /// <param name="uniqueKey"></param>
        /// <returns></returns>
        private BaseTimeEvent FindByKey(int uniqueKey)
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i].UniqueKey == uniqueKey)
                {
                    return eventList[i];
                }
            }
            return null;
        }
    }
}