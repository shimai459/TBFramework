using System;

namespace TBFramework.Event
{   
    //有一个参数的事件集合数据类
    public class EventData<T>:I_EventData{
        public event Action<T> actions;
        public EventData(Action<T> action){
            actions+=action;
        }
        public void Invoke(T info){
            if(actions!=null){
                actions.Invoke(info);
            }
        }
    }
}