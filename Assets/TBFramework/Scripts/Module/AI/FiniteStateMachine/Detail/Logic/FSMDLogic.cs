using System.Collections.Generic;
using TBFramework.Mono;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDLogic<T> : FSMDBaseLogic
    {

        //private FSMDStateArray<T> states;
        #region 初始化
        public void Set(T defaultState, params (T stateKey, FSMDState state)[] states)
        {
            this.AddStates(states);
            if (this.states.ContainsKey(defaultState))
            {
                this.defaultState = defaultState;
                this.currentState = defaultState;
                this.states[defaultState]?.enter?.Invoke(context);
            }
        }

        #endregion

        #region 运行操作

        private bool isAddListen = false;

        public override void StartLogic()
        {
            if (!isAddListen)
            {
                MonoConManager.Instance.AddUpdateListener(Update);
                MonoConManager.Instance.AddLateUpdateListener(LateUpdate);
                MonoConManager.Instance.AddFixedUpdateListener(FixedUpdate);
                isAddListen = true;
            }
        }

        public override void StopLogic()
        {
            MonoConManager.Instance.RemoveUpdateListener(Update);
            MonoConManager.Instance.RemoveLateUpdateListener(LateUpdate);
            MonoConManager.Instance.RemoveFixedUpdateListener(FixedUpdate);
            isAddListen = false;
        }

        #endregion

        #region 状态字典

        private Dictionary<T, FSMDState> states = new Dictionary<T, FSMDState>();

        public void AddState(T key, FSMDState state)
        {
            if (!states.ContainsKey(key))
            {
                states.Add(key, state);
                FSMDManager.Instance.states.AddUse(state);
            }
        }

        public void AddStates((T stateKey, FSMDState state)[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                AddState(states[i].stateKey, states[i].state);
            }
        }

        public void ChangeState(T key, FSMDState state)
        {
            RemoveState(key);
            AddState(key, state);
        }

        public void ChangeStates((T stateKey, FSMDState state)[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                ChangeState(states[i].stateKey, states[i].state);
            }
        }

        public void RemoveState(T key)
        {
            if (states.ContainsKey(key))
            {
                if (states[key] != null)
                {
                    FSMDManager.Instance.states.Destory(states[key].key);
                }
                states.Remove(key);
            }
        }

        public void RemoveStates(T[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                RemoveState(keys[i]);
            }
        }

        public void RemoveState(FSMDState state)
        {
            if (states.ContainsValue(state))
            {
                foreach (var item in states)
                {
                    if (item.Value == state)
                    {
                        FSMDManager.Instance.states.Destory(state.key);
                        states.Remove(item.Key);
                        break;
                    }
                }
            }
        }

        public void RemoveStates(FSMDState[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                RemoveState(states[i]);
            }
        }

        public void Clear()
        {
            foreach (FSMDState state in states.Values)
            {
                FSMDManager.Instance.states.Destory(state.key);
            }
            states.Clear();
        }


        #endregion

        #region 运行具体逻辑

        private T currentState;
        private T defaultState;
        private T previousState;

        private void Update()
        {
            if (currentState != null && states.ContainsKey(currentState))
            {
                FSMDBaseTransition transition = states[currentState].transition;
                if (transition != null && transition is FSMDBaseTransition<T>)
                {
                    T newState = (transition as FSMDBaseTransition<T>).Change(context);
                    Change(newState);
                }

                states[currentState]?.update?.Invoke(context);
            }
        }

        private void LateUpdate()
        {
            if (currentState != null && states.ContainsKey(currentState))
            {
                states[currentState]?.lateUpdate?.Invoke(context);
            }
        }

        private void FixedUpdate()
        {
            if (currentState != null && states.ContainsKey(currentState))
            {
                states[currentState]?.fixedUpdate?.Invoke(context);
            }
        }

        #endregion

        #region 变更

        public void Change(T state)
        {
            if (!state.Equals(default(T)) && !state.Equals(currentState) && states.ContainsKey(state))
            {
                states[currentState]?.exit?.Invoke(context);
                previousState = currentState;
                currentState = state;
                states[currentState]?.enter?.Invoke(context);
            }
        }

        public override void ToDefault()
        {
            Change(defaultState);
        }

        public void ToPrevious()
        {
            Change(previousState);
        }

        #endregion

        #region 重置

        public override void Reset()
        {
            base.Reset();
            this.StopLogic();
            this.Clear();
            currentState = default(T);
            previousState = default(T);
            defaultState = default(T);
        }

        #endregion

    }
}