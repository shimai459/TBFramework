using System;
using System.Collections.Generic;
using System.Diagnostics;
using TBFramework.Pool;

namespace TBFramework.Event
{
    /// <summary>
    /// 事件中心控制器
    /// </summary>
    public class EventCenter : Singleton<EventCenter>
    {
        private Dictionary<string, List<EventDataNoOutParamBase>> eventDictNoOutParam = new Dictionary<string, List<EventDataNoOutParamBase>>();

        //用于一种事件(有一个参数)的行为存储
        private Dictionary<Type, Dictionary<string, List<EventBase>>> eventDictWithOutParam = new Dictionary<Type, Dictionary<string, List<EventBase>>>();

        /// <summary>
        /// 提供外部调用,添加无参函数
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="action">要添加的行为</param>
        public void AddEventListener(string eventName, Action action)
        {
            if (eventDictNoOutParam.ContainsKey(eventName))
            {
                (eventDictNoOutParam[eventName][0] as EventData).actions += action;
            }
            else
            {
                EventData eventData = CPoolManager.Instance.Pop<EventData>();
                eventData.Init(action);
                eventDictNoOutParam.Add(eventName, new List<EventDataNoOutParamBase> { eventData });
            }
        }

        public void AddEventListeners(params (string eventName, Action action)[] events)
        {
            foreach (var item in events)
            {
                AddEventListener(item.eventName, item.action);
            }
        }

        public void AddEventListener<T>(string eventName, Action<T> action, T param)
        {
            if (!eventDictNoOutParam.ContainsKey(eventName))
            {
                EventData eventData = CPoolManager.Instance.Pop<EventData>();
                eventDictNoOutParam.Add(eventName, new List<EventDataNoOutParamBase> { eventData });
            }
            EventDataWithInParam<T> eventDataWithInParam = CPoolManager.Instance.Pop<EventDataWithInParam<T>>();
            eventDataWithInParam.Init(action, param);
            eventDictNoOutParam[eventName].Add(eventDataWithInParam);
        }

