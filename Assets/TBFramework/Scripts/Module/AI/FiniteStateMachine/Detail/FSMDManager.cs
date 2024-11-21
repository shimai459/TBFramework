
using System;
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDManager : Singleton<FSMDManager>
    {
        #region Logic
        public BaseMgrObj<FSMDBaseLogic> logics = new BaseMgrObj<FSMDBaseLogic>();

        public FSMDLogic<V> CreateFSMD<V>(BaseContext context, FSMDStateArray<V> states, V defaultState)
        {
            FSMDLogic<V> logic = CPoolManager.Instance.Pop<FSMDLogic<V>>();
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

        public FSMDAction<BaseContext> CreateFSMDAction(Action<BaseContext> action)
        {
            FSMDAction<BaseContext> fsmdAction = CPoolManager.Instance.Pop<FSMDAction<BaseContext>>();
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

        public FSMDTransition<BaseContext, V> CreateTransition<V>(Func<BaseContext, V> func)
        {
            FSMDTransition<BaseContext, V> transition = CPoolManager.Instance.Pop<FSMDTransition<BaseContext, V>>();
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

        public BaseMgrObj<BaseContext> contexts = new BaseMgrObj<BaseContext>();

        public ContextWithTParam<T> CreateContext<T>(T param)
        {
            ContextWithTParam<T> context = CPoolManager.Instance.Pop<ContextWithTParam<T>>();
            context.Set(param);
            contexts.Create(context);
            return context;
        }

        public ContextWithDic CreateContext()
        {
            ContextWithDic context = CPoolManager.Instance.Pop<ContextWithDic>();
            contexts.Create(context);
            return context;
        }

        #endregion
    }
}
