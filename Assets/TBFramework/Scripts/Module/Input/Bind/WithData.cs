
using System;
using System.Collections;
using System.Collections.Generic;
using TBFramework.Event;
using TBFramework.Mono;
using UnityEngine;

namespace TBFramework.Input
{
    public class WithData : BindBaseData
    {
        private float delay;

        private List<InputData> triggers;

        private float timer = 0;

        private bool isAllTrigger = false;

        private BaseParam<List<I_BaseParam>> param = new BaseParam<List<I_BaseParam>>(E_InputType.With, new List<I_BaseParam>());

        private Action<bool> triggerAction;
        public WithData(string inputEvent, bool canChange, float delay, params InputData[] datas) : base(inputEvent, canChange, datas)
        {
            this.inputType = E_InputType.With;
            this.delay = delay;
        }

        public override void Check()
        {
            if (timer == 0)
            {
                base.Check();
            }

        }

        public override void IsTrigger(Action<bool> action)
        {
            if (timer == 0)
            {
                triggerAction = action;
                MonoManager.Instance.StartCoroutine(ReallyIsTrigger());
            }
            else
            {
                triggerAction += action;
            }
        }

        public override I_BaseParam GetParam()
        {
            return param;
        }

        protected override bool Compare(InputData other)
        {
            if (other is WithData)
            {
                WithData otherW = other as WithData;
                return base.Compare(other) && delay == otherW.delay;
            }
            else
            {
                return base.Compare(other);
            }
        }


        private IEnumerator ReallyIsTrigger()
        {
            timer = 0;
            triggers = new List<InputData>();
            param.param = new List<I_BaseParam>();
            isAllTrigger = false;
            while (timer < delay)
            {
                timer += Time.deltaTime;
                IsAllTriggered();
                if (isAllTrigger || triggers.Count == 0)
                {
                    break;
                }
                else
                {
                    yield return 0;
                }
            }
            timer = 0;
            triggerAction?.Invoke(isAllTrigger);
        }

        private void IsAllTriggered()
        {
            bool isAll = true;
            foreach (InputData data in datas)
            {
                if (!triggers.Contains(data))
                {
                    isAll = false;
                    InputData temp = data;
                    data.IsTrigger((b) =>
                    {
                        if (b)
                        {
                            triggers.Add(data);
                            param.param.Add(temp.GetParam());
                            IsAllTriggered();
                        }
                    });
                }
            }
            isAllTrigger = isAll;
        }
    }
}