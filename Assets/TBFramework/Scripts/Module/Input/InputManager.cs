using TBFramework.Mono;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

namespace TBFramework.Input
{

    public class InputManager : Singleton<InputManager>
    {

        #region 检测
        public InputManager()
        {
            MonoManager.Instance.AddUpdateListener(MyUpdate);
        }

        /// <summary>
        /// 检测输入映射事件
        /// </summary>
        private void MyUpdate()
        {
            if (masterSwitch)
            {
                foreach (InputData input in inputs.Values)
                {
                    if (CanTrigger(input.inputType))
                    {
                        input.Check();
                    }
                }
            }
        }

        #endregion

        #region 开关

        private bool masterSwitch = true;//总开关
        public bool MasterSwitch { set { masterSwitch = value; } }//设置总开关
        private Dictionary<E_InputType, bool> switchList = new Dictionary<E_InputType, bool>();//不同类型输入开关

        /// <summary>
        /// 设置某种输入映射类型的开关
        /// </summary>
        /// <param name="type">输入映射类型</param>
        /// <param name="value">开关状态</param>
        public void SetSwitch(E_InputType type, bool value)
        {
            if (switchList.ContainsKey(type))
            {
                switchList[type] = value;
            }
            else
            {
                switchList.Add(type, value);
            }

        }

        /// <summary>
        /// 获取具体输入映射类型开关状态
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanTrigger(E_InputType type)
        {
            if (switchList.ContainsKey(type))
            {
                return switchList[type];
            }
            return true;
        }

        #endregion

        #region 输入映射

        private Dictionary<string, InputData> inputs = new Dictionary<string, InputData>();//所有的输入映射

        /// <summary>
        /// 获取所有的输入映射
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, InputData> GetInputs() { return inputs; }

        /// <summary>
        /// 添加输入映射
        /// </summary>
        /// <param name="inputs">输入映射</param>
        public void AddInput(params InputData[] inputs)
        {
            foreach (InputData input in inputs)
            {
                if (this.inputs.ContainsKey(input.inputEvent))
                {
                    InputData inputData = this.inputs[input.inputEvent];
                    if (inputData.inputType == E_InputType.Or)
                    {
                        (this.inputs[input.inputEvent] as OrData).AddData(input);
                    }
                    else if (inputData != input)
                    {
                        InputData newInput = new OrData(input.inputEvent, true, true, inputData, input);
                        this.inputs[input.inputEvent] = newInput;
                    }
                }
                else
                {
                    this.inputs.Add(input.inputEvent, input);
                }
            }
        }

        /// <summary>
        /// 通过事件名来移除输入映射
        /// </summary>
        /// <param name="inputEvents">输入映射事件名</param>
        public void RemoveInput(params string[] inputEvents)
        {
            foreach (string inputEvent in inputEvents)
            {
                if (inputs.ContainsKey(inputEvent))
                {
                    inputs.Remove(inputEvent);
                }
            }
        }

        /// <summary>
        /// 移除输入映射
        /// </summary>
        /// <param name="inputs">输入映射</param>
        public void RemoveInput(params InputData[] inputs)
        {
            foreach (InputData input in inputs)
            {
                if (this.inputs.ContainsKey(input.inputEvent))
                {
                    this.inputs.Remove(input.inputEvent);
                }
            }
        }

        /// <summary>
        /// 清楚所有输入映射
        /// </summary>
        public void ClearInput()
        {
            inputs.Clear();
        }

        #endregion

        #region 改键

        /// <summary>
        /// 根据事件名改变输入映射，没有就添加
        /// </summary>
        /// <param name="newInput">输入映射</param>
        public void ChangeInput(InputData newInput)
        {
            if (inputs.ContainsKey(newInput.inputEvent))
            {
                InputData inputData = inputs[newInput.inputEvent];
                if (inputData.canChange)
                {
                    inputs[newInput.inputEvent] = newInput;
                }
            }
            else
            {
                inputs.Add(newInput.inputEvent, newInput);
            }
        }

