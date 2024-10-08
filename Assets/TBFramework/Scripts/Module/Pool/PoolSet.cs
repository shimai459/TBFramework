using System.IO;

namespace TBFramework.Pool
{

    public class PoolSet
    {
        #region Pool(缓存池)
        /// <summary>
        /// 缓存池根对象的名字
        /// </summary>
        public const string POOL_OBJECT_NAME = "Pool";
        /// <summary>
        /// 每个缓存池父物体名字的后缀
        /// </summary>
        public const string POOL_SINGLE_PARENT_EXTENSION = "Pool Parent";
        /// <summary>
        /// 默认缓存池中物体的数量
        /// </summary>
        public const int POOL_MAX_NUMBER = 1000;
        /// <summary>
        /// 缓存池是否开启整理功能
        /// </summary> 
        public const bool POOL_FINISH_OPEN = true;

        #endregion
    }
}
