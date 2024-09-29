

namespace TBFramework.Input
{
    public class AxisSingleData
    {
        public string axisName;

        public E_AxisNumType axisNumType;

        public AxisSingleData(string axisName, E_AxisNumType axisNumType = E_AxisNumType.Float)
        {
            this.axisName = axisName;
            this.axisNumType = axisNumType;
        }

        public float Value
        {
            get
            {
                switch (axisNumType)
                {
                    case E_AxisNumType.Float:
                        return UnityEngine.Input.GetAxis(axisName);
                    case E_AxisNumType.Int:
                        return UnityEngine.Input.GetAxisRaw(axisName);
                    case E_AxisNumType.Down_Bool:
                        return UnityEngine.Input.GetButtonDown(axisName) ? 1f : 0f;
                    case E_AxisNumType.Up_Bool:
                        return UnityEngine.Input.GetButtonUp(axisName) ? 1f : 0f;
                    case E_AxisNumType.Hold_Bool:
                        return UnityEngine.Input.GetButton(axisName) ? 1f : 0f;
                }
                return 0;
            }
        }

        public bool Value_Bool
        {
            get { return Value == 0f ? false : true; }
        }

        public bool Compare(AxisSingleData other)
        {
            return axisName == other.axisName && axisNumType == other.axisNumType;
        }
    }
}