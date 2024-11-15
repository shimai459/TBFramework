using System.Collections.Generic;
using TBFramework.Pool;
using TBFramework.Util;

namespace TBFramework.AI
{
    public class BaseMgrObj<T> where T : KeyBase
    {
        private Dictionary<int, (T obj, int count)> objs = new Dictionary<int, (T obj, int count)>();

        private List<int> uniqueKeys = new List<int>();

        public void Create(T obj)
        {
            bool have = false;
            foreach (var item in objs)
            {
                if (item.Value.obj.Equals(obj))
                {
                    AddUse(item.Key);
                    have = true;
                    break;
                }
            }
            if (!have)
            {
                int key = UniqueKeyUtil.GetUnusedKey(uniqueKeys);
                obj.SetKey(key);
                uniqueKeys.Add(key);
                objs.Add(key, (obj, 0));
            }
        }

        public void AddUse(int key)
        {
            if (objs.ContainsKey(key))
            {
                objs[key] = (objs[key].obj, objs[key].count + 1);
            }
        }

        public void AddUse(T obj)
        {
            bool have = false;
            foreach (var item in objs)
            {
                if (item.Value.obj.Equals(obj))
                {
                    have = true;
                    AddUse(item.Key);
                    return;
                }
            }
            if (!have)
            {
                int key = UniqueKeyUtil.GetUnusedKey(uniqueKeys);
                obj.SetKey(key);
                uniqueKeys.Add(key);
                objs.Add(key, (obj, 1));
            }
        }

        public void SubUse(int key)
        {
            if (objs.ContainsKey(key))
            {
                objs[key] = (objs[key].obj, objs[key].count - 1);
                if (objs[key].count < 0)
                {
                    UnityEngine.Debug.LogError($"key:{key}的引用小于0");
                    objs[key] = (objs[key].obj, 0);
                }
            }
        }

        public void SubUse(T obj)
        {
            foreach (var item in objs)
            {
                if (item.Value.obj.Equals(obj))
                {
                    SubUse(item.Key);
                    return;
                }
            }
        }

        public T Get(int key)
        {
            if (objs.ContainsKey(key))
            {
                return objs[key].obj;
            }
            return default(T);
        }

        public void Destory(int key)
        {
            SubUse(key);
            if (objs.ContainsKey(key) && objs[key].count <= 0)
            {
                CPoolManager.Instance.Push(objs[key].obj);
                objs.Remove(key);
                uniqueKeys.Remove(key);
            }
        }

        public void Destory(T obj)
        {
            bool have = false;
            foreach (var item in objs)
            {
                if (item.Value.obj.Equals(obj))
                {
                    have = true;
                    SubUse(item.Key);
                    if (objs.ContainsKey(item.Key) && objs[item.Key].count <= 0)
                    {
                        CPoolManager.Instance.Push(obj);
                        objs.Remove(item.Key);
                        uniqueKeys.Remove(item.Key);
                    }
                    return;
                }
            }
            if (!have)
            {
                CPoolManager.Instance.Push(obj);
            }
        }

        public int Count()
        {
            return objs.Count;
        }
    }
}
