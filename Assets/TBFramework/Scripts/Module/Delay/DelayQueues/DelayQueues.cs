
using System;
using System.Collections.Generic;
using TBFramework.Pool;

namespace TBFramework.Delay.DelayQueues
{
    public class DelayQueues : BaseDelay
    {
        /// <summary>
        /// 根据延时时间排序的延时事件降序列表
        /// </summary>
        /// <typeparam name="BaseTimeEvent"></typeparam>
        /// <returns></returns>
        private List<BaseTimeEvent> eventList = new List<BaseTimeEvent>();

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public DelayQueues()
        {
        }

        /// <summary>
        /// 检测是否执行延时事件
        /// </summary>
        /// <param name="condition"></param>
        protected override void Check(Func<long, bool> condition)
        {
            //从列表最后开始取，只要符合条件，就执行延时事件
            List<int> timeEvents = new List<int>();
            for (int i = eventList.Count - 1; i >= 0; i--)
            {
                BaseTimeEvent te = eventList[i];
                if (condition(te.ExpiredTime))
                {
                    te.Invoke();
                    timeEvents.Add(te.UniqueKey);
                }
                else
                {
                    break;
                }
            }
            RemoveEvents(timeEvents);
        }

        /// <summary>
        /// 添加延时事件
        /// </summary>
        /// <param name="delayTime">延时时间</param>
        /// <param name="startTime">添加延时事件的时间</param>
        /// <param name="action">延时事件具体方法</param>
        /// <param name="param">延时事件参数</param>
        /// <param name="myStr">延时事件标识类型</param>
        /// <typeparam name="T">延时事件参数类型</typeparam>
        /// <returns></returns>
        public override TimeEvent<T> AddEvent<T>(int delayTime, long startTime, Action<T> action, T param, string myStr)
        {
            TimeEvent<T> timeEvent = base.AddEvent<T>(delayTime, startTime, action, param, myStr);
            eventList.Add(timeEvent);
            //添加事件后，对所有事件进行延时时间的降序排序
            ListSort();
            return timeEvent;
        }

        /// <summary>
        /// 根据唯一Key移除延时事件
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        public override void RemoveEvent(int uniqueKey)
        {

            if (eventDic.ContainsKey(uniqueKey))
            {
                BaseTimeEvent timeEvent = eventDic[uniqueKey];
                CPoolManager.Instance.Push(timeEvent);
                eventDic.Remove(uniqueKey);
                eventList.Remove(timeEvent);
                uniqueKeys.Remove(uniqueKey);
            }
        }

        /// <summary>
        /// 清理所有延时事件
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            eventList.Clear();
        }

        /// <summary>
        /// 添加某一事件的延时时间
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="delayTime">添加时间</param>
        public override void AddDelayTime(int uniqueKey, int delayTime)
        {
            base.AddDelayTime(uniqueKey, delayTime);
            //改变时间后，重新排序
            ListSort();
        }

        /// <summary>
        /// 改变某一事件的延时时间
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="delayTime">延时时间</param>
        public override void ChangeDelayTime(int uniqueKey, int delayTime)
        {
            base.ChangeDelayTime(uniqueKey, delayTime);
            //改变时间后，重新排序
            ListSort();
        }

        /// <summary>
        /// 延时时间排序
        /// </summary>
        private void ListSort()
        {
            eventList.Sort((x, y) => y.ExpiredTime.CompareTo(x.ExpiredTime));
        }
    }
}