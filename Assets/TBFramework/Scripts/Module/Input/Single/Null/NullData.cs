
using System;

namespace TBFramework.Input
{
    public class NullData : InputData
    {
        public bool canTrigger;
        public NullData(string inputEvent, bool canChange, bool canTrigger = false) : base(inputEvent, canChange)
        {
            this.inputType = E_InputType.Null;
            this.canTrigger = canTrigger;
        }

        public override I_BaseParam GetParam()
        {
            return new I_BaseParam(this.inputType);
        }

        public override void IsTrigger(Action<bool> action)
        {
            if (canTrigger)
            {
                action(false);
            }
        }

        protected override bool Compare(InputData other)
        {
            if (other is NullData)
            {
                NullData otherW = other as NullData;
                return base.Compare(other) && canTrigger == otherW.canTrigger;
            }
            else
            {
                return base.Compare(other);
            }
        }
    }
}