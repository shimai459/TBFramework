using System.Xml;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TBFramework.Config.Data.Xml
{
    public class XCDataManager : Singleton<XCDataManager>
    {
        #region 用于存储读取自动化生成数据文件的方法
            /// <summary>
            /// 存储所有由Excel自动生成的xml文件的数据
            /// </summary>
            /// <typeparam name="string">数据容器类类名</typeparam>
            /// <typeparam name="object">具体的数据容器类</typeparam>
            /// <returns></returns>
            private Dictionary<string,object> tableDicX=new Dictionary<string, object>();
            
            /// <summary>
            /// 初始化tableDicX,将自动生成的所有XML数据文件都读到内存中
            /// </summary>
            public void InitTable(){
                FileInfo[] infos=Directory.CreateDirectory(XCDataSet.XML_DATA_EXCEL_PATH).GetFiles();
                if(infos.Length==0){
                    return;
                }
                for(int i=0;i<infos.Length;i++){
                    if(infos[i].Extension!=".xml"){
                        continue;
                    }
                    LoadTable(Type.GetType(infos[i].Name.Split('.')[0]+CDataSet.DATA_CONTAINER_EXTENSION),Type.GetType(infos[i].Name.Split('.')[0]));
                }
            }

            /// <summary>
            /// 把Excel文件自动生成的XML文件读到内存中
            /// </summary>
            /// <param name="containerType">数据容器类类型</param>
            /// <param name="classType">数据结构类类型</param>
            public object LoadTable(Type containerType,Type classType){
                if(!tableDicX.ContainsKey(containerType.Name)){
                    try{
                        XmlDocument xml=new XmlDocument();
                        xml.Load(Path.Combine(XCDataSet.XML_DATA_EXCEL_PATH,classType.Name+".xml"));
                        //获取根节点
                        XmlNode root=xml.SelectSingleNode(XCDataSet.XML_DATA_EXCEL_ROOT_NAME);
                        //获取数据有多少行
                        int count=int.Parse(root.Attributes[XCDataSet.XML_DATA_EXCEL_ROW_INFO_NAME].Value);
                        //获取主键名字
                        string keyName=root.Attributes[XCDataSet.XML_DATA_EXCEL_KEY_NAME].Value;
                        //获取所有

                        //创建容器对象
                        object containerObj=Activator.CreateInstance(containerType);
                        //获取数据结构类下的所有字段
                        FieldInfo[] infos=classType.GetFields();
                        XmlNodeList list=root.SelectNodes(classType.Name);
                        //为每个字段赋值
                        for(int i=0;i<count;i++){
                            object dataObj=Activator.CreateInstance(classType);
                            XmlNode node=list[i];
                            foreach(FieldInfo info in infos){
                                SetFieldValue(info,dataObj,node);
                            }
                            object dicObj=containerType.GetField(CDataSet.DATA_CONTAINER_NAME).GetValue(containerObj);
                            MethodInfo mInfo=dicObj.GetType().GetMethod("Add");
                            object keyValue=classType.GetField(keyName).GetValue(dataObj);
                            mInfo.Invoke(dicObj,new object[]{keyValue,dataObj});
                        }
                        tableDicX.Add(containerType.Name,containerObj);
                        
                    }catch(Exception e){
                        Debug.Log($"表{containerType.Name}加载失败:"+e.Message);
                        return null;
                    }
                }
                return tableDicX[containerType.Name];
            }

            /// <summary>
            /// 把Excel文件自动生成的XML文件读到内存中的泛型方法
            /// </summary>
            /// <typeparam name="T">数据容器类类型</typeparam>
            /// <typeparam name="K">数据结构类类型</typeparam>
            public T LoadTable<T,K>(){
                // try{
                //     XmlDocument xml=new XmlDocument();
                //     xml.Load(Path.Combine(DataSetting.XML_DATA_EXCEL_PATH,typeof(K).Name+".xml"));
                //     //获取根节点
                //     XmlNode root=xml.SelectSingleNode(DataSetting.XML_DATA_EXCEL_ROOT_NAME);
                //     //获取数据有多少行
                //     int count=int.Parse(root.Attributes[DataSetting.XML_DATA_EXCEL_ROW_INFO_NAME].Value);
                //     //获取主键名字
                //     string keyName=root.Attributes[DataSetting.XML_DATA_EXCEL_KEY_NAME].Value;
                //     //获取所有

                //     //获取容器类类型
                //     Type containerType=typeof(T);
                //     //创建容器类对象
                //     object containerObj=Activator.CreateInstance<T>();
                //     //获取数据结构类类型
                //     Type classType=typeof(K);
                //     //获取数据结构类下所有的字段信息
                //     FieldInfo[] infos=classType.GetFields();
                //     XmlNodeList list=root.SelectNodes(classType.Name);
                //     //为每个字段赋值
                //     for(int i=0;i<count;i++){
                //         object dataObj=Activator.CreateInstance(classType);
                //         XmlNode node=list[i];
                //         foreach(FieldInfo info in infos){
                //             XSetFieldValue(info,dataObj,node);
                //         }
                //         object dicObj=containerType.GetField(DataSetting.DATA_CONTAINER_NAME).GetValue(containerObj);
                //         MethodInfo mInfo=dicObj.GetType().GetMethod("Add");
                //         object keyValue=classType.GetField(keyName).GetValue(dataObj);
                //         mInfo.Invoke(dicObj,new object[]{keyValue,dataObj});
                //     }
                //     tableDicX.Add(containerType.Name,containerObj);
                //     return (T)containerObj;
                // }catch(Exception e){
                //     Debug.Log($"表{typeof(T).Name}加载失败:"+e.Message);
                // }
                // return default(T);
                
                return (T)LoadTable(typeof(T),typeof(K));


            }

            /// <summary>
            /// 设置每个字段中的值
            /// </summary>
            /// <param name="info">字段信息</param>
            /// <param name="dataObj">要设置值的对象</param>
            /// <param name="node">XML元素</param>
            private void SetFieldValue(FieldInfo info,object dataObj,XmlNode node){
                #region if else
                // if(TypeEquals(info.FieldType,typeof(int))){
                //     info.SetValue(dataObj,int.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(float))){
                //     info.SetValue(dataObj,float.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(bool))){
                //     info.SetValue(dataObj,bool.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(string))){
                //     info.SetValue(dataObj,node.Attributes[info.Name].Value);
                // }else if(TypeEquals(info.FieldType,typeof(byte))){
                //     info.SetValue(dataObj,byte.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(sbyte))){
                //     info.SetValue(dataObj,sbyte.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(short))){
                //     info.SetValue(dataObj,short.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(ushort))){
                //     info.SetValue(dataObj,ushort.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(uint))){
                //     info.SetValue(dataObj,uint.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(long))){
                //     info.SetValue(dataObj,long.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(ulong))){
                //     info.SetValue(dataObj,ulong.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(double))){
                //     info.SetValue(dataObj,double.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(char))){
                //     info.SetValue(dataObj,char.Parse(node.Attributes[info.Name].Value));
                // }else if(TypeEquals(info.FieldType,typeof(decimal))){
                //     info.SetValue(dataObj,decimal.Parse(node.Attributes[info.Name].Value));
                // }
                #endregion
                switch(info.GetValue(dataObj)){
                    case int value:
                        info.SetValue(dataObj,int.Parse(node.Attributes[info.Name].Value));
                        break;
                    case float value:
                        info.SetValue(dataObj,float.Parse(node.Attributes[info.Name].Value));
                        break;
                    case bool value:
                        info.SetValue(dataObj,bool.Parse(node.Attributes[info.Name].Value));
                        break;
                    case string value:
                        //info.SetValue(dataObj,node.Attributes[info.Name].Value);
                        break; 
                    case char value:
                        info.SetValue(dataObj,char.Parse(node.Attributes[info.Name].Value));
                        break;
                    case double value:
                        info.SetValue(dataObj,double.Parse(node.Attributes[info.Name].Value));
                        break;
                    case byte value:
                        info.SetValue(dataObj,byte.Parse(node.Attributes[info.Name].Value));
                        break;
                    case sbyte value:
                        info.SetValue(dataObj,sbyte.Parse(node.Attributes[info.Name].Value));
                        break;
                    case short value:
                        info.SetValue(dataObj,short.Parse(node.Attributes[info.Name].Value));
                        break;
                    case ushort value:
                        info.SetValue(dataObj,ushort.Parse(node.Attributes[info.Name].Value));
                        break;
                    case uint value:
                        info.SetValue(dataObj,uint.Parse(node.Attributes[info.Name].Value));
                        break;
                    case long value:
                        info.SetValue(dataObj,long.Parse(node.Attributes[info.Name].Value));
                        break;
                    case ulong value:
                        info.SetValue(dataObj,ulong.Parse(node.Attributes[info.Name].Value));
                        break;
                    case decimal value:
                        info.SetValue(dataObj,decimal.Parse(node.Attributes[info.Name].Value));
                        break;
                    default:
                        if(info.FieldType.Equals(typeof(string))){
                            info.SetValue(dataObj,node.Attributes[info.Name].Value);
                        }
                        break;
                }
            }
            
            /// <summary>
            /// 获取一个XML数据文件生成的数据容器类表
            /// </summary>
            /// <param name="type">数据容器类类型</param>
            /// <returns></returns>
            public object GetTable(Type type){
                if(tableDicX.ContainsKey(type.Name)){
                    return tableDicX[type.Name];
                }
                Debug.Log($"({type.Name})类数据未加载!");
                return null;
            }
            
            /// <summary>
            /// 获取一个XML数据文件生成的数据容器类表的泛型方法
            /// </summary>
            /// <typeparam name="T">数据容器类类型</typeparam>
            /// <returns></returns>
            public T GetTable<T>(){
                if(tableDicX.ContainsKey(typeof(T).Name)){
                    return (T)tableDicX[typeof(T).Name];
                }
                Debug.Log($"({typeof(T).Name})类数据未加载!");
                return default(T);
            }
            
            #endregion
    }
}
