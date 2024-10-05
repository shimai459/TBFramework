using System;
using System.Collections.Generic;
using TBFramework.Pool;
using TBFramework.Timer;
using TBFramework.Util;

namespace TBFramework.Delay
{
    public abstract class BaseDelayManager<T, DT> : Singleton<T> where T : BaseDelayManager<T, DT>, new() where DT : BaseDelay, new()
    {
        /// <summary>
        /// 延时
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <typeparam name="DT"></typeparam>
        /// <returns></returns>
        protected Dictionary<int, DT> delayDict = new Dictionary<int, DT>();

        /// <summary>
        /// 延时处理器唯一Key数组
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <returns></returns>
        protected List<int> uniqueKeys = new List<int>();

        /// <summary>
        /// 创建一个新的延时处理器
        /// </summary>
        /// <param name="intervalTime">检测间隔时间</param>
        /// <param name="timerType">计时器类型</param>
        /// <param name="condition">检测通过条件</param>
        /// <returns></returns>
        public virtual DT CreateDelay(int intervalTime, E_TimerType timerType, Func<long, bool> condition)
        {
            DT delayQueues = CPoolManager.Instance.Pop<DT>();
            int uniqueKey = UniqueKeyUtil.GetUnusedKey(uniqueKeys);
            delayQueues.Init(uniqueKey, intervalTime, timerType, condition);
            delayDict.Add(uniqueKey, delayQueues);
            return delayQueues;
        }

        /// <summary>
        /// 通过唯一Key移除延时处理器
        /// </summary>
        /// <param name="uniqueKey">延时处理器唯一Key</param>
        public void RemoveDelayQueues(int uniqueKey)
        {
            if (delayDict.ContainsKey(uniqueKey))
            {
                CPoolManager.Instance.Push<DT>(delayDict[uniqueKey]);
                delayDict.Remove(uniqueKey);
            }
        }

        /// <summary>
        /// 移除延时处理器
        /// </summary>
        /// <param name="delayQueues">延时处理器</param>
        public void RemoveDelayQueues(DT delayQueues)
        {
            if (delayQueues != null)
            {
                CPoolManager.Instance.Push(delayQueues);
                if (delayDict.ContainsKey(delayQueues.UniqueKey))
                {
                    delayDict.Remove(delayQueues.UniqueKey);
                }
            }
        }

        /// <summary>
        /// 清除所有延时处理器
        /// </summary>
        public void Clear()
        {
            foreach (var delayQueue in delayDict.Values)
            {
                CPoolManager.Instance.Push(delayQueue);
            }
            delayDict.Clear();
            uniqueKeys.Clear();
        }

        /// <summary>
        /// 为一个延时处理器添加延时事件
        /// </summary>
        /// <param name="delayUniqueKey">延时处理器唯一Key</param>
        /// <param name="delayTime">延时时间</param>
        /// <param name="startTime">添加延时事件的时间</param>
        /// <param name="action">延时事件具体方法</param>
        /// <param name="param">延时事件参数</param>
        /// <param name="myStr">延时事件标识类型</param>
        /// <typeparam name="VT">延时事件参数类型</typeparam>
        /// <returns></returns>
        public TimeEvent<VT> AddEvent<VT>(int delayUniqueKey, int delayTime, long startTime, Action<VT> action, VT param, string myStr)
        {
            if (delayDict.ContainsKey(delayUniqueKey))
            {
                return delayDict[delayUniqueKey].AddEvent(delayTime, startTime, action, param, myStr);
            }
            return null;
        }

        /// <summary>
        /// 通过唯一Key移除某一延时处理器中的延时事件
        /// </summary>
        /// <param name="delayUniqueKey">延时处理器唯一Key</param>
        /// <param name="eventUniqueKey">延时事件唯一Key</param>
        public void RemoveEvent(int delayUniqueKey, int eventUniqueKey)
        {
            if (delayDict.ContainsKey(delayUniqueKey))
            {
                delayDict[delayUniqueKey].RemoveEvent(eventUniqueKey);
            }
        }

        /// <summary>
        /// 移除某一延时处理器中的延时事件
        /// </summary>
        /// <param name="delayUniqueKey">延时处理器唯一Key</param>
        /// <param name="timeEvent">延时事件</param>
        public void RemoveEvent(int delayUniqueKey, BaseTimeEvent timeEvent)
        {
            if (delayDict.ContainsKey(delayUniqueKey))
            {
                delayDict[delayUniqueKey].RemoveEvent(timeEvent);
            }
        }

