using System.IO;

namespace TBFramework.Pool
{

    public class PoolSet
    {
        #region Pool(缓存池)
            /// <summary>
            /// 缓存池的预制体保存路径
            /// </summary>
            /// <returns></returns>
            public static string POOL_PREFABS_PATH=Path.Combine("Prefabs","Pool");
            /// <summary>
            /// 缓存池根对象的名字
            /// </summary>
            public static string POOL_OBJECT_NAME="Pool";
            /// <summary>
            /// 每个缓存池父物体名字的后缀
            /// </summary>
            public static string POOL_SINGLE_PARENT_EXTENSION="Pool Parent";
            /// <summary>
            /// 默认缓存池中物体的数量
            /// </summary>
            public static int POOL_MAX_NUMBER=1000;
        
        #endregion
    }
}
