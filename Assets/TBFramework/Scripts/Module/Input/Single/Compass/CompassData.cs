using System;
using TBFramework.Event;
using UnityEngine;

namespace TBFramework.Input
{
    public class CompassData : InputData
    {

        private Func<Compass, bool> condition;

        public CompassData(string inputEvent, bool canChange, Func<Compass, bool> condition) : base(inputEvent, canChange)
        {
            this.condition = condition;
            this.inputType = E_InputType.Compass;
        }

        public override void IsTrigger(Action<bool> action)
        {
            if (condition != null)
            {
                action?.Invoke(condition.Invoke(UnityEngine.Input.compass));
            }
            action?.Invoke(false);
        }

        public override I_BaseParam GetParam()
        {
            return new BaseParam<Compass>(this.inputType, UnityEngine.Input.compass);
        }

        protected override bool Compare(InputData other)
        {
            if (other is CompassData)
            {
                CompassData otherW = other as CompassData;
                return base.Compare(other) && condition == otherW.condition;
            }
            else
            {
                return base.Compare(other);
            }
        }

    }
}