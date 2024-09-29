using UnityEngine;

namespace TBFramework.Input
{
    public class KeySingleData
    {
        public KeyCode keyCode;

        public E_KeyOperationType operationType;

        public KeySingleData(KeyCode keyCode, E_KeyOperationType operationType)
        {
            this.keyCode = keyCode;
            this.operationType = operationType;
        }

        public bool IsTrigger()
        {
            switch (this.operationType)
            {
                case E_KeyOperationType.Down:
                    return UnityEngine.Input.GetKeyDown(keyCode);
                case E_KeyOperationType.Up:
                    return UnityEngine.Input.GetKeyUp(keyCode);
                case E_KeyOperationType.Hold:
                    return UnityEngine.Input.GetKey(keyCode);
                case E_KeyOperationType.AnyKeyDown:
                    return UnityEngine.Input.anyKeyDown;
                case E_KeyOperationType.AnyKeyHold:
                    return UnityEngine.Input.anyKey;
            }
            return false;
        }

        public bool Compare(KeySingleData other)
        {
            return keyCode == other.keyCode && operationType == other.operationType;
        }
    }
}