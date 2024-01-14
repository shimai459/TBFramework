using System.Xml;
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Excel;
using TBFramework.Config.Data.Xml;
using TBFramework.Config.Data.Binary;
using TBFramework.Config.Data.Json;

namespace TBFramework.Config.Data
{
    public static class AutoExcelClass
    {
        
        /// <summary>
        /// 检测是否有Excel文件,有就生成对应的数据结构类,数据容器类和数据存储文件
        /// </summary>
        /// <returns></returns>
        public static List<DataTable> GenerateExcelClass(){
            //获取存储Excel的文件夹
            DirectoryInfo dInfo=Directory.CreateDirectory(ExcelSet.DATA_EXCEL_PATH);
            //获取文件夹下所有文件
            FileInfo[] infos=dInfo.GetFiles();
            //判断文件夹下是否有文件,没有直接返回,并警告
            if(infos.Length==0){
                Debug.LogWarning("Excel文件夹下没有文件");
            }
            //数据表容器
            DataTableCollection tableCollection;
            //使用List存储所有的表
            List<DataTable> tables=new List<DataTable>();
            //遍历每一个文件
            for(int i=0;i<infos.Length;i++){
                //如果文件的后缀不为Excel文件的后缀,就直接跳过该文件
                if(infos[i].Extension!=".xlsx"&&infos[i].Extension!=".xls"){
                    continue;
                }
                //打开一个Excel文件,获取其中所有表的数据
                using(FileStream fs=infos[i].Open(FileMode.Open,FileAccess.Read)){
                    //获取Excel表数据的固定写法
                    IExcelDataReader excelDataReader=ExcelReaderFactory.CreateOpenXmlReader(fs);
                    tableCollection=excelDataReader.AsDataSet().Tables;
                    fs.Close();
                }
                //遍历该Excel文件中的所有表,去为每个表创建数据结构类和数据容器类
                foreach(DataTable table in tableCollection){
                    //生成表的数据结构类
                    GenerateExcelDataClass(table);
                    //生成表的数据容器类
                    GenerateExcelContainer(table);
                    tables.Add(table);
                }
            }
            return tables;
        }

        /// <summary>
        /// 生成表对应的数据结构类
        /// </summary>
        /// <param name="table">要生成数据结构类的表</param>
        private static void GenerateExcelDataClass(DataTable table){
            //判断是否存在存放数据结构类的文件夹,没有就生成
            if(!Directory.Exists(ExcelSet.DATA_CLASS_PATH)){
                Directory.CreateDirectory(ExcelSet.DATA_CLASS_PATH);
            }
            //获取字段名行
            DataRow nameRow=table.Rows[ExcelSet.VARIABLE_NAME_ROW];
            //获取字段类型行
            DataRow typeRow=table.Rows[ExcelSet.VARIABLE_TYPE_ROW];
            //数据结构类的第一句
            string str="public class "+table.TableName+"\n{\n";
            //数据结构类中的变量声明
            for(int i=0;i<table.Columns.Count;i++){
                str+="\tpublic "+typeRow[i].ToString()+" "+nameRow[i].ToString()+";\n";
            }
            //数据结构类中最后的}
            str+="}";
            //将上述数据结构类写入文件中
            File.WriteAllText(Path.Combine(ExcelSet.DATA_CLASS_PATH,table.TableName+".cs"),str);

            //刷新界面
            AssetDatabase.Refresh();
        } 

        /// <summary>
        /// 生成表对应的数据容器类
        /// </summary>
        /// <param name="table">要生成数据容器类的表</param>
        private static void GenerateExcelContainer(DataTable table){
            //判断是否存在存放数据容器类的文件夹,没有就生成
            if(!Directory.Exists(ExcelSet.DATA_CONTAINER_PATH)){
                Directory.CreateDirectory(ExcelSet.DATA_CONTAINER_PATH);
            }
            //获取主键的类型
            List<int> keysIndex=GetKeysIndex(table);
            string keyType="";
            if(keysIndex.Count==1){
                keyType=table.Rows[ExcelSet.VARIABLE_TYPE_ROW][keysIndex[0]].ToString();
            }else{
                keyType="string";
            }
            //数据容器类的第一行(引用的数据集信息)
            string str="using System.Collections.Generic;\n";
            //数据容器类的第一行(类名的声明)
            str+="public class "+table.TableName+CDataSet.DATA_CONTAINER_EXTENSION+"\n{\n";
            //数据容器类第三行(类中容器字段声明)
            str+="\tpublic Dictionary<"+keyType+","+table.TableName+">"+CDataSet.DATA_CONTAINER_NAME+"=new Dictionary<"+keyType+","+table.TableName+">();\n";
            //数据容器类第四行(结束})
            str+="}";

            //将上方拼接的数据容器类写入文件中
            File.WriteAllText(Path.Combine(ExcelSet.DATA_CONTAINER_PATH,table.TableName+CDataSet.DATA_CONTAINER_EXTENSION+".cs"),str);

            //刷新界面
            AssetDatabase.Refresh();


        }

        /// <summary>
        /// 获取主键所在列的索引
        /// </summary>
        /// <param name="table">查找主键索引的表</param>
        /// <returns></returns>
        public static List<int> GetKeysIndex(DataTable table){
            List<int> indexs=new List<int>();
            DataRow keyRow=table.Rows[ExcelSet.KEY_ROW];
            string keyStr;
            for(int i=0;i<table.Columns.Count;i++){
                keyStr=keyRow[i].ToString().ToLower();
                if(keyStr.Equals(ExcelSet.KEY_NAME)){
                    indexs.Add(i);
                }
            }
            return indexs;
        }
    }
}