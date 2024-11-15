using System;
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Simple
{
    public class FSMSManager : Singleton<FSMSManager>
    {
        //#region Logic
        public BaseMgrObj<FSMSBaseLogic> logics = new BaseMgrObj<FSMSBaseLogic>();

        public FSMSLogic<T, V> CreateFSMS<T, V>(V param, T defaultState, params (T key, FSMSState<T, V> state)[] states)
        {
            FSMSLogic<T, V> logic = CPoolManager.Instance.Pop<FSMSLogic<T, V>>();
            logic.Set(param, defaultState, states);
            for (int i = 0; i < states.Length; i++)
            {
                this.states.AddUse(states[i].state);
            }
            logics.Create(logic);
            logics.AddUse(logic);
            return logic;
        }

        //#endregion

        //#region State

        public BaseMgrObj<FSMSBaseState> states = new BaseMgrObj<FSMSBaseState>();

        public FSMSState<T, V> CreateFSMSState<T, V>(Action<V> enter, Action<V> update, Action<V> lateUpdate, Action<V> fixedUpdate, Action<V> exit, Func<V, T> change)
        {
            FSMSState<T, V> state = CPoolManager.Instance.Pop<FSMSState<T, V>>();
            state.Set(enter, update, lateUpdate, fixedUpdate, exit, change);
            states.Create(state);
            return state;
        }

        //#endregion
    }
}
