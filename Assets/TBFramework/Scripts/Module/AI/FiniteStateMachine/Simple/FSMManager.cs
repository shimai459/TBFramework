using System.Collections.Generic;

namespace TBFramework.AI.FSM.Simple
{
    public class FSMSManager : Singleton<FSMSManager>
    {
        private Dictionary<string, FSMSLogic> aiDic = new Dictionary<string, FSMSLogic>();

        public void AddAILogic(string aiName, FSMSLogic ai)
        {
            if (!aiDic.ContainsKey(aiName))
            {
                aiDic.Add(aiName, ai);
            }
        }

        public void RemoveAILogic(string aiName)
        {
            if (aiDic.ContainsKey(aiName))
            {
                aiDic.Remove(aiName);
            }
        }

        public FSMSLogic GetAILogic(string aiName)
        {
            if (aiDic.ContainsKey(aiName))
            {
                return aiDic[aiName];
            }
            return null;
        }
    }
}
