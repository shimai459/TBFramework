
using UnityEngine;

namespace TBFramework.Util
{
    public class MathUtil
    {
        public static float Deg2Rad(float deg)
        {
            return deg * Mathf.Deg2Rad;
        }

        public static float Rad2Deg(float rad)
        {
            return rad * Mathf.Rad2Deg;
        }

        public static float DistanceXY2D(Vector3 srcPos, Vector3 targetPos)
        {
            srcPos.z = 0;
            targetPos.z = 0;
            return Vector3.Distance(srcPos, targetPos);
        }

        public static float DistanceXZ2D(Vector3 srcPos, Vector3 targetPos)
        {
            srcPos.y = 0;
            targetPos.y = 0;
            return Vector3.Distance(srcPos, targetPos);
        }

        public static bool IsWorldPosInScreenMain(Vector3 worldPos)
        {
            return IsWorldPosInScreen(worldPos, Camera.main);
        }

        public static bool IsWorldPosInScreen(Vector3 worldPos, Camera camera)
        {
            Vector2 screenPos = camera.WorldToScreenPoint(worldPos);
            if (screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height)
            {
                return true;
            }
            return false;
        }
    }
}
