using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Data;
using System.Text;

namespace TBFramework.Config.Data.Binary
{
    public class ExcelToBConfig
    {
        /// <summary>
        /// 提供外部自动化生成Excel文件对应的数据结构类,数据容器类和数据文件(二进制)
        /// </summary>
        [MenuItem("Automation/GenerateExcel/Binary")]
        public static void GenerateExcelToBinary(){
            List<DataTable> tables = AutoExcelClass.GenerateExcelClass();
            foreach(DataTable table in tables){
                GenerateExcelToBinary(table);
            }
        }

        /// <summary>
        /// 生成表对应的数据文件,格式为Binary(二进制)
        /// </summary>
        /// <param name="table">要生成数据文件的表</param>
        private static void GenerateExcelToBinary(DataTable table){
            if(!Directory.Exists(BCDataSet.BINARY_DATA_EXCEL_PATH)){
                Directory.CreateDirectory(BCDataSet.BINARY_DATA_EXCEL_PATH);
            }
            using(FileStream fs=new FileStream(Path.Combine(BCDataSet.BINARY_DATA_EXCEL_PATH,table.TableName+BCDataSet.BINARY_EXTENSION),FileMode.OpenOrCreate,FileAccess.Write)){
                //1.先写入一共有多少行的数据
                fs.Write(BitConverter.GetBytes(table.Rows.Count-ExcelSet.BEGIN_DATA_ROW),0,4);
                //2.写入主键的变量名
                List<int> keysIndex=AutoExcelClass.GetKeysIndex(table);
                string keyName="";
                for(int i=0;i<keysIndex.Count;i++){
                    if(i>0){
                        keyName+=CDataSet.CONTAINER_MULTIPLE_KEY_SEPARATOR; 
                    }
                    keyName+=table.Rows[ExcelSet.VARIABLE_NAME_ROW][AutoExcelClass.GetKeysIndex(table)[i]].ToString();
                }
                byte[] bytes=Encoding.UTF8.GetBytes(keyName);
                //存储主键变量名的字节长度
                fs.Write(BitConverter.GetBytes(bytes.Length),0,4);
                //存储主键变量名的字节信息
                fs.Write(bytes,0,bytes.Length);

                //3.遍历表中所有数据,写入二进制文件中
                DataRow row;
                DataRow typeRow=table.Rows[ExcelSet.VARIABLE_TYPE_ROW];
                for(int i=ExcelSet.BEGIN_DATA_ROW;i<table.Rows.Count;i++){
                    row=table.Rows[i];
                    for(int j=0;j<table.Columns.Count;j++){
                        switch(typeRow[j].ToString()){
                            case "int":
                                fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())),0,sizeof(int));
                                break;
                            case "float":
                                fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())),0,sizeof(float));
                                break;
                            case "bool":
                                fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())),0,sizeof(bool));
                                break;
                            case "string":
                                bytes=Encoding.UTF8.GetBytes(row[j].ToString());
                                fs.Write(BitConverter.GetBytes(bytes.Length),0,sizeof(int));
                                fs.Write(bytes,0,bytes.Length);
                                break;
                            case "byte":
                                fs.Write(new byte[]{byte.Parse(row[j].ToString())},0,sizeof(byte));
                                break;
                            case "sbyte":
                                fs.Write(new byte[]{byte.Parse(row[j].ToString())},0,sizeof(sbyte));
                                break;
                            case "short":
                                fs.Write(BitConverter.GetBytes(short.Parse(row[j].ToString())),0,sizeof(short));
                                break;
                            case "ushort":
                                fs.Write(BitConverter.GetBytes(ushort.Parse(row[j].ToString())),0,sizeof(ushort));
                                break;
                            case "uint":
                                fs.Write(BitConverter.GetBytes(uint.Parse(row[j].ToString())),0,sizeof(uint));
                                break;
                            case "long":
                                fs.Write(BitConverter.GetBytes(long.Parse(row[j].ToString())),0,sizeof(long));
                                break;
                            case "ulong":
                                fs.Write(BitConverter.GetBytes(ulong.Parse(row[j].ToString())),0,sizeof(ulong));
                                break;
                            case "double":
                                fs.Write(BitConverter.GetBytes(double.Parse(row[j].ToString())),0,sizeof(double));
                                break;
                            case "decimal":
                                //因为BitConverter没有提供将decimal转成byte[]的方法,所以将decimal变成字符串进行转byte[]
                                bytes=Encoding.UTF8.GetBytes(row[j].ToString());
                                fs.Write(BitConverter.GetBytes(bytes.Length),0,sizeof(int));
                                fs.Write(bytes,0,bytes.Length);
                                break;
                            case "char":
                                fs.Write(BitConverter.GetBytes(char.Parse(row[j].ToString())),0,sizeof(char));
                                break;
                        }
                    }
                }
                fs.Close();
            }
            AssetDatabase.Refresh();
        }
    }
}
