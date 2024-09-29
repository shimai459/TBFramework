
using System;
using System.Collections.Generic;
using TBFramework.Event;

namespace TBFramework.Input
{
    public class AxisData : InputData
    {
        private AxisSingleData axis;

        private Func<AxisSingleData, bool> condition;
        public AxisData(string inputEvent, bool canChange, AxisSingleData datas, Func<AxisSingleData, bool> condition) : base(inputEvent, canChange)
        {
            axis = datas;
            this.condition = condition;
            this.inputType = E_InputType.Axis;
        }

        public override void IsTrigger(Action<bool> action)
        {
            if (condition != null)
            {
                action?.Invoke(condition.Invoke(axis));
            }
            action?.Invoke(false);
        }

        public override I_BaseParam GetParam()
        {
            return new BaseParam<AxisSingleData>(this.inputType, axis);
        }

        protected override bool Compare(InputData other)
        {
            if (other is AxisData)
            {
                AxisData otherW = other as AxisData;
                return base.Compare(other) && condition == otherW.condition && axis.Compare(otherW.axis);
            }
            else
            {
                return base.Compare(other);
            }
        }
    }
}