        /// <summary>
        /// 移除某一延时处理器中的所有同标识的延时事件
        /// </summary>
        /// <param name="delayUniqueKey">延时处理器唯一Key</param>
        /// <param name="myStr">延时事件标识类型</param>
        public void RemoveEvent(int delayUniqueKey, string myStr)
        {
            if (delayDict.ContainsKey(delayUniqueKey))
            {
                delayDict[delayUniqueKey].RemoveEvent(myStr);
            }
        }

        /// <summary>
        /// 移除所有同标识的延时事件
        /// </summary>
        /// <param name="myStr">延时事件标识类型</param>
        public void RemoveEvent(string myStr)
        {
            foreach (DT delay in delayDict.Values)
            {
                delay.RemoveEvent(myStr);
            }
        }

        /// <summary>
        /// 清空某一延时处理器中的延时事件
        /// </summary>
        /// <param name="uniqueKey"></param>
        public void ClearOneDelayQueues(int uniqueKey)
        {
            if (delayDict.ContainsKey(uniqueKey))
            {
                delayDict[uniqueKey].Clear();
            }
        }

        /// <summary>
        /// 添加某一延时事件的延时时间
        /// </summary>
        /// /// <param name="delayUniqueKey">延时队列唯一Key</param>
        /// <param name="eventUniqueKey">事件唯一Key</param>
        /// <param name="delayTime">添加延时的时间</param>
        public void AddDelayTime(int delayUniqueKey, int eventUniqueKey, int delayTime)
        {
            if (delayDict.ContainsKey(delayUniqueKey))
            {
                delayDict[delayUniqueKey].AddDelayTime(eventUniqueKey, delayTime);
            }
        }

        /// <summary>
        /// 改变某一延时事件的延时时间 
        /// </summary>
        /// /// <param name="delayUniqueKey">延时队列唯一Key</param>
        /// <param name="eventUniqueKey">事件唯一Key</param>
        /// <param name="delayTime">延时时间</param>
        public void ChangeDelayTime(int delayUniqueKey, int eventUniqueKey, int delayTime)
        {
            if (delayDict.ContainsKey(delayUniqueKey))
            {
                delayDict[delayUniqueKey].ChangeDelayTime(eventUniqueKey, delayTime);
            }
        }

        /// <summary>
        /// 添加某一Key的延时事件
        /// </summary>
        /// /// <param name="delayUniqueKey">延时队列唯一Key</param>
        /// <param name="eventUniqueKey">事件唯一Key</param>
        /// <param name="action">延时事件</param>
        /// <typeparam name="T">事件参数类型</typeparam>
        public void AddAction<VT>(int delayUniqueKey, int eventUniqueKey, Action<VT> action)
        {
            if (delayDict.ContainsKey(delayUniqueKey))
            {
                delayDict[delayUniqueKey].AddAction(eventUniqueKey, action);
            }
        }

        /// <summary>
        /// 移除某一Key的延时事件
        /// </summary>
        /// /// <param name="delayUniqueKey">延时队列唯一Key</param>
        /// <param name="eventUniqueKey">事件唯一Key</param>
        /// <param name="action">延时事件</param>
        /// <typeparam name="T">事件参数类型</typeparam>
        public void RemoveAction<VT>(int delayUniqueKey, int eventUniqueKey, Action<VT> action)
        {
            if (delayDict.ContainsKey(delayUniqueKey))
            {
                delayDict[delayUniqueKey].RemoveAction(eventUniqueKey, action);
            }
        }

        /// <summary>
        /// 改变某一Key的延时事件参数
        /// </summary>
        /// /// <param name="delayUniqueKey">延时队列唯一Key</param>
        /// <param name="eventUniqueKey">事件唯一Key</param>
        /// <param name="value">延时事件参数</param>
        /// <typeparam name="T">事件参数类型</typeparam>
        public virtual void ChangeValue<VT>(int delayUniqueKey, int eventUniqueKey, VT value)
        {
            if (delayDict.ContainsKey(delayUniqueKey))
            {
                delayDict[delayUniqueKey].ChangeValue(eventUniqueKey, value);
            }
        }
    }
}