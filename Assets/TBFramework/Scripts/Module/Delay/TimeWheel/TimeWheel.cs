

using System;
using System.Collections.Generic;
using TBFramework.Pool;
using TBFramework.Timer;

namespace TBFramework.Delay.TimeWheel
{
    public class TimeWheel : BaseDelay
    {
        /// <summary>
        /// 单个时间轮的槽数
        /// </summary>
        public int slotNum;

        /// <summary>
        /// 最低等级时间轮每个槽位的时间
        /// </summary> 
        public int slotInterval;

        /// <summary>
        /// 建立该时间轮的起始时间
        /// </summary>
        public long startTime;

        /// <summary>
        /// 总Tick数
        /// </summary>
        public int sumTick = 0;

        /// <summary>
        /// 所有层级的轮
        /// </summary>
        /// <typeparam name="SingleTimeWheel"></typeparam>
        /// <returns></returns>
        private List<SingleTimeWheel> wheels = new List<SingleTimeWheel>();

        /// <summary>
        /// 时间轮初始化
        /// </summary>
        /// <param name="slotNum">槽数</param>
        /// <param name="slotInterval">每槽的间隔时间</param>
        /// <param name="startTime">建立该时间轮的起始时间</param>
        public void SetWheel(int slotNum, int slotInterval, long startTime)
        {
            this.slotNum = slotNum;
            this.slotInterval = slotInterval;
            this.startTime = startTime;
        }

        /// <summary>
        /// 总时间轮进行一次Tick
        /// </summary>
        /// <param name="condition"></param>
        protected override void Check(Func<long, bool> condition)
        {
            sumTick++;
            //从总Tick数判断是否需要更高的轮，需要就创建
            GetMaxLevel(sumTick * slotInterval * TimeSpan.TicksPerMillisecond + startTime);
            //从最低等级开始进行Tick，直到找到可以执行的轮
            int i = 0;
            List<BaseTimeEvent> list = null;
            for (; i < this.wheels.Count; i++)
            {
                list = wheels[i].Tick();
                if (list != null)
                {
                    break;
                }
            }
            //将可以执行的轮的当前槽转变为下一层的轮，直到最低轮
            long start = wheels[i].startTime + wheels[i].currentTick * (wheels[i].slotInterval - 1) * TimeSpan.TicksPerMillisecond;
            for (i--; i >= 0; i--)
            {
                list = wheels[i].EventListToNewWheel(list, start);
            }
            //将已经执行过的延时事件从事件轮中清理出去
            RemoveEvents(list);
        }

        /// <summary>
        /// 添加延时事件
        /// </summary>
        /// <param name="delayTime">延时时间</param>
        /// <param name="startTime">创建延时的时间</param>
        /// <param name="action">延时事件具体方法</param>
        /// <param name="param">延时事件参数</param>
        /// <param name="myStr">延时事件标识类型</param>
        /// <typeparam name="T">延时事件参数类型</typeparam>
        /// <returns></returns>
        public override TimeEvent<T> AddEvent<T>(int delayTime, long startTime, Action<T> action, T param, string myStr)
        {
            TimeEvent<T> timeEvent = base.AddEvent<T>(delayTime, startTime, action, param, myStr);
            AddEventOnlyWheel(timeEvent);
            return timeEvent;
        }

        /// <summary>
        /// 在轮中添加延时事件
        /// </summary>
        /// <param name="timeEvent">延时事件</param>
        private void AddEventOnlyWheel(BaseTimeEvent timeEvent)
        {
            int level = GetCurrentLevel(timeEvent.ExpiredTime);
            if (level >= 0 && level < wheels.Count)
            {
                wheels[level].AddEvent(timeEvent);
            }
        }

        /// <summary>
        /// 通过唯一Key移除延时事件
        /// </summary>
        /// <param name="uniqueKey"></param>
        public override void RemoveEvent(int uniqueKey)
        {
            if (eventDic.ContainsKey(uniqueKey))
            {
                RemoveEventOnlyWheel(uniqueKey);
                CPoolManager.Instance.Push(eventDic[uniqueKey]);
                eventDic.Remove(uniqueKey);
                uniqueKeys.Remove(uniqueKey);
            }
        }

        /// <summary>
        /// 通过唯一Key在轮中移除延时事件
        /// </summary>
        /// <param name="uniqueKey"></param>
        private void RemoveEventOnlyWheel(int uniqueKey)
        {

            if (eventDic.ContainsKey(uniqueKey))
            {
                BaseTimeEvent timeEvent = eventDic[uniqueKey];
                int level = GetCurrentLevel(timeEvent.ExpiredTime);
                if (level >= 0 && level < wheels.Count)
                {
                    wheels[level].RemoveEvent(timeEvent);
                }
            }
        }


        /// <summary>
        /// 清理所有延时事件
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            foreach (SingleTimeWheel wheel in wheels)
            {
                CPoolManager.Instance.Push(wheel);
            }
            wheels.Clear();
        }

        /// <summary>
        /// 添加某一延时事件延迟时间
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="delayTime">添加时间</param>
        public override void AddDelayTime(int uniqueKey, int delayTime)
        {
            if (eventDic.ContainsKey(uniqueKey))
            {
                //先从轮中移除
                RemoveEventOnlyWheel(uniqueKey);
                eventDic[uniqueKey].AddDelayTime(delayTime);
                //改变延时时间后重新添加
                AddEventOnlyWheel(eventDic[uniqueKey]);
            }
        }

        /// <summary>
        /// 改变某一延时事件延迟时间
        /// </summary>
        /// <param name="uniqueKey">唯一Key</param>
        /// <param name="delayTime">延时时间</param>
        public override void ChangeDelayTime(int uniqueKey, int delayTime)
        {
            if (eventDic.ContainsKey(uniqueKey))
            {
                //先从轮中移除
                RemoveEventOnlyWheel(uniqueKey);
                eventDic[uniqueKey].ChangeDelayTime(delayTime);
                //改变延时时间后重新添加
                AddEventOnlyWheel(eventDic[uniqueKey]);
            }
        }

        /// <summary>
        /// 重置回归
        /// </summary>
        public override void Reset()
        {
            slotNum = 0;
            slotInterval = 0;
            startTime = 0;
            sumTick = 0;
            base.Reset();
        }

        /// <summary>
        /// 根据时间获得最高层级
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private int GetMaxLevel(long time)
        {
            int level = 0;
            while (time >= (slotInterval * (long)Math.Pow(slotNum, level + 1) * TimeSpan.TicksPerMillisecond) + startTime)
            {
                level++;
            }
            for (int i = this.wheels.Count; i <= level; i++)
            {
                SingleTimeWheel wheel = CPoolManager.Instance.Pop<SingleTimeWheel>();
                wheel.SetWheel(slotNum, slotInterval * (int)Math.Pow(slotNum, i), this.startTime, i, i == 0);
                wheels.Add(wheel);
            }
            return level;
        }

        /// <summary>
        /// 根据时间获得当前层级
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private int GetCurrentLevel(long time)
        {
            int level = GetMaxLevel(time);
            for (int j = level; j >= 0; j--)
            {
                SingleTimeWheel wheel = wheels[j];
                if (wheel.GetIndex(time) > wheel.currentTick)
                {
                    return j;
                }
            }
            return -1;
        }

    }
}