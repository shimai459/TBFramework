namespace TBFramework.Event
{
    public abstract class EventDataWithOutParamBase<T> : EventBase
    {
        public abstract void Invoke(T info);
    }
}