using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{
    public class FSMDState<V> : FSMDBaseState
    {
        public FSMDBaseAction enter;

        public FSMDBaseAction update;

        public FSMDBaseAction lateUpdate;

        public FSMDBaseAction fixedUpdate;

        public FSMDBaseAction exit;

        public FSMDBaseTransition<V> transition;

        public void Set(FSMDBaseAction enter, FSMDBaseAction update, FSMDBaseAction exit, FSMDBaseTransition<V> transition)
        {
            this.enter = enter;
            this.update = update;
            this.exit = exit;
            this.transition = transition;
        }

        public void Set(FSMDBaseAction enter, FSMDBaseAction update, FSMDBaseAction lateUpdate, FSMDBaseAction fixedUpdate, FSMDBaseAction exit, FSMDBaseTransition<V> transition)
        {
            this.enter = enter;
            this.update = update;
            this.exit = exit;
            this.lateUpdate = lateUpdate;
            this.fixedUpdate = fixedUpdate;
            this.transition = transition;
        }

        public override void Reset()
        {
            base.Reset();
            if (enter != null)
            {
                FSMDManager.Instance.actions.Destory(enter.key);
            }
            if (exit != null)
            {
                FSMDManager.Instance.actions.Destory(exit.key);
            }
            if (update != null)
            {
                FSMDManager.Instance.actions.Destory(update.key);
            }
            if (lateUpdate != null)
            {
                FSMDManager.Instance.actions.Destory(lateUpdate.key);
            }
            if (fixedUpdate != null)
            {
                FSMDManager.Instance.actions.Destory(fixedUpdate.key);
            }
            if (transition != null)
            {
                FSMDManager.Instance.transitions.Destory(transition.key);
            }
            enter = null;
            update = null;
            lateUpdate = null;
            fixedUpdate = null;
            exit = null;
            transition = null;
        }

    }
}
