using System.Collections.Generic;
using TBFramework.Mono;
using TBFramework.Pool;

/// <summary>
/// AI逻辑基类
/// </summary>
namespace TBFramework.AI.FSM.Simple
{
    public class FSMSLogic : CBase
    {
        public Dictionary<string, FSMSState> stateDic = new Dictionary<string, FSMSState>();
        public FSMSState nowState;

        public void AddState(string name, FSMSState state)
        {
            if (!stateDic.ContainsKey(name))
            {
                stateDic.Add(name, state);
            }
        }

        public void RemoveState(string name)
        {
            if (stateDic.ContainsKey(name))
            {
                stateDic.Remove(name);
            }
        }

        public void ChangeState(string name)
        {
            if (stateDic.ContainsKey(name))
            {
                MonoConManager.Instance.RemoveUpdateListener(nowState.UpdateState);
                MonoConManager.Instance.RemoveUpdateListener(nowState.CheckChange);
                nowState.ExitState();
                nowState = stateDic[name];
                nowState.EnterState();
                MonoConManager.Instance.AddUpdateListener(nowState.UpdateState);
                MonoConManager.Instance.AddUpdateListener(nowState.CheckChange);
            }
        }

        public override void Reset()
        {
            this.stateDic.Clear();
            this.nowState = null;
        }
    }
}