        /// <summary>
        /// 替换旧输入映射为新输入映射
        /// </summary>
        /// <param name="oldInput">旧输入映射</param>
        /// <param name="newInput">新输入映射</param>
        /// <returns></returns>
        public bool ChangeInput(InputData oldInput, InputData newInput)
        {
            if (oldInput.inputEvent == newInput.inputEvent && oldInput.canChange)
            {
                if (inputs.ContainsKey(oldInput.inputEvent))
                {
                    if (inputs[oldInput.inputEvent] == oldInput)
                    {
                        inputs[oldInput.inputEvent] = newInput;
                        return true;
                    }
                    else if (inputs[oldInput.inputEvent].inputType == E_InputType.Or)
                    {
                        (inputs[oldInput.inputEvent] as OrData).ChangeData(oldInput, newInput);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 替换旧输入映射为新输入映射，没有就旧映射就使用根据事件名改变输入映射，没有就添加的方法
        /// </summary>
        /// <param name="oldInput">旧输入映射</param>
        /// <param name="newInput">新输入映射</param>
        public void ChangeInputOrAdd(InputData oldInput, InputData newInput)
        {
            if (!ChangeInput(oldInput, newInput))
            {
                ChangeInput(newInput);
            }
        }

        /// <summary>
        /// 通过按键检测修改输入映射
        /// </summary>
        /// <param name="oldInput"></param>
        public void ChangeInputWithCheck(InputData oldInput)
        {
            ChangeInputWithCheck(oldInput, KeyCodesToInputData);
        }

        /// <summary>
        /// 通过按键检测修改输入映射(自定义按键列表转换输入映射)
        /// </summary>
        /// <param name="oldInput"></param>
        /// <param name="action"></param>
        public void ChangeInputWithCheck(InputData oldInput, Func<string, List<KeyCode>, InputData> action)
        {
            MonoManager.Instance.StartCoroutine(ReallyCheckTriggerKeyCode((list) =>
            {
                if (action != null)
                {
                    ChangeInput(oldInput, action.Invoke(oldInput.inputEvent, list));
                }
                else
                {
                    ChangeInput(oldInput, KeyCodesToInputData(oldInput.inputEvent, list));
                }

            }));
        }

        /// <summary>
        /// 真正检测当前的键位输入
        /// </summary>
        /// <param name="action">处理键位输入的事件</param>
        /// <returns></returns>
        private IEnumerator ReallyCheckTriggerKeyCode(Action<List<KeyCode>> action)
        {
            yield return null;
            List<KeyCode> tKeyCodes = new List<KeyCode>();
            Array keyCodes = Enum.GetValues(typeof(KeyCode));
            E_ChangeStatus status = E_ChangeStatus.Start;
            while (status != E_ChangeStatus.End)
            {
                if (UnityEngine.Input.anyKeyDown)
                {
                    foreach (KeyCode keyCode in keyCodes)
                    {
                        if (UnityEngine.Input.GetKeyDown(keyCode))
                        {
                            tKeyCodes.Add(keyCode);
                            status = E_ChangeStatus.Ongoing;
                        }
                    }
                }
                if (status == E_ChangeStatus.Ongoing && !UnityEngine.Input.anyKey && !UnityEngine.Input.anyKeyDown)
                {
                    status = E_ChangeStatus.End;
                    break;
                }
                yield return null;
            }
            action?.Invoke(tKeyCodes);
        }

        /// <summary>
        /// 将键位输入转为输入映射
        /// </summary>
        /// <param name="inputEvent">输入映射事件名</param>
        /// <param name="list">键位输入</param>
        /// <returns></returns>
        private InputData KeyCodesToInputData(string inputEvent, List<KeyCode> list)
        {
            if (list.Count == 1)
            {
                return new KeyData(inputEvent, true, new KeySingleData(list[0], E_KeyOperationType.Down));
            }
            else if (list.Count > 1)
            {
                WithData withData = new WithData(inputEvent, true, InputSet.DEFAULT_DELAY_VALUE, null);
                foreach (KeyCode keyCode in list)
                {
                    withData.AddData(new KeyData(inputEvent, true, new KeySingleData(keyCode, CheckKeyCodeType(keyCode))));
                }
                return withData;
            }
            return new NullData(inputEvent, true);
        }

        /// <summary>
        /// 检测键位输入，返回输入映射类型
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        private E_KeyOperationType CheckKeyCodeType(KeyCode keyCode)
        {
            if (keyCode == KeyCode.LeftControl ||
                keyCode == KeyCode.RightControl ||
                keyCode == KeyCode.LeftAlt ||
                keyCode == KeyCode.RightAlt ||
                keyCode == KeyCode.LeftShift ||
                keyCode == KeyCode.RightShift)
            {
                return E_KeyOperationType.Hold;
            }
            else
            {
                return E_KeyOperationType.Down;
            }
        }

        #endregion
    }
}
