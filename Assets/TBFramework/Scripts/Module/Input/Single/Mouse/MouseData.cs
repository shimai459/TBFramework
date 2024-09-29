
using System;

namespace TBFramework.Input
{
    public class MouseData : InputData
    {
        private MouseSingleData mouse;

        private Func<MouseSingleData, bool> condition;
        public MouseData(string inputEvent, bool canChange, MouseSingleData datas, Func<MouseSingleData, bool> condition) : base(inputEvent, canChange)
        {
            mouse = datas;
            this.condition = condition;
            this.inputType = E_InputType.Axis;
        }

        public override void IsTrigger(Action<bool> action)
        {
            if (condition != null)
            {
                action?.Invoke(condition.Invoke(mouse));
            }
            action?.Invoke(false);
        }

        public override I_BaseParam GetParam()
        {
            return new BaseParam<MouseSingleData>(this.inputType, mouse);
        }

        protected override bool Compare(InputData other)
        {
            if (other is MouseData)
            {
                MouseData otherW = other as MouseData;
                return base.Compare(other) && condition == otherW.condition && mouse.Compare(otherW.mouse);
            }
            else
            {
                return base.Compare(other);
            }
        }
    }
}