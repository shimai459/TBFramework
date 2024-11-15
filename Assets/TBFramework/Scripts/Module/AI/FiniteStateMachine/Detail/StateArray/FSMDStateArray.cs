
using System.Collections.Generic;
using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{

    public class FSMDStateArray<V> : FSMDBaseStateArray
    {
        private Dictionary<V, FSMDState<V>> states = new Dictionary<V, FSMDState<V>>();

        public void Set(params (V stateKey, FSMDState<V> state)[] states)
        {
            this.states.Clear();
            AddStates(states);
        }

        public bool Have(V key)
        {
            return states.ContainsKey(key);
        }

        public bool Have(FSMDState<V> state)
        {
            return states.ContainsValue(state);
        }

        public FSMDState<V> Get(V key)
        {
            if (states.ContainsKey(key))
            {
                return states[key];
            }
            return null;
        }

        public void AddState(V key, FSMDState<V> state)
        {
            if (!states.ContainsKey(key))
            {
                states.Add(key, state);
            }
        }

        public void AddStates((V stateKey, FSMDState<V> state)[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                AddState(states[i].stateKey, states[i].state);
            }
        }

        public void RemoveState(V key)
        {
            if (states.ContainsKey(key))
            {
                if (states[key] != null)
                {
                    FSMDManager.Instance.states.Destory(states[key].key);
                }
                states.Remove(key);
            }
        }

        public void RemoveStates(V[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                RemoveState(keys[i]);
            }
        }

        public void RemoveState(FSMDState<V> state)
        {
            if (states.ContainsValue(state))
            {
                foreach (var item in states)
                {
                    if (item.Value == state)
                    {
                        FSMDManager.Instance.states.Destory(state.key);
                        states.Remove(item.Key);
                        break;
                    }
                }
            }
        }

        public void RemoveStates(FSMDState<V>[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                RemoveState(states[i]);
            }
        }

        public void Clear()
        {
            foreach (FSMDState<V> state in states.Values)
            {
                FSMDManager.Instance.states.Destory(state.key);
            }
            states.Clear();
        }

        public override void Reset()
        {
            base.Reset();
            key = default(int);
            this.Clear();
        }
    }
}
