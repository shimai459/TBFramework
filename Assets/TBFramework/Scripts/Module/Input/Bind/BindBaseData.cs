
using System.Collections.Generic;

namespace TBFramework.Input
{
    public abstract class BindBaseData : InputData
    {
        protected List<InputData> datas;

        public BindBaseData(string inputEvent, bool canChange, params InputData[] datas) : base(inputEvent, canChange)
        {
            this.datas = new List<InputData>();
            AddData(datas);

        }

        public void AddData(params InputData[] datas)
        {
            if (datas == null)
            {
                return;
            }
            foreach (var data in datas)
            {
                if (!this.datas.Contains(data))
                {
                    this.datas.Add(data);
                }
            }

        }

        public void RemoveData(params InputData[] datas)
        {
            if (datas == null)
            {
                return;
            }
            foreach (InputData data in datas)
            {
                if (this.datas.Contains(data))
                {
                    this.datas.Remove(data);
                }
            }

        }

        public void Clear()
        {
            this.datas.Clear();
        }

        protected bool HaveInList(InputData data)
        {
            foreach (InputData data2 in datas)
            {

            }
            return false;
        }

        protected override bool Compare(InputData other)
        {
            if (other is BindBaseData)
            {
                BindBaseData otherW = other as BindBaseData;
                return base.Compare(other) && CompareList(this.datas, otherW.datas);
            }
            else
            {
                return base.Compare(other);
            }
        }

        private bool CompareList(List<InputData> left, List<InputData> right)
        {
            foreach (InputData data in left)
            {
                bool haveSame = false;
                foreach (InputData data2 in right)
                {
                    if (data == data2)
                    {
                        haveSame = true;
                    }
                }
                if (!haveSame)
                {
                    return false;
                }
            }
            return true;
        }
    }
}