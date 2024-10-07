namespace TBFramework.Util
{
    public class TextUtil
    {
        public static string GetNumStr(int value, int length)
        {
            return value.ToString().PadLeft(length, '0');//另一种写法：value.ToString($"D{length}");
        }

        public static string GetDecimalStr(int value, int length)
        {
            return value.ToString($"F{length}");
        }
    }
}
