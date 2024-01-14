using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TBFramework.Data.Binary;

namespace TBFramework.Config.Data.Binary
{
    public class BCDataManager:Singleton<BCDataManager>
    {
        /// <summary>
            /// 存储所有由Excel自动生成的二进制文件的数据
            /// </summary>
            /// <typeparam name="string">数据容器类类名</typeparam>
            /// <typeparam name="object">数据容器类的具体对象</typeparam>
            /// <returns></returns>
            private Dictionary<string,object> tableDic=new Dictionary<string, object>();
            
            /// <summary>
            /// 初始化tableDicB,将自动生成的所有二进制文件都读到内存中
            /// </summary>
            public void InitTable(){
                FileInfo[] infos=Directory.CreateDirectory(BCDataSet.BINARY_DATA_EXCEL_PATH).GetFiles();
                if(infos.Length==0){
                    return;
                }
                for(int i=0;i<infos.Length;i++){
                    if(infos[i].Extension!=BCDataSet.BINARY_EXTENSION){
                        continue;
                    }
                    LoadTable(Type.GetType(infos[i].Name.Split('.')[0]+CDataSet.DATA_CONTAINER_EXTENSION),Type.GetType(infos[i].Name.Split('.')[0]));
                }
            }
            
            /// <summary>
            /// 把Excel文件自动生成的二进制文件读到内存中
            /// </summary>
            /// <param name="containerType">数据容器类类型</param>
            /// <param name="classType">数据结构类类型</param>
            public object LoadTable(Type containerType,Type classType){
                if(!tableDic.ContainsKey(containerType.Name)){
                    try{
                        using (FileStream fs=File.Open(Path.Combine(BCDataSet.BINARY_DATA_EXCEL_PATH,classType.Name+BCDataSet.BINARY_EXTENSION),FileMode.Open,FileAccess.Read)){
                            byte[] bytes=new byte[fs.Length];
                            fs.Read(bytes,0,bytes.Length);
                            fs.Close();
                            //记录读取的字节数
                            int index=0;

                            //读取有多少行数据的信息
                            int count=BitConverter.ToInt32(bytes,index);
                            index+=sizeof(int);

                            //读取主键的名字
                            int keyNameLength=BitConverter.ToInt32(bytes,index);
                            index+=sizeof(int);
                            string[] keysName=Encoding.UTF8.GetString(bytes,index,keyNameLength).Split(CDataSet.CONTAINER_MULTIPLE_KEY_SEPARATOR);
                            index+=keyNameLength;

                            //创建容器类对象
                            object containerObj=Activator.CreateInstance(containerType);
                            //获取数据结构类下所有的字段信息
                            FieldInfo[] infos=classType.GetFields();

                            //读取每一行的信息
                            for(int i=0;i<count;i++){
                                //实例化一个数据结构类
                                object dataObj=Activator.CreateInstance(classType);
                                foreach(FieldInfo info in infos){
                                    //index=BSetFieldValue(info,dataObj,bytes,index);
                                    info.SetValue(dataObj,BDataManager.Instance.FromBytes(bytes,info.FieldType,ref index,true));
                                }
                                object dicObj=containerType.GetField(CDataSet.DATA_CONTAINER_NAME).GetValue(containerObj);
                                MethodInfo mInfo=dicObj.GetType().GetMethod("Add");
                                if(keysName.Length==1){
                                    object keyValue=classType.GetField(keysName[0]).GetValue(dataObj);
                                    mInfo.Invoke(dicObj,new object[]{keyValue,dataObj});
                                }else{
                                    string keys="";
                                    for(int j=0;j<keysName.Length;j++){
                                        if(j>0){
                                            keys+=CDataSet.CONTAINER_MULTIPLE_KEY_SEPARATOR;
                                        }
                                        keys+=classType.GetField(keysName[j]).GetValue(dataObj).ToString();
                                    }
                                    mInfo.Invoke(dicObj,new object[]{keys,dataObj});
                                }
                            }
                            tableDic.Add(containerType.Name,containerObj);
                            fs.Close();   
                        }
                    }catch(Exception e){
                        Debug.Log($"表{containerType.Name}加载失败:"+e.Message);
                        return null;
                    }
                }
                return tableDic[containerType.Name];
            }
            
