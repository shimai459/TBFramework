using System;
using System.Collections.Generic;
using TBFramework.Pool;
using TBFramework.Timer;
using TBFramework.Util;

namespace TBFramework.Delay.TimeWheel
{
    public class TimeWheelManager : BaseDelayManager<TimeWheelManager, TimeWheel>
    {
        public override TimeWheel CreateDelay(int intervalTime, E_TimerType timerType, Func<long, bool> condition)
        {
            TimeWheel delayQueues = CPoolManager.Instance.Pop<TimeWheel>();
            int uniqueKey = UniqueKeyUtil.GetUnusedKey(uniqueKeys);
            delayQueues.Init(uniqueKey, intervalTime, timerType, condition);
            return delayQueues;
        }

        public TimeWheel CreateTimeWheel(int intervalTime, int slotNum, long startTime, E_TimerType timerType, Func<long, bool> condition)
        {
            TimeWheel timeWheel = CreateDelay(intervalTime, timerType, condition);
            timeWheel.SetWheel(slotNum, intervalTime, startTime);
            delayDict.Add(timeWheel.UniqueKey, timeWheel);
            return timeWheel;
        }
    }
}