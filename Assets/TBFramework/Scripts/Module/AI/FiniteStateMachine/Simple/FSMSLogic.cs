using System.Collections.Generic;
using TBFramework.Mono;
using TBFramework.Pool;

/// <summary>
/// AI逻辑基类
/// </summary>
namespace TBFramework.AI.FSM.Simple
{
    public class FSMSLogic<T, V> : FSMSBaseLogic
    {
        private Dictionary<T, FSMSState<T, V>> stateDic = new Dictionary<T, FSMSState<T, V>>();

        private T defaultState;
        private T nowState;
        private T previousState;

        private V param;

        private bool isAddListen = false;

        public FSMSLogic()
        {
            MonoConManager.Instance.AddUpdateListener(Update);
            MonoConManager.Instance.AddLateUpdateListener(LateUpdate);
            MonoConManager.Instance.AddFixedUpdateListener(FixedUpdate);
            isAddListen = true;
        }

        public void Set(V param, T defaultState, params (T key, FSMSState<T, V> state)[] states)
        {
            this.param = param;
            this.AddStates(states);
            if (stateDic.ContainsKey(defaultState))
            {
                this.defaultState = defaultState;
                this.nowState = defaultState;
                stateDic[defaultState]?.enter?.Invoke(param);
            }
            if (!isAddListen)
            {
                MonoConManager.Instance.AddUpdateListener(Update);
                MonoConManager.Instance.AddLateUpdateListener(LateUpdate);
                MonoConManager.Instance.AddFixedUpdateListener(FixedUpdate);
                isAddListen = true;
            }
        }

        public void ChangeParam(string valueName, object value)
        {
            param.GetType().GetProperty(valueName).SetValue(param, value);
        }

        public V GetParam()
        {
            return param;
        }

        public void AddState(T key, FSMSState<T, V> state)
        {
            if (!stateDic.ContainsKey(key))
            {
                stateDic.Add(key, state);
            }
        }

        public void AddStates(params (T key, FSMSState<T, V> state)[] states)
        {
            foreach (var item in states)
            {
                AddState(item.key, item.state);
            }
        }

        public void RemoveState(T key)
        {
            if (stateDic.ContainsKey(key))
            {
                FSMSManager.Instance.states.Destory(stateDic[key]);
                stateDic.Remove(key);
            }
        }

        public void RemoveState(FSMSState<T, V> state)
        {
            if (stateDic.ContainsValue(state))
            {
                foreach (var item in stateDic)
                {
                    if (item.Value == state)
                    {
                        FSMSManager.Instance.states.Destory(state.key);
                        stateDic.Remove(item.Key);
                        break;
                    }
                }
            }
        }

        public void RemoveStates(params T[] keys)
        {
            foreach (var item in keys)
            {
                RemoveState(item);
            }
        }

        public void RemoveStates(params FSMSState<T, V>[] states)
        {
            foreach (var item in states)
            {
                RemoveState(item);
            }
        }

        public void ChangeState(T key)
        {
            if (stateDic.ContainsKey(key) && !key.Equals(nowState))
            {
                stateDic[nowState]?.exit?.Invoke(param);
                previousState = nowState;
                nowState = key;
                stateDic[nowState]?.enter?.Invoke(param);
            }
        }

        public void ToDefaultState()
        {
            ChangeState(defaultState);
        }

        public void ToPreviousState()
        {
            ChangeState(previousState);
        }

        private void Update()
        {
            if (stateDic[nowState] != null && stateDic[nowState].change != null)
            {
                T key = stateDic[nowState].change(param);
                ChangeState(key);
            }
            stateDic[nowState]?.update?.Invoke(param);
        }

        private void LateUpdate()
        {
            stateDic[nowState]?.lateUpdate?.Invoke(param);
        }

        private void FixedUpdate()
        {
            stateDic[nowState]?.fixedUpdate?.Invoke(param);
        }

        public void Clear()
        {
            foreach (FSMSState<T, V> item in stateDic.Values)
            {
                CPoolManager.Instance.Push(item);
            }
            this.stateDic.Clear();
        }

        public override void Reset()
        {
            base.Reset();
            MonoConManager.Instance.RemoveUpdateListener(Update);
            MonoConManager.Instance.RemoveLateUpdateListener(LateUpdate);
            MonoConManager.Instance.RemoveFixedUpdateListener(FixedUpdate);
            isAddListen = false;
            this.Clear();
            this.nowState = default(T);
            this.defaultState = default(T);
            this.previousState = default(T);
        }
    }
}
