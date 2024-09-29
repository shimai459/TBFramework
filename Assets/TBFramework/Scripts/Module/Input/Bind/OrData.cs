
using System;
using TBFramework.Event;

namespace TBFramework.Input
{
    public class OrData : BindBaseData
    {
        private bool isRepeated;

        private int untriggerCount = 0;

        private bool isTrigger = false;
        public OrData(string inputEvent, bool canChange, bool isRepeated, params InputData[] datas) : base(inputEvent, canChange, datas)
        {
            this.isRepeated = isRepeated;
            this.inputType = E_InputType.Or;
        }

        public override void Check()
        {
            isTrigger = false;
            untriggerCount = 0;
            foreach (InputData data in this.datas)
            {
                InputData temp = data;
                data.IsTrigger((b) =>
                {
                    if (b && (!isTrigger || isRepeated))
                    {
                        isTrigger = true;
                        EventCenter.Instance.EventTrigger<I_BaseParam>(inputEvent, new BaseParam<I_BaseParam>(E_InputType.Or, temp.GetParam()));
                    }
                    else
                    {
                        untriggerCount++;
                        if (untriggerCount == datas.Count)
                        {
                            EventCenter.Instance.EventTrigger<I_BaseParam>(inputEvent + InputSet.NOT_TRIGGER_SUFFIX, new BaseParam<I_BaseParam>(E_InputType.Or, temp.GetParam()));
                        }
                    }
                });

            }
        }

        public override void IsTrigger(Action<bool> action)
        {
            bool trigger = false;
            foreach (InputData data in this.datas)
            {
                if (trigger)
                {
                    break;
                }
                data.IsTrigger(action);
                data.IsTrigger((b) =>
                {
                    if (b)
                    {
                        trigger = true;
                    }
                });
            }
        }

        public override I_BaseParam GetParam()
        {
            BaseParam<I_BaseParam> param = new BaseParam<I_BaseParam>(E_InputType.Or, null);
            bool trigger = false;
            foreach (InputData data in this.datas)
            {
                if (trigger)
                {
                    break;
                }
                InputData temp = data;
                data.IsTrigger((b) =>
                {
                    if (b)
                    {
                        trigger = true;
                        param.param = temp.GetParam();
                    }
                });
            }
            return param;
        }

        protected override bool Compare(InputData other)
        {
            if (other is OrData)
            {
                OrData otherW = other as OrData;
                return base.Compare(other) && isRepeated == otherW.isRepeated;
            }
            else
            {
                return base.Compare(other);
            }
        }

        public void ChangeData(InputData oldData, InputData newData)
        {
            if (oldData.canChange)
            {
                InputData remove = null;
                foreach (InputData data in datas)
                {
                    if (data == oldData)
                    {
                        remove = data;
                        break;
                    }
                }
                if (remove != null)
                {
                    this.datas.Remove(remove);
                    this.datas.Add(newData);
                }
            }
        }
    }
}