        public void AddEventListeners<T>(params (string eventName, Action<T> action, T param)[] events)
        {
            foreach (var item in events)
            {
                AddEventListener(item.eventName, item.action, item.param);
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
            if (eventDictWithOutParam.ContainsKey(typeof(T)))
            {
                if (eventDictWithOutParam[typeof(T)].ContainsKey(eventName))
                {
                    (eventDictWithOutParam[typeof(T)][eventName][0] as EventData<T>).actions += action;
                }
                else
                {
                    EventData<T> eventData = CPoolManager.Instance.Pop<EventData<T>>();
                    eventData.Init(action);
                    eventDictWithOutParam[typeof(T)].Add(eventName, new List<EventBase> { eventData });
                }
            }
            else
            {
                EventData<T> eventData = CPoolManager.Instance.Pop<EventData<T>>();
                eventData.Init(action);
                eventDictWithOutParam.Add(typeof(T), new Dictionary<string, List<EventBase>> { { eventName, new List<EventBase>() { eventData } } });
            }
        }

        public void AddEventListeners<T>(params (string eventName, Action<T> action)[] events)
        {
            foreach (var item in events)
            {
                AddEventListener(item.eventName, item.action);
            }
        }

        public void AddEventListener<T, K>(string eventName, Action<T, K> action, T param)
        {
            if (eventDictWithOutParam.ContainsKey(typeof(K)))
            {
                if (!eventDictWithOutParam[typeof(K)].ContainsKey(eventName))
                {
                    EventData<K> eventData = CPoolManager.Instance.Pop<EventData<K>>();
                    eventDictWithOutParam[typeof(K)].Add(eventName, new List<EventBase> { eventData });
                }
            }
            else
            {
                EventData<K> eventData = CPoolManager.Instance.Pop<EventData<K>>();
                eventDictWithOutParam.Add(typeof(K), new Dictionary<string, List<EventBase>> { { eventName, new List<EventBase>() { eventData } } });
            }
            EventDataWithInParam<T, K> eventDataWithInParam = CPoolManager.Instance.Pop<EventDataWithInParam<T, K>>();
            eventDataWithInParam.Init(action, param);
            eventDictWithOutParam[typeof(K)][eventName].Add(eventDataWithInParam);
        }

        public void AddEventListeners<T, K>(params (string eventName, Action<T, K> action, T param)[] events)
        {
            foreach (var item in events)
            {
                AddEventListener(item.eventName, item.action, item.param);
            }
        }



        /// <summary>
        /// 用于外部调用,移除无参函数
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="action">要移除的行为</param>
        public void RemoveEventListener(string eventName, Action action)
        {
            if (eventDictNoOutParam.ContainsKey(eventName))
            {
                (eventDictNoOutParam[eventName][0] as EventData).actions -= action;
            }
        }

        public void RemoveEventListeners(params (string eventName, Action action)[] events)
        {
            foreach (var item in events)
            {
                RemoveEventListener(item.eventName, item.action);
            }
        }

        public void RemoveEventListener<T>(string eventName, Action<T> action, T param)
        {
            if (eventDictNoOutParam.ContainsKey(eventName) && eventDictNoOutParam[eventName].Count > 1)
            {
                List<EventDataNoOutParamBase> events = eventDictNoOutParam[eventName];
                List<EventDataNoOutParamBase> remove = new List<EventDataNoOutParamBase>();
                for (int i = 1; i < events.Count; i++)
                {
                    if ((events[i] as EventDataWithInParam<T>).CheckSame(action, param))
                    {
                        remove.Add(events[i]);
                    }
                }
                for (int i = 0; i < remove.Count; i++)
                {
                    CPoolManager.Instance.Push(remove[i]);
                    events.Remove(remove[i]);
                }
            }
        }

        public void RemoveEventListeners<T>(params (string eventName, Action<T> action, T param)[] events)
        {
            for (int i = 0; i < events.Length; i++)
            {
                RemoveEventListener<T>(events[i].eventName, events[i].action, events[i].param);
            }
        }

        public void RemoveEventListener<T>(string eventName, Action<T> action, bool isIn = true)
        {
            if (isIn)
            {
                if (eventDictNoOutParam.ContainsKey(eventName) && eventDictNoOutParam[eventName].Count > 1)
                {
                    List<EventDataNoOutParamBase> events = eventDictNoOutParam[eventName];
                    List<EventDataNoOutParamBase> remove = new List<EventDataNoOutParamBase>();
                    for (int i = 1; i < events.Count; i++)
                    {
                        if ((events[i] as EventDataWithInParam<T>).CheckActionSame(action))
                        {
                            remove.Add(events[i]);
                        }
                    }
                    for (int i = 0; i < remove.Count; i++)
                    {
                        CPoolManager.Instance.Push(remove[i]);
                        events.Remove(remove[i]);
                    }
                }
            }
            else
            {
                if (eventDictWithOutParam.ContainsKey(typeof(T)) && eventDictWithOutParam[typeof(T)].ContainsKey(eventName))
                {
                    (eventDictWithOutParam[typeof(T)][eventName][0] as EventData<T>).actions -= action;
                }
            }
        }

        public void RemoveEventListeners<T>(params (string eventName, Action<T> action, bool isIn)[] events)
        {
            for (int i = 0; i < events.Length; i++)
            {
                RemoveEventListener<T>(events[i].eventName, events[i].action, events[i].isIn);
            }
        }

        /// <summary>
        /// 用于外部调用,移除有一个参数的函数
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="action">要移除的行为</param>
        /// <typeparam name="T">参数的类型</typeparam>
        public void RemoveEventListener<T, K>(string eventName, Action<T, K> action)
        {
            if (eventDictWithOutParam.ContainsKey(typeof(K)) && eventDictWithOutParam[typeof(K)].ContainsKey(eventName) && eventDictWithOutParam[typeof(K)][eventName].Count > 1)
            {
                List<EventBase> events = eventDictWithOutParam[typeof(K)][eventName];
                List<EventBase> remove = new List<EventBase>();
                for (int i = 1; i < events.Count; i++)
                {
                    if ((events[i] as EventDataWithInParam<T, K>).CheckActionSame(action))
                    {
                        remove.Add(events[i]);
                    }
                }
                for (int i = 0; i < remove.Count; i++)
                {
                    CPoolManager.Instance.Push(remove[i]);
                    events.Remove(remove[i]);
                }
            }
        }

        public void RemoveEventListeners<T, K>(params (string eventName, Action<T, K> action)[] events)
        {
            for (int i = 0; i < events.Length; i++)
            {
                RemoveEventListener<T, K>(events[i].eventName, events[i].action);
            }
        }

        public void RemoveEventListener<T, K>(string eventName, Action<T, K> action, T param)
        {
            if (eventDictWithOutParam.ContainsKey(typeof(K)) && eventDictWithOutParam[typeof(K)].ContainsKey(eventName) && eventDictWithOutParam[typeof(K)][eventName].Count > 1)
            {
                List<EventBase> events = eventDictWithOutParam[typeof(K)][eventName];
                List<EventBase> remove = new List<EventBase>();
                for (int i = 1; i < events.Count; i++)
                {
                    if ((events[i] as EventDataWithInParam<T, K>).CheckSame(action, param))
                    {
                        remove.Add(events[i]);
                    }
                }
                for (int i = 0; i < remove.Count; i++)
                {
                    CPoolManager.Instance.Push(remove[i]);
                    events.Remove(remove[i]);
                }
            }
        }

        public void RemoveEventListeners<T, K>(params (string eventName, Action<T, K> action, T param)[] events)
        {
            for (int i = 0; i < events.Length; i++)
            {
                RemoveEventListener<T, K>(events[i].eventName, events[i].action, events[i].param);
            }
        }


        /// <summary>
        /// 用于外部调用,触发无参事件执行
        /// </summary>
        /// <param name="eventName">事件名</param>
        public void EventTrigger(string eventName)
        {
            if (eventDictNoOutParam.ContainsKey(eventName))
            {
                List<EventDataNoOutParamBase> events = eventDictNoOutParam[eventName];
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].Invoke();
                }
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
            if (eventDictWithOutParam.ContainsKey(typeof(T)) && eventDictWithOutParam[typeof(T)].ContainsKey(eventName))
            {
                List<EventBase> events = eventDictWithOutParam[typeof(T)][eventName];
                for (int i = 0; i < events.Count; i++)
                {
                    (events[i] as EventDataWithOutParamBase<T>).Invoke(info);
                }

            }
        }

