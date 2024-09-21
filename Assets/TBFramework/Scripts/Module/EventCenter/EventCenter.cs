using System;
using System.Collections.Generic;

namespace TBFramework.Event
{
    /// <summary>
    /// 事件中心控制器
    /// </summary>
    public class EventCenter : Singleton<EventCenter>
    {
        //用于一种事件(无参)的行为存储
        private Dictionary<string, EventData> eventDictNoArgu = new Dictionary<string, EventData>();
        //用于一种事件(有一个参数)的行为存储
        private Dictionary<Type, Dictionary<string, I_EventData>> eventDictOneArgu = new Dictionary<Type, Dictionary<string, I_EventData>>();
        /// <summary>
        /// 提供外部调用,添加无参函数
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="action">要添加的行为</param>
        public void AddEventListener(string eventName, Action action)
        {
            if (eventDictNoArgu.ContainsKey(eventName))
            {
                eventDictNoArgu[eventName].actions += action;
            }
            else
            {
                eventDictNoArgu.Add(eventName, new EventData(action));
            }
        }
        /// <summary>
        /// 提供外部调用,添加有一个参数的函数
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="action">要添加的行为</param>
        /// <typeparam name="T">参数的类型</typeparam>
        public void AddEventListener<T>(string eventName, Action<T> action)
        {
            if (eventDictOneArgu.ContainsKey(typeof(T)))
            {
                if (eventDictOneArgu[typeof(T)].ContainsKey(eventName))
                {
                    (eventDictOneArgu[typeof(T)][eventName] as EventData<T>).actions += action;
                }
                else
                {
                    eventDictOneArgu[typeof(T)].Add(eventName, new EventData<T>(action));
                }
            }
            else
            {
                eventDictOneArgu.Add(typeof(T), new Dictionary<string, I_EventData> { { eventName, new EventData<T>(action) } });
            }
        }
        /// <summary>
        /// 用于外部调用,移除无参函数
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="action">要移除的行为</param>
        public void RemoveEventListener(string eventName, Action action)
        {
            if (eventDictNoArgu.ContainsKey(eventName))
            {
                eventDictNoArgu[eventName].actions -= action;
            }
        }

        /// <summary>
        /// 用于外部调用,移除有一个参数的函数
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="action">要移除的行为</param>
        /// <typeparam name="T">参数的类型</typeparam>
        public void RemoveEventListener<T>(string eventName, Action<T> action)
        {
            if (eventDictOneArgu.ContainsKey(typeof(T)) && eventDictOneArgu[typeof(T)].ContainsKey(eventName))
            {
                (eventDictOneArgu[typeof(T)][eventName] as EventData<T>).actions -= action;
            }
        }

        /// <summary>
        /// 用于外部调用,触发无参事件执行
        /// </summary>
        /// <param name="eventName">事件名</param>
        public void EventTrigger(string eventName)
        {
            if (eventDictNoArgu.ContainsKey(eventName))
            {
                eventDictNoArgu[eventName].Invoke();
            }
        }

        /// <summary>
        /// 用于外部调用,触发有一个参数的事件执行
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="info">要传入的参数</param>
        /// <typeparam name="T">参数类型</typeparam>
        public void EventTrigger<T>(string eventName, T info)
        {
            if (eventDictOneArgu.ContainsKey(typeof(T)) && eventDictOneArgu[typeof(T)].ContainsKey(eventName))
            {
                (eventDictOneArgu[typeof(T)][eventName] as EventData<T>).Invoke(info);
            }
        }

        /// <summary>
        /// 清理没有参数的某个具体监听
        /// </summary>
        /// <param name="eventName">监听名</param>
        public void Clear(string eventName)
        {
            if (eventDictNoArgu.ContainsKey(eventName))
            {
                eventDictNoArgu.Remove(eventName);
            }
        }

        /// <summary>
        /// 清理有参数的某个具体监听
        /// </summary>
        /// <param name="eventName">监听名</param>
        /// <typeparam name="T">参数类型</typeparam>
        public void Clear<T>(string eventName)
        {
            if (eventDictOneArgu.ContainsKey(typeof(T)) && eventDictOneArgu[typeof(T)].ContainsKey(eventName))
            {
                eventDictOneArgu[typeof(T)].Remove(eventName);
            }
        }

        /// <summary>
        /// 清理有参数的某个参数类型的所有监听
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        public void Clear<T>()
        {
            if (eventDictOneArgu.ContainsKey(typeof(T)))
            {
                eventDictOneArgu.Remove(typeof(T));
            }
        }

        /// <summary>
        /// 清理某个监听
        /// </summary>
        /// <param name="eventName">监听名</param>
        public void ClearOneEvent(string eventName){
            if (eventDictNoArgu.ContainsKey(eventName))
            {
                eventDictNoArgu.Remove(eventName);
            }
            foreach (Dictionary<string, I_EventData> v in eventDictOneArgu.Values){
                if (v.ContainsKey(eventName)){
                    v.Remove(eventName);
                }
            }
        }

        /// <summary>
        /// 清空所有事件
        /// 一般用于切换场景
        /// </summary>
        public void ClearAll()
        {
            eventDictNoArgu.Clear();
            eventDictOneArgu.Clear();
        }
    }
}