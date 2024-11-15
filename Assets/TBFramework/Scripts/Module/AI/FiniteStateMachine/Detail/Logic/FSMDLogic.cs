using TBFramework.Mono;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDLogic<T, V> : FSMDBaseLogic
    {
        private FSMDContext<T> context;
        private FSMDStateArray<V> states;
        private V currentState;
        private V defaultState;
        private V previousState;

        public FSMDLogic()
        {
            MonoConManager.Instance.AddUpdateListener(Update);
            MonoConManager.Instance.AddLateUpdateListener(LateUpdate);
            MonoConManager.Instance.AddFixedUpdateListener(FixedUpdate);
        }

        public void Set(FSMDContext<T> context, FSMDStateArray<V> states, V defaultState)
        {
            this.context = context;
            this.states = states;
            if (states.Have(defaultState))
            {
                this.defaultState = defaultState;
                this.currentState = defaultState;
                states.Get(defaultState)?.enter?.Invoke(context);
            }
        }

        private void Update()
        {
            if (currentState != null && states.Have(currentState))
            {
                if (states.Get(currentState).transition != null)
                {
                    V newState = states.Get(currentState).transition.Change(context);
                    ChangeState(newState);
                }

                states.Get(currentState)?.update?.Invoke(context);
            }
        }

        private void LateUpdate()
        {
            if (currentState != null && states.Have(currentState))
            {
                states.Get(currentState)?.lateUpdate?.Invoke(context);
            }
        }

        private void FixedUpdate()
        {
            if (currentState != null && states.Have(currentState))
            {
                states.Get(currentState)?.fixedUpdate?.Invoke(context);
            }
        }

        public void ChangeState(V state)
        {
            if (!state.Equals(default(V)) && !state.Equals(currentState) && states.Have(state))
            {
                states.Get(currentState)?.exit?.Invoke(context);
                previousState = currentState;
                currentState = state;
                states.Get(currentState)?.enter?.Invoke(context);
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

        public FSMDContext<T> GetContext()
        {
            return context;
        }

        public void SetContextOneValue(string valueName, object value)
        {
            context.SetValue(valueName, value);
        }

        public override void Reset()
        {
            base.Reset();
            MonoConManager.Instance.RemoveUpdateListener(Update);
            if (context != null)
                FSMDManager.Instance.contexts.Destory(context.key);
            if (states != null)
                FSMDManager.Instance.stateArrays.Destory(states.key);
            context = default(FSMDContext<T>);
            states = default(FSMDStateArray<V>);
            currentState = default(V);
            previousState = default(V);
            defaultState = default(V);
        }

    }
}