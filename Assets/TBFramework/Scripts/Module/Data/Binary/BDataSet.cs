using System.IO;
using UnityEngine;

namespace TBFramework.Data.Binary
{
    public class BDataSet
    {
        /// <summary>
        /// 二进制格式数据存储文件存储的路径
        /// </summary>
        /// <returns></returns>
        public readonly static string BINARY_DATA_PATH=Path.Combine(Application.persistentDataPath,"BinaryData");
        
        /// <summary>
        /// 二进制数据配置文件的存储位置
        /// </summary>
        /// <returns></returns>
        public readonly static string BINARY_DATA_CONFIGURATION_PATH=Path.Combine(Application.streamingAssetsPath,"BinaryData");
        
        /// <summary>
        /// 存储为二进制文件时使用的异或加密的KEY
        /// </summary>
        public readonly static byte BINARY_DATA_ENCRYPT=9;

        /// <summary>
        /// 存储的二进制文件的后缀名
        /// </summary>
        public readonly static string BINARY_EXTENSION=".data";
    }
}