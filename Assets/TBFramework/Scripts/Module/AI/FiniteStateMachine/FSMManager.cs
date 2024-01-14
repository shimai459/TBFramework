using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.AI.FSM
{
    public class FSMManager : Singleton<FSMManager>
    {
        private Dictionary<string,FSMLogic> aiDic=new Dictionary<string, FSMLogic>();

        public void AddAILogic(string aiName,FSMLogic ai){
            if(!aiDic.ContainsKey(aiName)){
                aiDic.Add(aiName,ai);
            }
        }

        public void RemoveAILogic(string aiName){
            if(aiDic.ContainsKey(aiName)){
                aiDic.Remove(aiName);
            }
        }

        public FSMLogic GetAILogic(string aiName){
            if(aiDic.ContainsKey(aiName)){
                return aiDic[aiName];
            }
            return null;
        }
    }
}
