
using System;

namespace TBFramework.Input
{
    public class OrderData : BindBaseData
    {
        public OrderData(string inputEvent, bool canChange, params InputData[] datas) : base(inputEvent, canChange, datas)
        {
            this.inputType = E_InputType.Order;
        }

        public override I_BaseParam GetParam()
        {
            bool isTrigger = true;
            BaseParam<I_BaseParam> param = new BaseParam<I_BaseParam>(E_InputType.Order, null);
            foreach (InputData data in datas)
            {
                InputData temp = data;
                data.IsTrigger((b) =>
                {
                    if (b)
                    {
                        param.param = temp.GetParam();
                    }
                    else
                    {
                        isTrigger = false;
                    }
                });
                if (!isTrigger)
                {
                    break;
                }
            }
            return param;
        }

        public override void IsTrigger(Action<bool> action)
        {
            bool isTrigger = true;
            int triggerCount = 0;
            foreach (InputData data in datas)
            {
                data.IsTrigger((b) =>
                {
                    if (b)
                    {
                        triggerCount++;
                    }
                    else
                    {
                        isTrigger = false;
                    }
                });
                if (!isTrigger)
                {
                    break;
                }
            }
            action?.Invoke(triggerCount == datas.Count);
        }
    }
}