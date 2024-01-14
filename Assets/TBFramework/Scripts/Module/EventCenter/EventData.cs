using System;

namespace TBFramework.Event
{
    //没有参数的事件集合数据类
    public class EventData{
        public event Action actions;
        public EventData(Action action){
            actions+=action;
        }
        public void Invoke(){
            if(actions!=null){
                actions.Invoke();
            }
        }
    }
}