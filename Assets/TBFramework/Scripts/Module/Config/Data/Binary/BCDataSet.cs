using System.IO;
using UnityEngine;

namespace TBFramework.Config.Data.Binary
{
    public class BCDataSet
    {
        #region 二进制
        
        /// <summary>
        /// 由Excel文件自动生成的二进制数据存储文件的路径
        /// </summary>
        /// <returns></returns>
        public readonly static string BINARY_DATA_EXCEL_PATH=Path.Combine(Application.streamingAssetsPath,"BinaryDataByExcel");

        /// <summary>
        /// 存储的由Excel文件生成的二进制文件的后缀名
        /// </summary>
        public readonly static string BINARY_EXTENSION=".configdata";
        
        #endregion
    }
}
