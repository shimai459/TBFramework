using System.Collections.Generic;
using TBFramework.Mono;
using TBFramework.Pool;

/// <summary>
/// AI逻辑基类
/// </summary>
namespace TBFramework.AI.FSM.Simple
{
    public class FSMSLogic<T> : FSMSBaseLogic<T>
    {

        #region 初始化

        public void Set(T defaultState, params (T key, FSMSState<T> state)[] states)
        {
            this.AddStates(states);
            if (stateDic.ContainsKey(defaultState))
            {
                this.defaultState = defaultState;
                this.nowState = defaultState;
                stateDic[defaultState]?.enter?.Invoke(context);
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

        #region 状态字典操作

        private Dictionary<T, FSMSState<T>> stateDic = new Dictionary<T, FSMSState<T>>();

        public void AddState(T key, FSMSState<T> state)
        {
            if (!stateDic.ContainsKey(key) && KeyBase.IsLegal(state))
            {
                stateDic.Add(key, state);
                FSMSManager.Instance.states.AddUse(state);
            }
        }

        public void AddStates(params (T key, FSMSState<T> state)[] states)
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

        public void RemoveState(FSMSState<T> state)
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

        public void RemoveStates(params FSMSState<T>[] states)
        {
            foreach (var item in states)
            {
                RemoveState(item);
            }
        }

        public void Clear()
        {
            foreach (FSMSState<T> item in stateDic.Values)
            {
                FSMSManager.Instance.states.Destory(item.key);
            }
            this.stateDic.Clear();
        }

        #endregion

        #region 状态切换操作

        public void Change(T key)
        {
            if (stateDic.ContainsKey(key) && !key.Equals(nowState))
            {
                stateDic[nowState]?.exit?.Invoke(context);
                previousState = nowState;
                nowState = key;
                stateDic[nowState]?.enter?.Invoke(context);
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

        #region 运行具体逻辑

        private T defaultState;
        private T nowState;
        private T previousState;


        private void Update()
        {
            if (stateDic[nowState] != null && stateDic[nowState].change != null)
            {
                T key = stateDic[nowState].change(context);
                Change(key);
            }
            stateDic[nowState]?.update?.Invoke(context);
        }

        private void LateUpdate()
        {
            stateDic[nowState]?.lateUpdate?.Invoke(context);
        }

        private void FixedUpdate()
        {
            stateDic[nowState]?.fixedUpdate?.Invoke(context);
        }

        #endregion

        #region 重置

        public override void Reset()
        {
            base.Reset();
            this.StopLogic();
            this.Clear();
            this.nowState = default(T);
            this.defaultState = default(T);
            this.previousState = default(T);
        }

        #endregion

    }
}
