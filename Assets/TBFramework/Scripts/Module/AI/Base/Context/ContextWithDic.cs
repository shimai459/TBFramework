using System.Collections.Generic;

namespace TBFramework.AI
{
    public class ContextWithDic : BaseContext
    {
        private Dictionary<string, object> dataDic = new Dictionary<string, object>();

        public int Count => dataDic.Count;

        public override object GetValue(string key)
        {
            if (dataDic.ContainsKey(key))
            {
                return dataDic[key];
            }
            return null;
        }

        public override T GetValue<T>(string key)
        {
            if (dataDic.ContainsKey(key) && dataDic[key] is T value)
            {
                return value;
            }
            return default(T);
        }

        public override void SetValue(string key, object value)
        {
            if (dataDic.ContainsKey(key))
            {
                dataDic[key] = value;
            }
        }

        public void SetValue<T>(string key, T value)
        {
            if (dataDic.ContainsKey(key))
            {
                dataDic[key] = value;
            }
        }

        public void SetValues(params (string key, object value)[] values)
        {
            foreach ((string key, object value) value in values)
            {
                SetValue(value.key, value.value);
            }
        }

        public bool ContainsKey(string key)
        {
            return dataDic.ContainsKey(key);
        }

        public void AddValue(string key, object value)
        {
            if (!dataDic.ContainsKey(key))
            {
                dataDic.Add(key, value);
            }
            else
            {
                dataDic[key] = value;
            }
        }

        public void AddValues(params (string key, object value)[] values)
        {
            foreach ((string key, object value) value in values)
            {
                AddValue(value.key, value.value);
            }
        }

        public void RemoveValue(string key)
        {
            dataDic.Remove(key);
        }

        public void RemoveValues(params string[] keys)
        {
            foreach (string key in keys)
            {
                RemoveValue(key);
            }
        }

        public override void Reset()
        {
            this.dataDic.Clear();
        }

        public bool Compare(ContextWithDic other)
        {
            if (this == other)
            {
                return true;
            }
            if (this.Count == other.Count)
            {
                foreach (var item in dataDic)
                {
                    if (!other.ContainsKey(item.Key) || !item.Value.Equals(other.GetValue(item.Key)))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
