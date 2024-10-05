
using System;
using System.Collections.Generic;
using TBFramework.Pool;

namespace TBFramework.Delay.TimeWheel
{
    public class SingleTimeWheel : CBase
    {

        /// <summary>
        /// 时间轮的槽数
        /// </summary>
        public int slotNum;

        /// <summary>
        /// 每个槽位的时间
        /// </summary> 
        public int slotInterval;

        /// <summary>
        /// 该时间轮的起始时间
        /// </summary>
        public long startTime;

        /// <summary>
        /// 当前时间轮的tick
        /// </summary>
        public int currentTick;

        /// <summary>
        /// 时间轮的槽
        /// </summary>
        public SingleTimeSlot[] slots;

        /// <summary>
        /// 当前时间轮的层级
        /// </summary>
        public int level;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public SingleTimeWheel()
        {
        }

        /// <summary>
        /// 时间轮的初始化函数
        /// </summary>
        /// <param name="slotNum">槽数</param>
        /// <param name="slotInterval">槽的间隔时间</param>
        /// <param name="startTime">轮的起始时间</param>
        /// <param name="level">轮的层级</param>
        /// <param name="isFirst">是不是层级为0的轮</param>
        public SingleTimeWheel(int slotNum, int slotInterval, long startTime, int level, bool isFirst)
        {
            SetWheel(slotNum, slotInterval, startTime, level, isFirst);
        }

        /// <summary>
        /// 时间轮的初始化函数
        /// </summary>
        /// <param name="slotNum">槽数</param>
        /// <param name="slotInterval">槽的间隔时间</param>
        /// <param name="startTime">轮的起始时间</param>
        /// <param name="level">轮的层级</param>
        /// <param name="isFrist">是不是层级为0的轮</param>
        public void SetWheel(int slotNum, int slotInterval, long startTime, int level, bool isFrist)
        {
            this.slotNum = slotNum;
            this.slotInterval = slotInterval;
            this.startTime = startTime;
            this.level = level;
            slots = new SingleTimeSlot[slotNum];
            currentTick = isFrist ? -1 : 0;
        }

        /// <summary>
        /// 将传入的事件列表添加到当前时间轮
        /// </summary>
        /// <param name="list">延时事件列表</param>
        /// <param name="startTime">时间轮的起始时间</param>
        /// <returns></returns>
        public List<BaseTimeEvent> EventListToNewWheel(List<BaseTimeEvent> list, long startTime)
        {
            currentTick = -1;
            this.startTime = startTime;
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    AddEvent(list[i]);
                }
            }
            return Tick();
        }

        /// <summary>
        /// 当前轮的指针偏转
        /// </summary>
        /// <returns></returns>
        public List<BaseTimeEvent> Tick()
        {
            //回收当前指针的槽
            if (currentTick >= 0 && currentTick < slots.Length && slots[currentTick] != null)
            {
                CPoolManager.Instance.Push<SingleTimeSlot>(slots[currentTick]);
                slots[currentTick] = null;
            }

            //指针转动
            currentTick++;

            //指针大于槽数时，返回null告诉外部要进行下一层级轮中的Tick
            if (currentTick >= slots.Length)
            {
                return null;
            }
            else
            {
                SingleTimeSlot slot = slots[currentTick];
                if (slot != null)
                {
                    //如果当前槽不为空：轮的层级不为0，则将事件列表返回外部形成新的上层轮；层级为0时，执行槽中的事件，返回空的事件列表。
                    if (level == 0)
                    {
                        slot.Invoke();
                    }
                    return slot.eventList;
                }
                return new List<BaseTimeEvent>();
            }
        }

        public int GetIndex(long time)
        {
            return (int)((time - startTime) / (slotInterval * TimeSpan.TicksPerMillisecond));
        }

        /// <summary>
        /// 添加延时事件
        /// </summary>
        /// <param name="baseTimeEvent">延时事件</param>
        public void AddEvent(BaseTimeEvent baseTimeEvent)
        {
            AddEvent(GetIndex(baseTimeEvent.ExpiredTime), baseTimeEvent);
        }

        /// <summary>
        /// 往指定槽中添加延时事件
        /// </summary>
        /// <param name="slotIndex">槽索引</param>
        /// <param name="baseTimeEvent">延时事件</param>
        public void AddEvent(int slotIndex, BaseTimeEvent baseTimeEvent)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length)
            {
                return;
            }
            if (slots[slotIndex] == null)
            {
                slots[slotIndex] = CPoolManager.Instance.Pop<SingleTimeSlot>();
            }
            slots[slotIndex].AddEvent(baseTimeEvent);
        }

        public void RemoveEvent(BaseTimeEvent baseTimeEvent)
        {
            RemoveEvent(GetIndex(baseTimeEvent.ExpiredTime), baseTimeEvent);
        }

        /// <summary>
        /// 清除单个槽中的延时事件
        /// </summary>
        /// <param name="slotIndex">槽索引</param>
        /// <param name="baseTimeEvent">延时事件</param>
        public void RemoveEvent(int slotIndex, BaseTimeEvent baseTimeEvent)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length)
            {
                return;
            }
            if (slots[slotIndex] != null)
            {
                slots[slotIndex].RemoveEvent(baseTimeEvent);
            }
        }

        /// <summary>
        /// 通过唯一Key清除单个槽中的延时事件
        /// </summary>
        /// <param name="slotIndex">槽索引</param>
        /// <param name="eventUnique">延时事件唯一Key</param>
        public void RemoveEvent(int slotIndex, int eventUnique)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length)
            {
                return;
            }
            if (slots[slotIndex] != null)
            {
                slots[slotIndex].RemoveEvent(eventUnique);
            }
        }

        /// <summary>
        /// 清除单个槽中所有相同标识类型的延时事件
        /// </summary>
        /// <param name="slotIndex">槽索引</param>
        /// <param name="myStr">延时事件标识类型</param>
        public void RemoveEvent(int slotIndex, string myStr)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length)
            {
                return;
            }
            if (slots[slotIndex] != null)
            {
                slots[slotIndex].RemoveEvent(myStr);
            }
        }

        /// <summary>
        /// 清除所有相同标识类型的延时事件
        /// </summary>
        /// <param name="myStr">延时事件标识类型</param>
        public void RemoveEvent(string myStr)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    slots[i].RemoveEvent(myStr);
                }
            }

        }

        /// <summary>
        /// 清理轮中的所有槽
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    CPoolManager.Instance.Push<SingleTimeSlot>(slots[i]);
                    slots[i] = null;
                }
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
            currentTick = -1;
            level = -1;
            Clear();
            slots = null;
        }
    }
}