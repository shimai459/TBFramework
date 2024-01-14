using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace TBFramework.Data.Json
{
    public class JDataManager : Singleton<JDataManager>
    {
        #region Json方式进行数据的存储和读取

            #region 普通用于用户存储读取的方法
            #region 说明
                /*主要使用的类:
                JsonUtility:unity自带的json序列化类,使用其中ToJson和FromJson的方法
                LitJson:第三方json序列化类,主要使用其中的JsonMapper,使用其中ToJson和ToObject方法
                File:使用WriteAllText和ReadAllText方法去读写Json数据
                */
                /*使用JsonUtility方式时:
                1.自定义类前需要加上序列化特性[System.Serializable]
                2.想要序列化私有/保护变量,需要加上特性[SerializeField]
                3.JsonUtility不支持序列化字典
                4.JsonUtility存储null对象不会是null,而是默认值的数据
                5.JsonUtility不能直接将数据反序列化成数据集合(如List,数组)
                6.编码格式必须为UTF-8
                */
                /*使用LitJson方式时:
                1.LitJson不支持(反)序列化私有/保护变量
                2.支持字典的序列化,但是键必须为string,否则反序列化会报错
                3.支持将数据直接反序列化成数据集合(数组,List,Dictionary)
                4.LitJson反序列化时,需要自定义类型有无参构造函数
                5.LitJson存储null对象是null
                6.编码格式必须为UTF-8
                */
                #endregion

            /// <summary>
            /// 将数据用Json方式写入文件
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="fileName">写入的Json文件名</param>
            /// <param name="saveType">Json序列化使用的方式,默认使用LitJson</param>
            public void Save(object data,string fileName,E_JDataType saveType=E_JDataType.LitJson,bool isConfig=false){
                if(!Directory.Exists(JDataSet.JSON_DATA_PATH)){
                    Directory.CreateDirectory(JDataSet.JSON_DATA_PATH);
                }
                if(!Directory.Exists(JDataSet.JSON_DATA_CONFIGURATION_PATH)){
                    Directory.CreateDirectory(JDataSet.JSON_DATA_CONFIGURATION_PATH);
                }
                //设置存储地址
                string path=Path.Combine(JDataSet.JSON_DATA_PATH,fileName+".json");
                if(isConfig){
                    path=Path.Combine(JDataSet.JSON_DATA_CONFIGURATION_PATH,fileName+".json");
                }
                string json="";
                //按不同Json序列化方式对数据进行序列化
                switch(saveType){
                    case E_JDataType.JsonUtility:
                        json=JsonUtility.ToJson(data);
                        break;
                    case E_JDataType.LitJson:
                        json=JsonMapper.ToJson(data);
                        break;
                }
                //如果json不为空,就写入文件中
                if(json!=""){
                    File.WriteAllText(path,json);
                }
            }
            
            /// <summary>
            /// 使用Json格式从文件中读取数据
            /// </summary>
            /// <param name="type">数据类型</param>
            /// <param name="fileName">Json文件名</param>
            /// <param name="saveType">Json反序列化使用的方式,默认使用LitJson</param>
            /// <returns></returns>
            public object Load(Type type,string fileName,E_JDataType saveType=E_JDataType.LitJson){
                //设置存储文件地址
                string path=Path.Combine(JDataSet.JSON_DATA_PATH,fileName+".json");
                //判断文件地址是否存在,不存在返回一个默认数据
                if(!File.Exists(path)){
                    path=Path.Combine(JDataSet.JSON_DATA_CONFIGURATION_PATH,fileName+".json");
                    if(!File.Exists(path)){
                        return Activator.CreateInstance(type);
                    }
                }
                //从文件中读取Json数据
                string json=File.ReadAllText(path);
                //将Json数据按不同的方式反序列化成数据类
                switch(saveType){
                    case E_JDataType.JsonUtility:
                        return JsonUtility.FromJson(json,type);
                    case E_JDataType.LitJson:
                        return JsonMapper.ToObject(json,type);
                }
                return null;
            }
            
            /// <summary>
            /// 使用Json格式从文件中读取数据的泛型方法
            /// </summary>
            /// <param name="fileName">Json文件名</param>
            /// <param name="saveType">Json反序列化使用的方式,默认使用LitJson</param>
            /// <typeparam name="T">数据类型</typeparam>
            /// <returns></returns>
            public T Load<T>(string fileName,E_JDataType saveType=E_JDataType.LitJson)
            {
                //配置存储文件地址
                string path=Path.Combine(JDataSet.JSON_DATA_PATH,fileName+".json");
                //判断是否存在文件地址,没有返回默认值
                if(!File.Exists(path)){
                    path=Path.Combine(JDataSet.JSON_DATA_CONFIGURATION_PATH,fileName+".json");
                    if(!File.Exists(path)){
                        return Activator.CreateInstance<T>();
                    }
                }
                //从文件中将Json数据读取出来
                string json=File.ReadAllText(path);
                //将Json数据按不同方式反序列化成数据类
                switch(saveType){
                    case E_JDataType.JsonUtility:
                        return JsonUtility.FromJson<T>(json);
                    case E_JDataType.LitJson:
                        return JsonMapper.ToObject<T>(json);
                }
                return default(T);
            }
            
            #endregion

        #endregion
    }
}