using System.IO;
using UnityEngine;

namespace TBFramework.Data.Json
{
    public class JDataSet
    {
        /// <summary>
        /// Json格式数据存储文件存储的路径
        /// </summary>
        /// <returns></returns>
        public readonly static string JSON_DATA_PATH=Path.Combine(Application.persistentDataPath,"JsonData");
        
        /// <summary>
        /// Json数据配置文件的存储位置
        /// </summary>
        /// <returns></returns>
        public readonly static string JSON_DATA_CONFIGURATION_PATH=Path.Combine(Application.streamingAssetsPath,"JsonData");
    }
}