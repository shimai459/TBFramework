using System;
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Simple
{
    public class FSMSManager : Singleton<FSMSManager>
    {
        #region Logic
        public BaseMgrObj<FSMSBaseLogic> logics = new BaseMgrObj<FSMSBaseLogic>();

        public FSMSLogic<T> CreateFSMS<T>(T defaultState, BaseContext context, bool isAddUseMeself = false, params (T key, FSMSState<T> state)[] states)
        {
            FSMSLogic<T> logic = CPoolManager.Instance.Pop<FSMSLogic<T>>();
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

        public FSMSLogic<T> CreateFSMS<T>(T defaultState, Func<BaseContext, T> func, bool isAddUseMeself = false, params (T key, FSMSState<T> state)[] states)
        {
            FSMSLogic<T> logic = CPoolManager.Instance.Pop<FSMSLogic<T>>();
            logic.Set(defaultState, states);
            logic.SetFunc(func);
            logic.StartLogic();
            logics.Create(logic);
            if (isAddUseMeself)
            {
                logics.AddUse(logic);
            }
            return logic;
        }

        public FSMSLogicArray<T> CreateFSMSArray<T>(T defaultState, BaseContext context, bool isAddUseMeself = false, params (T key, FSMSBaseLogic logic)[] logics)
        {
            FSMSLogicArray<T> logic = CPoolManager.Instance.Pop<FSMSLogicArray<T>>();
            logic.Set(defaultState, logics);
            logic.SetContext(context);
            logic.StartLogic();
            this.logics.Create(logic);
            if (isAddUseMeself)
            {
                this.logics.AddUse(logic);
            }
            return logic;
        }

        public FSMSLogicArray<T> CreateFSMSArray<T>(T defaultState, Func<BaseContext, T> func, bool isAddUseMeself = false, params (T key, FSMSBaseLogic logic)[] logics)
        {
            FSMSLogicArray<T> logic = CPoolManager.Instance.Pop<FSMSLogicArray<T>>();
            logic.Set(defaultState, logics);
            logic.SetFunc(func);
            this.logics.Create(logic);
            if (isAddUseMeself)
            {
                this.logics.AddUse(logic);
            }
            return logic;
        }

        #endregion

        #region State

        public BaseMgrObj<FSMSBaseState> states = new BaseMgrObj<FSMSBaseState>();

        public FSMSState<T> CreateFSMSState<T>(Action<BaseContext> enter, Action<BaseContext> update, Action<BaseContext> lateUpdate, Action<BaseContext> fixedUpdate, Action<BaseContext> exit, Func<BaseContext, T> change)
        {
            FSMSState<T> state = CPoolManager.Instance.Pop<FSMSState<T>>();
            state.Set(enter, update, lateUpdate, fixedUpdate, exit, change);
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
