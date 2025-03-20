using System;
using TBFramework.Pool;

namespace TBFramework.Load.LoadInfo
{
    public class LoadInfo : CBase
    {
        public string name;
        public BaseLoadData loadData;

        public LoadInfo() { }

        public LoadInfo(string name, BaseLoadData loadData)
        {
            this.Init(name, loadData);
        }

        public void Init(string name, BaseLoadData loadData)
        {
            this.name = name;
            this.loadData = loadData;
        }

        public override void Reset()
        {
            this.name = default;
            CPoolManager.Instance.Push(loadData);
            loadData = null;
        }
    }
}
