
using System;
using TBFramework.Event;
using UnityEngine;

namespace TBFramework.Input
{
    public class TouchData : InputData
    {

        public Func<Touch[], bool> condition;

        public TouchData(string inputEvent, bool canChange, Func<Touch[], bool> condition) : base(inputEvent, canChange)
        {
            this.condition = condition;
            this.inputType = E_InputType.Touch;
        }

        public override void IsTrigger(Action<bool> action)
        {
            if (condition != null)
            {
                action?.Invoke(condition.Invoke(UnityEngine.Input.touches));
            }
            action?.Invoke(false);
        }

        public override I_BaseParam GetParam()
        {
            return new BaseParam<Touch[]>(this.inputType, UnityEngine.Input.touches);
        }

        protected override bool Compare(InputData other)
        {
            if (other is TouchData)
            {
                TouchData otherW = other as TouchData;
                return base.Compare(other) && condition == otherW.condition;
            }
            else
            {
                return base.Compare(other);
            }
        }

    }
}