            /// <summary>
            /// 把Excel文件自动生成的二进制文件读到内存中的泛型方法
            /// </summary>
            /// <typeparam name="T">数据容器类类型</typeparam>
            /// <typeparam name="K">数据结构类类型</typeparam>
            public T LoadTable<T,K>(){
                #region before
                // try{
                //     using (FileStream fs=File.Open(Path.Combine(DataSetting.BINARY_DATA_EXCEL_PATH,typeof(K).Name+DataSetting.BINARY_EXTENSION),FileMode.Open,FileAccess.Read)){
                //     byte[] bytes=new byte[fs.Length];
                //     fs.Read(bytes,0,bytes.Length);
                //     fs.Close();
                //     //记录读取的字节数
                //     int index=0;

                //     //读取有多少行数据的信息
                //     int count=BitConverter.ToInt32(bytes,index);
                //     index+=sizeof(int);

                //     //读取主键的名字
                //     int keyNameLength=BitConverter.ToInt32(bytes,index);
                //     index+=sizeof(int);
                //     string keyName=Encoding.UTF8.GetString(bytes,index,keyNameLength);
                //     index+=keyNameLength;

                //     //获取容器类类型
                //     Type containerType=typeof(T);
                //     //创建容器类对象
                //     object containerObj=Activator.CreateInstance<T>();
                //     //获取数据结构类类型
                //     Type classType=typeof(K);
                //     //获取数据结构类下所有的字段信息
                //     FieldInfo[] infos=classType.GetFields();

                //     //读取每一行的信息
                //     for(int i=0;i<count;i++){
                //         //实例化一个数据结构类
                //         object dataObj=Activator.CreateInstance<K>();
                //         foreach(FieldInfo info in infos){
                //             //index=BSetFieldValue(info,dataObj,bytes,index);
                //             info.SetValue(dataObj,FromBytes(bytes,info.FieldType,ref index));
                //         }
                //         object dicObj=containerType.GetField(DataSetting.DATA_CONTAINER_NAME).GetValue(containerObj);
                //         MethodInfo mInfo=dicObj.GetType().GetMethod("Add");
                //         object keyValue=classType.GetField(keyName).GetValue(dataObj);
                //         mInfo.Invoke(dicObj,new object[]{keyValue,dataObj});
                //     }
                //     tableDicB.Add(containerType.Name,containerObj);
                //     fs.Close();
                //     return (T)containerObj;
                // }
                // }catch(Exception e){
                //     Debug.Log($"表{typeof(T).Name}加载失败:"+e.Message);
                // }
                // return default(T);
                #endregion
                return (T)LoadTable(typeof(T),typeof(K));
            }
            
            /// <summary>
            /// 设置每个字段中的值
            /// </summary>
            /// <param name="info">字段信息</param>
            /// <param name="dataObj">要设置值的对象</param>
            /// <param name="bytes">二进制数据</param>
            /// <param name="index">从二进制的哪里开始读</param>
            /// <returns>读取二进制数的数量</returns>
            private int SetFieldValue(FieldInfo info,object dataObj,byte[] bytes,ref int index){
                //info.SetValue(dataObj,FormBytes(bytes,info.FieldType,ref index));
                int beginIndex=index;
                Type type=info.FieldType;
                if(type.Equals(typeof(int))){
                    info.SetValue(dataObj,BitConverter.ToInt32(bytes,index));
                    index+=sizeof(int);
                }else if(type.Equals(typeof(float))){
                    info.SetValue(dataObj,BitConverter.ToSingle(bytes,index));
                    index+=sizeof(float);
                }else if(type.Equals(typeof(bool))){
                    info.SetValue(dataObj,BitConverter.ToBoolean(bytes,index));
                    index+=sizeof(bool);
                }else if(type.Equals(typeof(string))){
                    int strLength=BitConverter.ToInt32(bytes,index);
                    index+=sizeof(int);
                    info.SetValue(dataObj,Encoding.UTF8.GetString(bytes,index,strLength));
                    index+=strLength;
                }else if(type.Equals(typeof(byte))){
                    info.SetValue(dataObj,bytes[index]);
                    index+=sizeof(byte);
                }else if(type.Equals(typeof(sbyte))){
                    info.SetValue(dataObj,(sbyte)bytes[index]);
                    index+=sizeof(sbyte);
                }else if(type.Equals(typeof(short))){
                    info.SetValue(dataObj,BitConverter.ToInt16(bytes,index));
                    index+=sizeof(short);
                }else if(type.Equals(typeof(ushort))){
                    info.SetValue(dataObj,BitConverter.ToUInt16(bytes,index));
                    index+=sizeof(ushort);
                }else if(type.Equals(typeof(uint))){
                    info.SetValue(dataObj,BitConverter.ToUInt32(bytes,index));
                    index+=sizeof(uint);
                }else if(type.Equals(typeof(long))){
                    info.SetValue(dataObj,BitConverter.ToInt64(bytes,index));
                    index+=sizeof(long);
                }else if(type.Equals(typeof(ulong))){
                    info.SetValue(dataObj,BitConverter.ToUInt64(bytes,index));
                    index+=sizeof(ulong);
                }else if(type.Equals(typeof(double))){
                    info.SetValue(dataObj,BitConverter.ToDouble(bytes,index));
                    index+=sizeof(double);
                }else if(type.Equals(typeof(char))){
                    info.SetValue(dataObj,BitConverter.ToChar(bytes,index));
                    index+=sizeof(char);
                }else if(type.Equals(typeof(decimal))){
                    //因为BitConverter没有提供将decimal转成byte[]的方法,所以将decimal变成字符串进行转byte[]
                    int length=BitConverter.ToInt32(bytes,index);
                    index+=sizeof(int);
                    info.SetValue(dataObj,decimal.Parse(Encoding.UTF8.GetString(bytes,index,length)));
                    index+=length;
                }
                return index-beginIndex;
            }
            
            /// <summary>
            /// 获取一个二进制数据文件生成的数据容器类表
            /// </summary>
            /// <param name="type">数据容器类类型</param>
            /// <returns></returns>
            public object GetTable(Type type){
                if(tableDic.ContainsKey(type.Name)){
                    return tableDic[type.Name];
                }
                Debug.Log($"({type.Name})类数据未加载!");
                return null;
            }
            
            /// <summary>
            /// 获取一个二进制数据文件生成的数据容器类表的泛型方法
            /// </summary>
            /// <typeparam name="T">数据容器类类型</typeparam>
            /// <returns></returns>
            public T GetTable<T>(){
                if(tableDic.ContainsKey(typeof(T).Name)){
                    return (T)tableDic[typeof(T).Name];
                }
                Debug.Log($"({typeof(T).Name})类数据未加载!");
                return default(T);
            }
    }
}
