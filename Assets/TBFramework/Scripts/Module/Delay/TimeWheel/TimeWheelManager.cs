using System.Collections.Generic;

namespace TBFramework.Delay.TimeWheel
{
    public class TimeWheelManager : Singleton<TimeWheelManager>
    {
        private List<TimeWheel> wheelList=new List<TimeWheel>();
        public TimeWheel CreateTimeWheel(){
            return null;
        }

        public void DestoryTimeWheel(TimeWheel tw){
            wheelList.Remove(tw);
        }
    }
}