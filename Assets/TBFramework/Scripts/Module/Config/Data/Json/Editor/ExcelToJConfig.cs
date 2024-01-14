using System.Data;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TBFramework.Config.Data.Json
{
    public class ExcelToJConfig : MonoBehaviour
    {
        [MenuItem("Automation/GenerateExcel/Json")]
        public static void GenerateExcelToJson(){
            List<DataTable> tables = AutoExcelClass.GenerateExcelClass();
            foreach(DataTable table in tables){
                GenerateExcelToJson(table);
            }
        }
         /// <summary>
        /// 生成表对应的数据文件,格式为Json
        /// </summary>
        /// <param name="table">要生成数据文件的表</param>
        private static void GenerateExcelToJson(DataTable table){
            if(!Directory.Exists(JCDataSet.JSON_DATA_EXCEL_PATH)){
                Directory.CreateDirectory(JCDataSet.JSON_DATA_EXCEL_PATH);
            }
            string filePath=Path.Combine(JCDataSet.JSON_DATA_EXCEL_PATH,table.TableName+".json");
            Debug.LogWarning("方法未实现,不能用于生成Json文件,只能生成数据结构类和数据容器类!");
            //TODO
        }
    }
}
