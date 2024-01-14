namespace TBFramework.Config.Data
{
    public static class CDataSet 
    {
        /// <summary>
        /// 有多个值组成一个主键时,多个值之间的分隔符
        /// </summary>
        public readonly static char CONTAINER_MULTIPLE_KEY_SEPARATOR='_';

        #region 生成类的信息
        
        /// <summary>
        /// 创建的数据容器类中数据容器名
        /// </summary>
        public readonly static string DATA_CONTAINER_NAME="dataDic";
        
        /// <summary>
        /// 创建的容器类名的后缀
        /// </summary>
        public readonly static string DATA_CONTAINER_EXTENSION="Container";
        
        #endregion
    }
}