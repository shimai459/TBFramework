
using System;
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDManager : Singleton<FSMDManager>
    {
        #region Logic
        public BaseMgrObj<FSMDBaseLogic> logics = new BaseMgrObj<FSMDBaseLogic>();

        public FSMDLogic<T, V> CreateFSMD<T, V>(FSMDContext<T> context, FSMDStateArray<V> states, V defaultState)
        {
            FSMDLogic<T, V> logic = CPoolManager.Instance.Pop<FSMDLogic<T, V>>();
            logic.Set(context, states, defaultState);
            contexts.AddUse(context);
            stateArrays.AddUse(states);
            logics.Create(logic);
            logics.AddUse(logic);
            return logic;
        }

        #endregion

        #region Action

        public BaseMgrObj<FSMDBaseAction> actions = new BaseMgrObj<FSMDBaseAction>();

        public FSMDAction CreateFSMDAction(Action action)
        {
            FSMDAction fsmdAction = CPoolManager.Instance.Pop<FSMDAction>();
            fsmdAction.Set(action);
            actions.Create(fsmdAction);
            return fsmdAction;
        }

        public FSMDAction<T> CreateFSMDAction<T>(T param, Action<T> action)
        {
            FSMDAction<T> fsmdAction = CPoolManager.Instance.Pop<FSMDAction<T>>();
            fsmdAction.Set(param, action);
            actions.Create(fsmdAction);
            return fsmdAction;
        }

        public FSMDAction<FSMDBaseContext> CreateFSMDAction(Action<FSMDBaseContext> action)
        {
            FSMDAction<FSMDBaseContext> fsmdAction = CPoolManager.Instance.Pop<FSMDAction<FSMDBaseContext>>();
            fsmdAction.Set(action);
            actions.Create(fsmdAction);
            return fsmdAction;
        }

        #endregion

        #region Transition

        public BaseMgrObj<FSMDBaseTransition> transitions = new BaseMgrObj<FSMDBaseTransition>();

        public FSMDTransition<V> CreateTransition<V>(Func<V> func)
        {
            FSMDTransition<V> transition = CPoolManager.Instance.Pop<FSMDTransition<V>>();
            transition.Set(func);
            transitions.Create(transition);
            return transition;
        }

        public FSMDTransition<T, V> CreateTransition<T, V>(T param, Func<T, V> func)
        {
            FSMDTransition<T, V> transition = CPoolManager.Instance.Pop<FSMDTransition<T, V>>();
            transition.Set(param, func);
            transitions.Create(transition);
            return transition;
        }

        public FSMDTransition<FSMDBaseContext, V> CreateTransition<V>(Func<FSMDBaseContext, V> func)
        {
            FSMDTransition<FSMDBaseContext, V> transition = CPoolManager.Instance.Pop<FSMDTransition<FSMDBaseContext, V>>();
            transition.Set(func);
            transitions.Create(transition);
            return transition;
        }

        #endregion

        #region State

        public BaseMgrObj<FSMDBaseState> states = new BaseMgrObj<FSMDBaseState>();

        public FSMDState<V> CreateState<V>(FSMDBaseAction enter, FSMDBaseAction update, FSMDBaseAction exit, FSMDBaseTransition<V> transition)
        {
            FSMDState<V> state = CPoolManager.Instance.Pop<FSMDState<V>>();
            state.Set(enter, update, exit, transition);
            actions.AddUse(enter.key);
            actions.AddUse(update.key);
            actions.AddUse(exit.key);
            transitions.AddUse(transition.key);
            states.Create(state);
            return state;
        }

        public FSMDState<V> CreateState<V>(FSMDBaseAction enter, FSMDBaseAction update, FSMDBaseAction lateUpdate, FSMDBaseAction fixedUpdate, FSMDBaseAction exit, FSMDBaseTransition<V> transition)
        {
            FSMDState<V> state = CPoolManager.Instance.Pop<FSMDState<V>>();
            state.Set(enter, update, lateUpdate, fixedUpdate, exit, transition);
            actions.AddUse(enter.key);
            actions.AddUse(update.key);
            actions.AddUse(lateUpdate.key);
            actions.AddUse(fixedUpdate.key);
            actions.AddUse(exit.key);
            transitions.AddUse(transition.key);
            states.Create(state);
            return state;
        }

        #endregion

        #region StateArray

        public BaseMgrObj<FSMDBaseStateArray> stateArrays = new BaseMgrObj<FSMDBaseStateArray>();

        public FSMDStateArray<V> CreateStateArray<V>(params (V stateKey, FSMDState<V> state)[] states)
        {
            FSMDStateArray<V> stateArray = CPoolManager.Instance.Pop<FSMDStateArray<V>>();
            stateArray.Set(states);
            for (int i = 0; i < states.Length; i++)
            {
                this.states.AddUse(states[i].state.key);
            }
            stateArrays.Create(stateArray);
            return stateArray;
        }

        #endregion

        #region Context

        public BaseMgrObj<FSMDBaseContext> contexts = new BaseMgrObj<FSMDBaseContext>();

        public FSMDContext<T> CreateContext<T>(T param)
        {
            FSMDContext<T> context = CPoolManager.Instance.Pop<FSMDContext<T>>();
            context.Set(param);
            contexts.Create(context);
            return context;
        }

        #endregion
    }
}
