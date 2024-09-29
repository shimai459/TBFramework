using System;
using System.Collections.Generic;
using TBFramework.Event;

namespace TBFramework.Input
{
    public class KeyData : InputData
    {
        public KeySingleData data;
        public KeyData(string inputEvent, bool canChange, KeySingleData keys) : base(inputEvent, canChange)
        {
            this.data = keys;
            this.inputType = E_InputType.Key;
        }

        public override void IsTrigger(Action<bool> action)
        {
            action?.Invoke(data.IsTrigger());
        }

        public override I_BaseParam GetParam()
        {
            return new BaseParam<KeySingleData>(this.inputType, data);
        }

        protected override bool Compare(InputData other)
        {
            if (other is KeyData)
            {
                KeyData otherW = other as KeyData;
                return base.Compare(other) && data.Compare(otherW.data);
            }
            else
            {
                return base.Compare(other);
            }
        }
    }
}