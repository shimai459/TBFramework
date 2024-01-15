using System.Collections.Generic;

namespace TBFramework.Delay.TimeWheel
{
    public class TimeSlot
    {
        private List<TimeEvent> eventList = new List<TimeEvent>();

        private int level = -1;

        public void Add(TimeEvent timeEvent)
        {
            eventList.Add(timeEvent);
        }

        public void Remove(TimeEvent timeEvent)
        {
            eventList.Remove(timeEvent);
        }

        public void Remove(int uniqueKey)
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i].UniqueKey == uniqueKey)
                {
                    eventList.Remove(eventList[i]);
                    break;
                }
            }
        }

        public void Clear()
        {
            eventList.Clear();
        }

        public TimeWheel ToWheel()
        {
            //TODO 实现从高阶时间槽转换为低阶时间轮的算法
            return null;
        }

        public void Invoke()
        {
            eventList.ForEach((TimeEvent timeEvent) =>
            {
                timeEvent.Invoke();
            });
        }
    }
}