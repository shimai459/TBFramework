using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBFramework.Mono;

/// <summary>
/// AI逻辑基类
/// </summary>
namespace TBFramework.AI.FSM
{
    public class FSMLogic
    {
        public Dictionary<string,FSMState> stateDic=new Dictionary<string, FSMState>();
        public FSMState nowState;

        public void ChangeState(string name){
            if(stateDic.ContainsKey(name)){
                MonoManager.Instance.RemoveUpdateListener(nowState.UpdateState);
                nowState.ExitState();
                nowState=stateDic[name];
                nowState.EnterState();
                MonoManager.Instance.AddUpdateListener(nowState.UpdateState);
            }
        }
    }
}
