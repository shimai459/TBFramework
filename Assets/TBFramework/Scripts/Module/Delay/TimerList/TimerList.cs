
using System;
using System.Collections.Generic;
using TBFramework.Timer;

namespace TBFramework.Delay.TimerList
{
    public class TimerList : BaseDelay
    {

        public TimerList() { }

        public TimerList(int uniqueKey, int intervalTime, E_TimerType timerType, Func<long, bool> condition) : base(uniqueKey, intervalTime, timerType, condition) { }

        protected override void Check(Func<long, bool> condition)
        {
            List<int> timeEvents = new List<int>();
            foreach (BaseTimeEvent te in eventDic.Values)
            {
                if (condition(te.ExpiredTime))
                {
                    te.Invoke();
                    timeEvents.Add(te.UniqueKey);
                }
            }
            foreach (int te in timeEvents)
            {
                RemoveEvent(te);
            }
        }

    }
}
