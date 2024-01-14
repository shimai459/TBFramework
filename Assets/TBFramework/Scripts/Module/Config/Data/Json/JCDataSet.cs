using System.IO;
using UnityEngine;
namespace TBFramework.Config.Data.Json
{

    public class JCDataSet
    {
        #region Json
        
        /// <summary>
        /// 由Excel文件自动生成的Json数据存储文件的路径
        /// </summary>
        /// <returns></returns>
        public readonly static string JSON_DATA_EXCEL_PATH=Path.Combine(Application.streamingAssetsPath,"JsonDataByExcel");
        
        #endregion
    }
}