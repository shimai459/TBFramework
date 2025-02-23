using System.Collections.Generic;
using TBFramework.Mono;
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Simple
{
    public class FSMSLogicArray<T> : FSMSBaseLogic<T>
    {

        #region 初始化

        public void Set(T defaultLogic, params (T key, FSMSBaseLogic logic)[] logics)
        {
            this.AddLogics(logics);
            if (this.logics.ContainsKey(defaultLogic))
            {
                this.defaultLogic = defaultLogic;
                this.currentLogic = defaultLogic;
            }
        }

        public override void SetContext(BaseContext context)
        {
            base.SetContext(context);
            foreach (var logic in logics)
            {
                logic.Value.SetContext(context);
            }
        }

        #endregion

        #region 运行操作

        private bool isAddListen = false;

        public override void StartLogic()
        {
            if (!isAddListen)
            {
                MonoConManager.Instance.AddUpdateListener(Update);
                isAddListen = true;
            }
        }

        public override void StopLogic()
        {
            MonoConManager.Instance.RemoveUpdateListener(Update);
            isAddListen = false;
        }

        #endregion

        #region LogicDic操作

        private Dictionary<T, FSMSBaseLogic> logics = new Dictionary<T, FSMSBaseLogic>();

        public FSMSBaseLogic GetLogic(T key)
        {
            if (logics.ContainsKey(key))
            {
                return logics[key];
            }
            return default(FSMSBaseLogic);
        }


        public void AddLogic(T key, FSMSBaseLogic logic)
        {
            if (!logics.ContainsKey(key) && KeyBase.IsLegal(logic))
            {
                logics.Add(key, logic);
                FSMSManager.Instance.logics.AddUse(logic);
                if (context != null)
                {
                    logic.SetContext(context);
                }
            }
        }

        public void AddLogics(params (T key, FSMSBaseLogic logic)[] logics)
        {
            foreach (var logic in logics)
            {
                AddLogic(logic.key, logic.logic);
            }
        }

        public void ChangeLogic(T key, FSMSBaseLogic logic)
        {
            RemoveLogic(key);
            AddLogic(key, logic);
        }

        public void ChangeLogics(params (T key, FSMSBaseLogic logic)[] logics)
        {
            foreach (var logic in logics)
            {
                ChangeLogic(logic.key, logic.logic);
            }
        }

        public void RemoveLogic(T key)
        {
            if (logics.ContainsKey(key))
            {
                FSMSManager.Instance.logics.Destory(logics[key].key);
                logics.Remove(key);
            }
        }

        public void RemoveLogics(params T[] keys)
        {
            foreach (var key in keys)
            {
                RemoveLogic(key);
            }
        }

        public void RemoveLogic(FSMSBaseLogic logic)
        {
            if (logics.ContainsValue(logic))
            {
                foreach (var item in logics)
                {
                    if (item.Value == logic)
                    {
                        FSMSManager.Instance.states.Destory(logic.key);
                        logics.Remove(item.Key);
                        break;
                    }
                }
            }
        }

        public void RemoveLogics(FSMSBaseLogic[] logics)
        {
            for (int i = 0; i < logics.Length; i++)
            {
                RemoveLogic(logics[i]);
            }
        }

        public void Clear()
        {
            foreach (var logic in logics)
            {
                FSMSManager.Instance.logics.Destory(logic.Value.key);
            }
            logics.Clear();
        }

        #endregion

        #region 运行具体逻辑

        private void Update()
        {
            if (currentLogic != null && logics.ContainsKey(currentLogic))
            {
                if (logics[currentLogic] != null && logics[currentLogic] is FSMSBaseLogic<T>)
                {
                    FSMSBaseLogic<T> logic = logics[currentLogic] as FSMSBaseLogic<T>;
                    if (logic.func != null)
                    {
                        T newState = logic.func(context);
                        Change(newState);
                    }

                }
            }
        }

        #endregion

        #region 变更状态

        private T currentLogic;
        private T defaultLogic;
        private T previousLogic;

        public void Change(T logic)
        {
            if (!logic.Equals(default(T)) && !logic.Equals(currentLogic) && logics.ContainsKey(logic))
            {
                logics[currentLogic].StopLogic();
                previousLogic = currentLogic;
                currentLogic = logic;
                logics[currentLogic].StartLogic();
                logics[currentLogic].ToDefault();
            }
        }

        public override void ToDefault()
        {
            Change(defaultLogic);
        }

        public void ToPrevious()
        {
            Change(previousLogic);
        }

        #endregion

        #region 重置
        public override void Reset()
        {
            base.Reset();
            this.StopLogic();
            this.Clear();
            currentLogic = default(T);
            previousLogic = default(T);
            defaultLogic = default(T);
        }

        #endregion

    }
}