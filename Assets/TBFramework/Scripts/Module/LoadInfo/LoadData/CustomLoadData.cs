using System;

namespace TBFramework.LoadInfo
{
    public class CustomLoadData<T> : BaseLoadData where T : UnityEngine.Object
    {
        public Action<bool, Action<T>> create;
        public CustomLoadData(Action<bool, Action<T>> create)
        {
            this.create = create;
        }
    }
}
