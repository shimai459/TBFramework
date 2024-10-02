using System.Collections.Generic;

namespace TBFramework.Util
{
    public class UniqueKeyUtil
    {
        public static int GetUnusedKey(List<int> uniqueKeys)
        {
            uniqueKeys.Sort();
            int key = 1;
            foreach (int use in uniqueKeys)
            {
                if (use != key)
                {
                    break;
                }
                key++;
            }
            return key;
        }
    }
}
