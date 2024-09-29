


using UnityEngine;

namespace TBFramework.Input
{
    public class MouseSingleData
    {
        private int index;

        private E_MouseNumType type;

        public E_MouseNumType Type { get => type; }

        public MouseSingleData(int index, E_MouseNumType type)
        {
            this.index = index;
            this.type = type;
        }

        public Vector2 Value()
        {
            switch (type)
            {
                case E_MouseNumType.Float:
                    return UnityEngine.Input.mouseScrollDelta;
                case E_MouseNumType.Position:
                    return UnityEngine.Input.mousePosition;
                case E_MouseNumType.Down_Bool:
                    return UnityEngine.Input.GetMouseButtonDown(index) ? Vector2.one : Vector2.zero;
                case E_MouseNumType.Up_Bool:
                    return UnityEngine.Input.GetMouseButtonUp(index) ? Vector2.one : Vector2.zero;
                case E_MouseNumType.Hold_Bool:
                    return UnityEngine.Input.GetMouseButton(index) ? Vector2.one : Vector2.zero;
            }
            return Vector2.zero;
        }

        public bool Compare(MouseSingleData other)
        {
            return index == other.index && type == other.type;
        }
    }
}