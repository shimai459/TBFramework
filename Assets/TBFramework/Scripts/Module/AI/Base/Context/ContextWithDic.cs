using System.Collections.Generic;

namespace TBFramework.AI
{
    public class ContextWithDic : BaseContext
    {
        private Dictionary<string, object> dataDic = new Dictionary<string, object>();

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

        public void RemoveData(string key)
        {
            dataDic.Remove(key);
        }

        public override void Reset()
        {
            this.dataDic.Clear();
        }
    }
}
