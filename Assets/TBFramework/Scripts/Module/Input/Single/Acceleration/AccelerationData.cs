
using System;
using TBFramework.Event;
using UnityEngine;

namespace TBFramework.Input
{
    public class AccelerationData : InputData
    {
        private Func<AccelerationEvent[], bool> condition;

        public AccelerationData(string inputEvent, bool canChange, Func<AccelerationEvent[], bool> condition) : base(inputEvent, canChange)
        {
            this.condition = condition;
            this.inputType = E_InputType.Acceleration;
        }

        public override void IsTrigger(Action<bool> action)
        {
            if (condition != null)
            {
                action?.Invoke(condition.Invoke(UnityEngine.Input.accelerationEvents));
            }
            action?.Invoke(false);
        }

        public override I_BaseParam GetParam()
        {
            return new BaseParam<AccelerationEvent[]>(this.inputType, UnityEngine.Input.accelerationEvents);
        }

        protected override bool Compare(InputData other)
        {
            if (other is AccelerationData)
            {
                AccelerationData otherW = other as AccelerationData;
                return base.Compare(other) && condition == otherW.condition;
            }
            else
            {
                return base.Compare(other);
            }
        }
    }
}