        /// <summary>
        /// 清理没有参数的某个具体监听
        /// </summary>
        /// <param name="eventName">监听名</param>
        public void Clear(string eventName)
        {
            if (eventDictNoOutParam.ContainsKey(eventName))
            {
                for (int i = 0; i < eventDictNoOutParam[eventName].Count; i++)
                {
                    CPoolManager.Instance.Push(eventDictNoOutParam[eventName][i]);
                }
                eventDictNoOutParam.Remove(eventName);
            }
        }

        /// <summary>
        /// 清理有参数的某个具体监听
        /// </summary>
        /// <param name="eventName">监听名</param>
        /// <typeparam name="T">参数类型</typeparam>
        public void Clear<T>(string eventName)
        {
            if (eventDictWithOutParam.ContainsKey(typeof(T)) && eventDictWithOutParam[typeof(T)].ContainsKey(eventName))
            {
                for (int i = 0; i < eventDictWithOutParam[typeof(T)][eventName].Count; i++)
                {
                    CPoolManager.Instance.Push(eventDictWithOutParam[typeof(T)][eventName][i]);
                }
                eventDictWithOutParam[typeof(T)].Remove(eventName);
            }
        }

        public void Clear(Type type, string eventName)
        {
            if (eventDictWithOutParam.ContainsKey(type) && eventDictWithOutParam[type].ContainsKey(eventName))
            {
                for (int i = 0; i < eventDictWithOutParam[type][eventName].Count; i++)
                {
                    CPoolManager.Instance.Push(eventDictWithOutParam[type][eventName][i]);
                }
                eventDictWithOutParam[type].Remove(eventName);
            }
        }
        /// <summary>
        /// 清理有参数的某个参数类型的所有监听
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        public void Clear<T>()
        {
            if (eventDictWithOutParam.ContainsKey(typeof(T)))
            {
                foreach (List<EventBase> v in eventDictWithOutParam[typeof(T)].Values)
                {
                    for (int i = 0; i < v.Count; i++)
                    {
                        CPoolManager.Instance.Push(v[i]);
                    }
                }
                eventDictWithOutParam.Remove(typeof(T));
            }
        }

        public void Clear(Type type)
        {
            if (eventDictWithOutParam.ContainsKey(type))
            {
                foreach (List<EventBase> v in eventDictWithOutParam[type].Values)
                {
                    for (int i = 0; i < v.Count; i++)
                    {
                        CPoolManager.Instance.Push(v[i]);
                    }
                }
                eventDictWithOutParam.Remove(type);
            }
        }

        /// <summary>
        /// 清理某个监听
        /// </summary>
        /// <param name="eventName">监听名</param>
        public void ClearOneEvent(string eventName)
        {
            this.Clear(eventName);
            List<Type> types = new List<Type>();
            foreach (KeyValuePair<Type, Dictionary<string, List<EventBase>>> v in eventDictWithOutParam)
            {
                if (v.Value.ContainsKey(eventName))
                {
                    types.Add(v.Key);
                }
            }
            for (int i = 0; i < types.Count; i++)
            {
                this.Clear(types[i], eventName);
            }
        }

        public void ClearNoOutParam()
        {
            foreach (List<EventDataNoOutParamBase> evens in eventDictNoOutParam.Values)
            {
                for (int i = 0; i < evens.Count; i++)
                {
                    CPoolManager.Instance.Push(evens[i]);
                }
            }
            eventDictNoOutParam.Clear();
        }

        public void ClearWithOutParam()
        {
            foreach (Dictionary<string, List<EventBase>> evensDic in eventDictWithOutParam.Values)
            {
                foreach (List<EventBase> evens in evensDic.Values)
                {
                    for (int i = 0; i < evens.Count; i++)
                    {
                        CPoolManager.Instance.Push(evens[i]);
                    }
                }
            }
            eventDictWithOutParam.Clear();
        }
        /// <summary>
        /// 清空所有事件
        /// 一般用于切换场景
        /// </summary>
        public void ClearAll()
        {
            this.ClearNoOutParam();
            this.ClearWithOutParam();
        }
    }
}