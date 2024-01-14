using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Data;
using System.Xml;
using UnityEditor;

namespace TBFramework.Config.Data.Xml
{
    public class ExcelToXConfig : MonoBehaviour
    {
        [MenuItem("Automation/GenerateExcel/XML")]
        public static void GenerateExcelToXml(){
            List<DataTable> tables = AutoExcelClass.GenerateExcelClass();
            foreach(DataTable table in tables){
                GenerateExcelToXml(table);
            }
        }
        /// <summary>
        /// 生成表对应的数据文件,格式为XML
        /// </summary>
        /// <param name="table">要生成数据文件的表</param>
        private static void GenerateExcelToXml(DataTable table){
            if(!Directory.Exists(XCDataSet.XML_DATA_EXCEL_PATH)){
                Directory.CreateDirectory(XCDataSet.XML_DATA_EXCEL_PATH);
            }
            string filePath=Path.Combine(XCDataSet.XML_DATA_EXCEL_PATH,table.TableName+".xml");
            //创建文件
            XmlDocument xml=new XmlDocument();
            //添加固定脚本信息
            XmlDeclaration XD=xml.CreateXmlDeclaration("1.0","UTF-8","");
            xml.AppendChild(XD);
            //添加根节点(根节点有两个属性,表示主键名和数据有几行)
            XmlElement root=xml.CreateElement(XCDataSet.XML_DATA_EXCEL_ROOT_NAME);
            //添加数据行数信息属性
            root.SetAttribute(XCDataSet.XML_DATA_EXCEL_ROW_INFO_NAME,(table.Rows.Count-ExcelSet.BEGIN_DATA_ROW).ToString());
            //添加主键名属性
            List<int> keysIndex=AutoExcelClass.GetKeysIndex(table);
            string keyName="";
            for(int i=0;i<keysIndex.Count;i++){
                if(i>0){
                    keyName+=CDataSet.CONTAINER_MULTIPLE_KEY_SEPARATOR; 
                }
                keyName+=table.Rows[ExcelSet.VARIABLE_NAME_ROW][AutoExcelClass.GetKeysIndex(table)[i]].ToString();
            }
            root.SetAttribute(XCDataSet.XML_DATA_EXCEL_KEY_NAME,keyName);
            xml.AppendChild(root);

            //添加数据子节点
            DataRow row;  
            DataRow nameRow=table.Rows[ExcelSet.VARIABLE_NAME_ROW];
            for(int i=ExcelSet.BEGIN_DATA_ROW;i<table.Rows.Count;i++){
                row=table.Rows[i];
                XmlElement dataItem=xml.CreateElement(table.TableName);
                for(int j=0;j<table.Columns.Count;j++){
                    dataItem.SetAttribute(nameRow[j].ToString(),row[j].ToString());
                }
                root.AppendChild(dataItem);
            }
            //将上面的xml存储到文件中
            xml.Save(filePath);
            //刷新界面
            AssetDatabase.Refresh();
        }
    }
}
