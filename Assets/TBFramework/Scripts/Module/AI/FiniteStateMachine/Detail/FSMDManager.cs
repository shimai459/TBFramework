
using System;
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDManager : Singleton<FSMDManager>
    {
        #region Logic
        public BaseMgrObj<FSMDBaseLogic> logics = new BaseMgrObj<FSMDBaseLogic>();

        public FSMDLogic<T> CreateFSMD<T>(T defaultState, BaseContext context, bool isAddUseMeself = false, params (T key, FSMDState state)[] states)
        {
            FSMDLogic<T> logic = CPoolManager.Instance.Pop<FSMDLogic<T>>();
            logic.Set(defaultState, states);
            logic.SetContext(context);
            logic.StartLogic();
            logics.Create(logic);
            if (isAddUseMeself)
            {
                logics.AddUse(logic);
            }
            return logic;
        }

        public FSMDLogic<T> CreateFSMD<T>(T defaultState, FSMDTransitionNoParam<T> transition, bool isAddUseMeself = false, params (T key, FSMDState state)[] states)
        {
            FSMDLogic<T> logic = CPoolManager.Instance.Pop<FSMDLogic<T>>();
            logic.Set(defaultState, states);
            logic.SetTransition(transition);
            logics.Create(logic);
            if (isAddUseMeself)
            {
                logics.AddUse(logic);
            }
            return logic;
        }

        public FSMDLogicArray<T> CreateFSMDArray<T>(BaseContext context, T defaultState, bool isAddUseMeself = false, params (T key, FSMDBaseLogic logic)[] logics)
        {
            FSMDLogicArray<T> logicArray = CPoolManager.Instance.Pop<FSMDLogicArray<T>>();
            logicArray.SetContext(context);
            logicArray.Set(defaultState, logics);
            logicArray.StartLogic();
            this.logics.Create(logicArray);
            if (isAddUseMeself)
            {
                this.logics.AddUse(logicArray);
            }
            return logicArray;
        }

        public FSMDLogicArray<T> CreateFSMDArray<T>(T defaultState, FSMDTransitionNoParam<T> transition, bool isAddUseMeself = false, params (T key, FSMDBaseLogic logic)[] logics)
        {
            FSMDLogicArray<T> logicArray = CPoolManager.Instance.Pop<FSMDLogicArray<T>>();
            logicArray.Set(defaultState, logics);
            logicArray.SetTransition(transition);
            this.logics.Create(logicArray);
            if (isAddUseMeself)
            {
                this.logics.AddUse(logicArray);
            }
            return logicArray;
        }

        #endregion

        #region Action

        public BaseMgrObj<FSMDBaseAction> actions = new BaseMgrObj<FSMDBaseAction>();

        public FSMDActionNoParam CreateFSMDAction(Action action)
        {
            FSMDActionNoParam fsmdAction = CPoolManager.Instance.Pop<FSMDActionNoParam>();
            fsmdAction.Set(action);
            actions.Create(fsmdAction);
            return fsmdAction;
        }

        public FSMDActionWithInParam<T> CreateFSMDAction<T>(T param, Action<T> action)
        {
            FSMDActionWithInParam<T> fsmdAction = CPoolManager.Instance.Pop<FSMDActionWithInParam<T>>();
            fsmdAction.Set(param, action);
            actions.Create(fsmdAction);
            return fsmdAction;
        }

        public FSMDActionWithContext CreateFSMDAction(Action<BaseContext> action)
        {
            FSMDActionWithContext fsmdAction = CPoolManager.Instance.Pop<FSMDActionWithContext>();
            fsmdAction.Set(action);
            actions.Create(fsmdAction);
            return fsmdAction;
        }

        #endregion

        #region Transition

        public BaseMgrObj<FSMDBaseTransition> transitions = new BaseMgrObj<FSMDBaseTransition>();

        public FSMDTransitionNoParam<T> CreateTransition<T>(Func<T> func)
        {
            FSMDTransitionNoParam<T> transition = CPoolManager.Instance.Pop<FSMDTransitionNoParam<T>>();
            transition.Set(func);
            transitions.Create(transition);
            return transition;
        }

        public FSMDTransitionWithInParam<V, T> CreateTransition<V, T>(V param, Func<V, T> func)
        {
            FSMDTransitionWithInParam<V, T> transition = CPoolManager.Instance.Pop<FSMDTransitionWithInParam<V, T>>();
            transition.Set(param, func);
            transitions.Create(transition);
            return transition;
        }

        public FSMDTransitionWithContext<T> CreateTransition<T>(Func<BaseContext, T> func)
        {
            FSMDTransitionWithContext<T> transition = CPoolManager.Instance.Pop<FSMDTransitionWithContext<T>>();
            transition.Set(func);
            transitions.Create(transition);
            return transition;
        }

        #endregion

        #region State

        public BaseMgrObj<FSMDState> states = new BaseMgrObj<FSMDState>();

        public FSMDState CreateState(FSMDBaseAction enter, FSMDBaseAction update, FSMDBaseAction exit, FSMDBaseTransition transition)
        {
            FSMDState state = CPoolManager.Instance.Pop<FSMDState>();
            state.Set(enter, update, exit, transition);
            states.Create(state);
            return state;
        }

        public FSMDState CreateState<T>(FSMDBaseAction enter, FSMDBaseAction update, FSMDBaseAction lateUpdate, FSMDBaseAction fixedUpdate, FSMDBaseAction exit, FSMDBaseTransition<T> transition)
        {
            FSMDState state = CPoolManager.Instance.Pop<FSMDState>();
            state.Set(enter, update, lateUpdate, fixedUpdate, exit, transition);
            states.Create(state);
            return state;
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
