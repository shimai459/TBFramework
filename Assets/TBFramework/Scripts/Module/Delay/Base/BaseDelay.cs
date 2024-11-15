using System;
using System.Collections.Generic;
using TBFramework.Pool;
using TBFramework.Timer;
using TBFramework.Util;

namespace TBFramework.Delay
{
    public abstract class BaseDelay : CBase
    {
        /// <summary>
        /// 唯一Key
        /// </summary>
        protected int uniqueKey;

        /// <summary>
        /// 获取唯一Key
        /// </summary>
        /// <value></value>
        public int UniqueKey
        {
            get => uniqueKey;
        }

        /// <summary>
        /// 已经使用过的唯一Key
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <returns></returns>
        protected List<int> uniqueKeys = new List<int>();

        /// <summary>
        /// 延时所使用的计时器
        /// </summary>
        protected I_BaseTimer timer;

        /// <summary>
        /// 所有的延时事件
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <typeparam name="BaseTimeEvent"></typeparam>
        /// <returns></returns>
        protected Dictionary<int, BaseTimeEvent> eventDic = new Dictionary<int, BaseTimeEvent>();

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public BaseDelay()
        {

        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="intervalTime">检测时间间隔</param>
        /// <param name="timerType">计时器类型</param>
        /// <param name="condition">检测延时事件执行的条件</param>
        public BaseDelay(int uniqueKey, int intervalTime, E_TimerType timerType, Func<long, bool> condition)
        {
            Init(uniqueKey, intervalTime, timerType, condition);
        }

        /// <summary>
        /// 提供缓存池使用的初始化方法
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="intervalTime">检测时间间隔</param>
        /// <param name="timerType">计时器类型</param>
        /// <param name="condition">检测延时事件执行的条件</param>
        public void Init(int uniqueKey, int intervalTime, E_TimerType timerType, Func<long, bool> condition)
        {
            this.uniqueKey = uniqueKey;
            this.timer = TimerManager.Instance.CreateTimer<Func<long, bool>>(timerType, intervalTime, Check, condition);
            uniqueKeys.Clear();
            eventDic.Clear();
        }

        /// <summary>
        /// 计时器每隔指定时间执行的检测方法
        /// </summary>
        /// <param name="condition">检测延时事件执行的条件</param>
        protected abstract void Check(Func<long, bool> condition);

        /// <summary>
        /// 开启计时器
        /// </summary>
        public void Start()
        {
            timer?.Start();
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        public void Stop()
        {
            timer?.Stop();
        }

        /// <summary>
        /// 改变计时器的间隔时间
        /// </summary>
        /// <param name="intervalTime">检测时间间隔</param>
        public void ChangeIntervalTime(int intervalTime)
        {
            timer?.SetIntervalTime(intervalTime);
        }

        /// <summary>
        /// 添加延时事件
        /// </summary>
        /// <param name="delayTime">延迟时间</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="action">延时事件</param>
        /// <param name="param">延时事件参数</param>
        /// <param name="myStr">延时事件类型</param>
        /// <typeparam name="T">延时事件参数类型</typeparam>
        /// <returns></returns>
        public virtual TimeEvent<T> AddEvent<T>(int delayTime, long startTime, Action<T> action, T param, string myStr)
        {
            int uniqueKey = UniqueKeyUtil.GetUnusedKey(uniqueKeys);
            TimeEvent<T> timeEvent = CPoolManager.Instance.Pop<TimeEvent<T>>();
            timeEvent.SetValue(uniqueKey, delayTime, startTime, action, param, myStr);
            this.eventDic.Add(uniqueKey, timeEvent);
            this.uniqueKeys.Add(uniqueKey);
            return timeEvent;
        }

        /// <summary>
        /// 通过唯一Key移除延时事件
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        public virtual void RemoveEvent(int uniqueKey)
        {
            if (eventDic.ContainsKey(uniqueKey))
            {
                CPoolManager.Instance.Push(eventDic[uniqueKey]);
                eventDic.Remove(uniqueKey);
                uniqueKeys.Remove(uniqueKey);
            }
        }

        /// <summary>
        /// 通过事件移除延时事件
        /// </summary>
        /// <param name="timeEvent">延时事件</param>
        public virtual void RemoveEvent(BaseTimeEvent timeEvent)
        {
            if (timeEvent != null)
            {
                CPoolManager.Instance.Push(timeEvent);
                if (eventDic.ContainsKey(timeEvent.UniqueKey) && eventDic[timeEvent.UniqueKey] == timeEvent)
                {
                    eventDic.Remove(timeEvent.UniqueKey);
                }
            }
        }

        /// <summary>
        /// 通过唯一Key组移除多个延时事件
        /// </summary>
        /// <param name="uniqueKeys">唯一Key组</param>
        public virtual void RemoveEvents(params int[] uniqueKeys)
        {
            if (uniqueKeys != null)
            {
                foreach (int uniqueKey in uniqueKeys)
                {
                    RemoveEvent(uniqueKey);
                }
            }
        }

        /// <summary>
        /// 通过事件移除多个延时事件
        /// </summary>
        /// <param name="timeEvents">延时事件组</param>
        public virtual void RemoveEvents(params BaseTimeEvent[] timeEvents)
        {
            if (timeEvents != null)
            {
                foreach (BaseTimeEvent timeEvent in timeEvents)
                {
                    RemoveEvent(timeEvent);
                }
            }
        }

        /// <summary>
        /// 通过用户输入延时事件类型，批量移除延时事件
        /// </summary>
        /// <param name="myStr">延时事件类型</param>
        public virtual void RemoveEvent(string myStr)
        {
            List<int> teKeys = new List<int>();
            foreach (BaseTimeEvent te in eventDic.Values)
            {
                if (te.MyStr == myStr)
                {
                    teKeys.Add(te.UniqueKey);
                }
            }
            foreach (int key in teKeys)
            {
                RemoveEvent(key);
            }
        }

        /// <summary>
        /// 清除所有延时事件
        /// </summary>
        public virtual void Clear()
        {
            foreach (BaseTimeEvent t in eventDic.Values)
            {
                CPoolManager.Instance.Push(t);
            }
            eventDic.Clear();
            uniqueKeys.Clear();
        }

        /// <summary>
        /// 添加某一延时事件的延时时间
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="delayTime">添加延时的时间</param>
        public virtual void AddDelayTime(int uniqueKey, int delayTime)
        {
            if (eventDic.ContainsKey(uniqueKey))
            {
                eventDic[uniqueKey].AddDelayTime(delayTime);
            }
        }

        /// <summary>
        /// 改变某一延时事件的延时时间 
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="delayTime">延时时间</param>
        public virtual void ChangeDelayTime(int uniqueKey, int delayTime)
        {
            if (eventDic.ContainsKey(uniqueKey))
            {
                eventDic[uniqueKey].ChangeDelayTime(delayTime);
            }
        }

        /// <summary>
        /// 添加某一Key的延时事件
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="action">延时事件</param>
        /// <typeparam name="T">事件参数类型</typeparam>
        public virtual void AddAction<T>(int uniqueKey, Action<T> action)
        {
            if (eventDic.ContainsKey(uniqueKey))
            {
                (eventDic[uniqueKey] as TimeEvent<T>).AddEvent(action);
            }
        }

        /// <summary>
        /// 移除某一Key的延时事件
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="action">延时事件</param>
        /// <typeparam name="T">事件参数类型</typeparam>
        public virtual void RemoveAction<T>(int uniqueKey, Action<T> action)
        {
            if (eventDic.ContainsKey(uniqueKey))
            {
                (eventDic[uniqueKey] as TimeEvent<T>).RemoveEvent(action);
            }
        }

        /// <summary>
        /// 改变某一Key的延时事件参数
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="value">延时事件参数</param>
        /// <typeparam name="T">事件参数类型</typeparam>
        public virtual void ChangeValue<T>(int uniqueKey, T value)
        {
            if (eventDic.ContainsKey(uniqueKey))
            {
                (eventDic[uniqueKey] as TimeEvent<T>).SetParam(value);
            }
        }

        /// <summary>
        /// 重置延时类
        /// </summary>
        public override void Reset()
        {
            uniqueKey = 0;
            if (timer != null)
            {
                TimerManager.Instance.RemoveTimer(timer);
            }
            Clear();

        }
    }
}