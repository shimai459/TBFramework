using System.IO;
using UnityEngine;

namespace TBFramework.Config.Data.Xml
{
    public static class XCDataSet
    {
        #region XML
        
        /// <summary>
        /// 由Excel文件自动生成的XML数据存储文件的路径
        /// </summary>
        /// <returns></returns>
        public readonly static string XML_DATA_EXCEL_PATH=Path.Combine(Application.streamingAssetsPath,"XMLDataByExcel");
        
        /// <summary>
        /// 由Excel文件自动生成的XML文件的根节点名字
        /// </summary>
        public readonly static string XML_DATA_EXCEL_ROOT_NAME="Root";
        
        /// <summary>
        /// 由Excel文件自动生成的XML文件的数据有多少行信息的属性名
        /// </summary>
        public readonly static string XML_DATA_EXCEL_ROW_INFO_NAME="RowInfo";
        
        /// <summary>
        /// 由Excel文件自动生成的XML文件的数据主键名字信息的属性名
        /// </summary>
        public readonly static string XML_DATA_EXCEL_KEY_NAME="KeyName";
        
        /// <summary>
        /// 由Excel文件自动生成的XML文件的数据存储变量名的总元素名
        /// </summary>
        public readonly static string XML_DATA_EXCEL_VARIABLE_ROOT_NAME="VariableRoot";
        
        /// <summary>
        /// 由Excel文件自动生成的XML文件的数据存储变量名的元素名
        /// </summary>
        public readonly static string XML_DATA_EXCEL_VARIABLE_NAME="VariableName";

        

        #endregion

    }
}