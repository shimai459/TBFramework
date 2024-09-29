
using System;
using System.Diagnostics;
using TBFramework.Event;
using UnityEngine.Rendering;

namespace TBFramework.Input
{
    public abstract class InputData
    {
        public string inputEvent;//事件名称

        public E_InputType inputType;//输入类型

        public bool canChange;

        public InputData(string inputEvent, bool canChange)
        {
            this.inputEvent = inputEvent;
            this.canChange = canChange;
        }

        public virtual void Check()
        {
            IsTrigger((b) =>
            {
                if (b)
                {
                    EventCenter.Instance.EventTrigger<I_BaseParam>(inputEvent, GetParam());
                }
                else
                {
                    EventCenter.Instance.EventTrigger<I_BaseParam>(inputEvent + InputSet.NOT_TRIGGER_SUFFIX, GetParam());
                }
            });
        }

        public abstract void IsTrigger(Action<bool> action);

        public abstract I_BaseParam GetParam();

        #region 运算符重载

        public static bool operator ==(InputData left, InputData right)
        {
            if (left is InputData && right is InputData)
            {
                return left.Compare(right) && right.Compare(left);
            }
            else
            {
                return left as object == right as object;
            }
        }

        public static bool operator !=(InputData left, InputData right)
        {
            if (left is InputData && right is InputData)
            {
                return !(left.Compare(right) && right.Compare(left));
            }
            else
            {
                return left as object != right as object;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is InputData && this is InputData)
            {
                InputData other = obj as InputData;
                return Compare(other) && other.Compare(this);
            }
            else
            {
                return base.Equals(obj);
            }

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        protected virtual bool Compare(InputData other)
        {
            return inputEvent == other.inputEvent && inputType == other.inputType && canChange == other.canChange;
        }

        #endregion
    }
}
