using System.Collections;
using System.IO;
using UnityEngine;

namespace TBFramework.Data.Xml
{
    public class XDataSet
    {
        /// <summary>
        /// XML格式数据存储文件存储的路径
        /// </summary>
        /// <returns></returns>
        public readonly static string XML_DATA_PATH=Path.Combine(Application.persistentDataPath,"XMLData");
        
        /// <summary>
        /// XML数据配置文件的存储位置
        /// </summary>
        /// <returns></returns>
        public readonly static string XML_DATA_CONFIGURATION_PATH=Path.Combine(Application.streamingAssetsPath,"XMLData");
    }
}
