using System;
using TBFramework.Event;
using UnityEngine;

namespace TBFramework.Input
{
    public class GyroData : InputData
    {
        private Func<Gyroscope, bool> condition;

        public GyroData(string inputEvent, bool canChange, Func<Gyroscope, bool> condition) : base(inputEvent, canChange)
        {
            this.condition = condition;
            this.inputType = E_InputType.Gyro;
        }

        public override void IsTrigger(Action<bool> action)
        {
            if (condition != null)
            {
                action?.Invoke(condition.Invoke(UnityEngine.Input.gyro));
            }
            action?.Invoke(false);
        }

        public override I_BaseParam GetParam()
        {
            return new BaseParam<Gyroscope>(this.inputType, UnityEngine.Input.gyro);
        }

        protected override bool Compare(InputData other)
        {
            if (other is GyroData)
            {
                GyroData otherW = other as GyroData;
                return base.Compare(other) && condition == otherW.condition;
            }
            else
            {
                return base.Compare(other);
            }
        }

    }
}