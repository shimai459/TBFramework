using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace TBFramework.Data.PlayerPrefs
{
    public class PDataManager : Singleton<PDataManager>
    {
        #region PlayerPrefs方式进行数据存储和读取
            //主要使用的方法Type用反射的原理加上自定义关键字,对数据使用ISF或STR进行存储

            /// <summary>
            /// PlayerPref型的存储方法,目前支持存储所有基础类型,List,Dictionary和不包含上面以外类型的自定义类或结构体
            /// </summary>
            /// <param name="data">要存储的数据</param>
            /// <param name="keyName">存储关键字</param>
            /// <param name="saveType">存储类型</param>
            public void Save(object data,string keyName,E_PDataType saveType=E_PDataType.ISF){
                string saveKeyName=$"{keyName}_{data.GetType().Name}";
                switch(saveType){
                    case E_PDataType.ISF:
                        //保存int,byte,sbyte,short,ushort类型的数据
                        if(TypeEquals(data.GetType(),typeof(int),typeof(byte),typeof(sbyte),typeof(short),typeof(ushort)))
                        {
                            UnityEngine.PlayerPrefs.SetInt(saveKeyName,Convert.ToInt32(data));
                        }
                        //保存bool类型的数据
                        else if(TypeEquals(data.GetType(),typeof(bool)))
                        {
                            int temp=(bool)data?1:0;
                            UnityEngine.PlayerPrefs.SetInt(saveKeyName,temp);
                        }
                        //保存float类型的数据
                        else if(TypeEquals(data.GetType(),typeof(float)))
                        {
                            UnityEngine.PlayerPrefs.SetFloat(saveKeyName,(float)data);
                        }
                        //保存string,char,double,uint,long,ulong,decimal类型的数据
                        else if(TypeEquals(data.GetType(),typeof(string),typeof(char),typeof(double),typeof(uint),typeof(long),typeof(ulong),typeof(decimal)))
                        {
                            UnityEngine.PlayerPrefs.SetString(saveKeyName,data.ToString());
                        }
                        else if(data is Array){
                            Array a=data as Array;
                            UnityEngine.PlayerPrefs.SetInt(saveKeyName+"_Length",a.Length);
                            int index=0;
                            foreach(var v in a){
                                Save(v,$"{saveKeyName}_{index}",saveType);
                                index++;
                            }
                        }
                        //如果是list<>类型,因为他继承IList,所以判断是否是IList的子类即可
                        else if(typeof(IList).IsAssignableFrom(data.GetType())){
                            //先要存储List数组的长度
                            IList list=data as IList;
                            UnityEngine.PlayerPrefs.SetInt(saveKeyName+"_Count",list.Count);
                            //再存储List数组中的每个值
                            for(int i=0;i<list.Count;i++){
                                //调用自身去判断数组类型,然后去存储
                                Save(list[i],$"{saveKeyName}_{i}",saveType);
                            }
                        }
                        //如果是Dictionary<>类型,因为他继承IDictionary,所以判断是否是IDictionary的子类即可
                        else if(typeof(IDictionary).IsAssignableFrom(data.GetType())){
                            IDictionary dic=data as IDictionary;
                            UnityEngine.PlayerPrefs.SetInt(saveKeyName+"_Count",dic.Count);
                            int index=0;
                            foreach(var key in dic.Keys){
                                Save(key,$"{saveKeyName}_Key_{index}",saveType);
                                Save(dic[key],$"{saveKeyName}_Value_{index}",saveType);
                                index++;
                            }
                        }
                        // //保存一维数组类型的数据,因为数组类型名的最后肯定为[],所以用正则表达式判断一下尾部是否相同即可
                        // else if(Regex.IsMatch(data.GetType().Name,"[/[/]]$")){
                        //     object[] objs=data as object[];
                        //     PlayerPrefs.SetInt(saveKeyName+"_Length",objs.Length);
                        //     for(int i=0;i<objs.Length;i++){
                        //         PSave(objs[i],$"{saveKeyName}_{i}",saveType);
                        //     }
                        // }
                        //保存自定义类或结构体,调用存储自定义类的方法
                        else{
                            SaveCustomClass(data,keyName,saveType);
                        }
                        break;
                    case E_PDataType.Str:
                        //所以的普通类型都可以直接使用toString使其直接使用string存储
                        if(TypeEquals(data.GetType(),typeof(int),typeof(float),typeof(bool),typeof(string),typeof(char),typeof(double),typeof(uint),typeof(byte),typeof(sbyte),typeof(short),typeof(ushort),typeof(long),typeof(ulong),typeof(decimal))){
                            UnityEngine.PlayerPrefs.SetString(saveKeyName,data.ToString());
                        }
                        else if(data is Array){
                            Array a=data as Array;
                            UnityEngine.PlayerPrefs.SetString(saveKeyName+"_Length",a.Length.ToString());
                            int index=0;
                            foreach(var v in a){
                                Save(v,$"{saveKeyName}_{index}",saveType);
                                index++;
                            }
                        }
                        //如果是list<>类型,因为他继承IList,所以判断是否是IList的子类即可
                        else if(typeof(IList).IsAssignableFrom(data.GetType())){
                            //先要存储List数组的长度
                            IList list=data as IList;
                            UnityEngine.PlayerPrefs.SetString(saveKeyName+"_Count",list.Count.ToString());
                            //再存储List数组中的每个值
                            for(int i=0;i<list.Count;i++){
                                //调用自身去判断数组类型,然后去存储
                                Save(list[i],$"{saveKeyName}_{i}",saveType);
                            }
                        }
                        //如果是Dictionary<>类型,因为他继承IDictionary,所以判断是否是IDictionary的子类即可
                        else if(typeof(IDictionary).IsAssignableFrom(data.GetType())){
                            IDictionary dic=data as IDictionary;
                            UnityEngine.PlayerPrefs.SetString(saveKeyName+"_Count",dic.Count.ToString());
                            int index=0;
                            foreach(var key in dic.Keys){
                                Save(key,$"{saveKeyName}_Key_{index}",saveType);
                                Save(dic[key],$"{saveKeyName}_Value_{index}",saveType);
                                index++;
                            }
                        }
                        //保存一维数组类型的数据,因为数组类型名的最后肯定为[],所以用正则表达式判断一下尾部是否相同即可
                        // else if(Regex.IsMatch(data.GetType().Name,"[/[/]]$")){
                        //     object[] objs=data as object[];
                        //     PlayerPrefs.SetString(saveKeyName+"_Length",objs.Length.ToString());
                        //     for(int i=0;i<objs.Length;i++){
                        //         PSave(objs[i],$"{saveKeyName}_{i}",saveType);
                        //     }
                        // }
                        //保存自定义类或结构体,调用存储自定义类的方法
                        else{
                            SaveCustomClass(data,keyName,saveType);
                        }
                        break;
                }
                UnityEngine.PlayerPrefs.Save();
            }
            
            /// <summary>
            /// PlayerPref型的读取方法,目前支持存储所有基础类型,List,Dictionary和不包含上面以外类型的自定义类或结构体,读取时记得使用和存储时一样的关键字和存储方式
            /// </summary>
            /// <param name="type">要读取的数据类型</param>
            /// <param name="keyName">存储该数据时的关键字</param>
            /// <param name="saveType">存储时使用的存储方法</param>
            /// <returns></returns>
            public object Load(Type type,string keyName,E_PDataType saveType=E_PDataType.ISF){
                //此处Key规则注意要和PSave的相同
                string saveKeyName=$"{keyName}_{type.Name}";
                switch(saveType){
                    case E_PDataType.ISF:
                        if(TypeEquals(type,typeof(int)))
                        {
                            return UnityEngine.PlayerPrefs.GetInt(saveKeyName);
                        }
                        else if(TypeEquals(type,typeof(string)))
                        {
                            return UnityEngine.PlayerPrefs.GetString(saveKeyName);
                        }
                        else if(TypeEquals(type,typeof(float)))
                        {
                            return UnityEngine.PlayerPrefs.GetFloat(saveKeyName);
                        }
                        else if(TypeEquals(type,typeof(bool)))
                        {
                            return UnityEngine.PlayerPrefs.GetInt(saveKeyName)==1;
                        }
                        else if(TypeEquals(type,typeof(char)))
                        {
                            return char.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(double)))
                        {
                            return double.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(uint)))
                        {
                            return uint.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(byte)))
                        {
                            return (byte)UnityEngine.PlayerPrefs.GetInt(saveKeyName);
                        }
                        else if(TypeEquals(type,typeof(sbyte)))
                        {
                            return (sbyte)UnityEngine.PlayerPrefs.GetInt(saveKeyName);
                        }
                        else if(TypeEquals(type,typeof(short)))
                        {
                            return (short)UnityEngine.PlayerPrefs.GetInt(saveKeyName);
                        }
                        else if(TypeEquals(type,typeof(ushort)))
                        {
                            return (ushort)UnityEngine.PlayerPrefs.GetInt(saveKeyName);
                        }
                        else if(TypeEquals(type,typeof(long)))
                        {
                            return long.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(ulong)))
                        {
                            return ulong.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(decimal)))
                        {
                            return decimal.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(type.Name.Substring(type.Name.Length-2).Equals("[]")){
                            int length=UnityEngine.PlayerPrefs.GetInt(saveKeyName+"_Length");
                            Type t=type.GetElementType();
                            Array a=Array.CreateInstance(t,length);
                            for(int i=0;i<length;i++){
                                a.SetValue(Load(t,$"{saveKeyName}_{i}",saveType),i);
                            }
                            return a;
                        }
                        //如果是list<>类型,因为他继承IList,所以判断是否是IList的子类即可
                        else if(typeof(IList).IsAssignableFrom(type)){
                            //
                            IList list=Activator.CreateInstance(type) as IList;
                            //
                            for(int i=0;i<UnityEngine.PlayerPrefs.GetInt(saveKeyName+"_Count");i++){
                                //
                                list.Add(Load(type.GetGenericArguments()[0],$"{saveKeyName}_{i}",saveType));
                            }
                            return list;
                        }
                        //如果是Dictionary<>类型,因为他继承IDictionary,所以判断是否是IDictionary的子类即可
                        else if(typeof(IDictionary).IsAssignableFrom(type)){
                            IDictionary dic=Activator.CreateInstance(type) as IDictionary;
                            for(int i=0;i<UnityEngine.PlayerPrefs.GetInt(saveKeyName+"_Count");i++){
                                dic.Add(Load(type.GetGenericArguments()[0],$"{saveKeyName}_Key_{i}",saveType),Load(type.GetGenericArguments()[1],$"{saveKeyName}_Value_{i}",saveType));
                            }
                            return dic;
                        }
                        //保存一维数组类型的数据,因为数组类型名的最后肯定为[],所以用正则表达式判断一下尾部是否相同即可
                        // else if(Regex.IsMatch(type.Name,"[/[/]]$")){
                        //     object[] objs=Activator.CreateInstance(type) as object[];
                        //     for(int i=0;i<PlayerPrefs.GetInt(saveKeyName+"_Length");i++){
                        //         PSave(objs[i],$"{saveKeyName}_{i}",saveType);
                        //     }
                        //     return objs;
                        // }
                        //保存自定义类或结构体,调用存储自定义类的方法
                        else{
                            return LoadCustomClass(type,keyName,saveType);
                        }
                    case E_PDataType.Str:
                        if(TypeEquals(type,typeof(int)))
                        {
                            return int.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(string)))
                        {
                            return UnityEngine.PlayerPrefs.GetString(saveKeyName);
                        }
                        else if(TypeEquals(type,typeof(float)))
                        {
                            return float.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(bool)))
                        {
                            return bool.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(char)))
                        {
                            return char.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(double)))
                        {
                            return double.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(uint)))
                        {
                            return uint.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(byte)))
                        {
                            return byte.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(sbyte)))
                        {
                            return sbyte.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(short)))
                        {
                            return short.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(ushort)))
                        {
                            return ushort.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(long)))
                        {
                            return long.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(ulong)))
                        {
                            return ulong.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(TypeEquals(type,typeof(decimal)))
                        {
                            return decimal.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName));
                        }
                        else if(type.Name.Substring(type.Name.Length-2).Equals("[]")){
                            int length=int.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName+"_Length"));
                            Type t=type.GetElementType();
                            Array a=Array.CreateInstance(t,length);
                            for(int i=0;i<length;i++){
                                a.SetValue(Load(t,$"{saveKeyName}_{i}",saveType),i);
                            }
                            return a;
                        }
                        //如果是list<>类型,因为他继承IList,所以判断是否是IList的子类即可
                        else if(typeof(IList).IsAssignableFrom(type)){
                            //
                            IList list=Activator.CreateInstance(type) as IList;
                            //
                            for(int i=0;i<int.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName+"_Count"));i++){
                                //
                                list.Add(Load(type.GetGenericArguments()[0],$"{saveKeyName}_{i}",saveType));
                            }
                            return list;
                        }
                        //如果是Dictionary<>类型,因为他继承IDictionary,所以判断是否是IDictionary的子类即可
                        else if(typeof(IDictionary).IsAssignableFrom(type)){
                            IDictionary dic=Activator.CreateInstance(type) as IDictionary;
                            for(int i=0;i<int.Parse(UnityEngine.PlayerPrefs.GetString(saveKeyName+"_Count"));i++){
                                dic.Add(Load(type.GetGenericArguments()[0],$"{saveKeyName}_Key_{i}",saveType),Load(type.GetGenericArguments()[1],$"{saveKeyName}_Value_{i}",saveType));
                            }
                            return dic;
                        }
                        //保存一维数组类型的数据,因为数组类型名的最后肯定为[],所以用正则表达式判断一下尾部是否相同即可
                        // else if(Regex.IsMatch(type.Name,"[/[/]]$")){
                        //     object[] objs=Activator.CreateInstance(type) as object[];
                        //     for(int i=0;i<PlayerPrefs.GetInt(saveKeyName+"_Length");i++){
                        //         PSave(objs[i],$"{saveKeyName}_{i}",saveType);
                        //     }
                        //     return objs;
                        // }
                        //保存自定义类或结构体,调用存储自定义类的方法
                        else{
                            return LoadCustomClass(type,keyName,saveType);
                        }
                }
                return null;
            }
            
            /// <summary>
            /// 提供一个直接返回对应类型的方法
            /// </summary>
            /// <param name="keyName">关键字</param>
            /// <param name="saveType">存储方式</param>
            /// <typeparam name="T">数据类型</typeparam>
            /// <returns></returns>
            public T Load<T>(string keyName,E_PDataType saveType=E_PDataType.ISF){
                return (T)Load(typeof(T),keyName,saveType);
            }
            
            /// <summary>
            /// 保存自定义类或结构体的方法
            /// </summary>
            /// <param name="data">具体要存储的自定义类或结构体的对象</param>
            /// <param name="keyName">区别于同类型的关键字</param>
            /// <param name="saveType">要使用的存储类型</param>
            private void SaveCustomClass(object data,string keyName,E_PDataType saveType){
                //获取被存储对象的类型信息
                Type type=data.GetType();
                //获取该类型下的所有字段信息
                FieldInfo[] infos=type.GetFields();
                //设置保存时键的名字
                string saveKeyName="";
                //遍历所有字段,分别保存值
                foreach(FieldInfo info in infos){
                    //普通的字段,保存时的键的名字规则为:要存储类型的类型(区别于其他同类型的自定义名)_字段类型名_字段变量名
                    saveKeyName=$"{type.Name}({keyName})_{info.Name}";
                    Save(info.GetValue(data),saveKeyName,saveType);
                }
                //记得将存在内存中的值,保存到硬盘中
                UnityEngine.PlayerPrefs.Save();
            }
            
            /// <summary>
            /// 读取自定义类或结构体的方法
            /// </summary>
            /// <param name="type">要读取自定义类或结构体的类型</param>
            /// <param name="keyName">存储时使用的关键字</param>
            /// <param name="saveType">存储时使用的存储类型</param>
            /// <returns></returns>
            private object LoadCustomClass(Type type,string keyName,E_PDataType saveType){
                object data=Activator.CreateInstance(type);
                FieldInfo[] infos=type.GetFields();
                foreach(FieldInfo info in infos){
                    string saveKeyName=$"{type.Name}({keyName})_{info.Name}";
                    info.SetValue(data,Load(info.FieldType,saveKeyName,saveType));
                }
                return data;
            }
            
            /// <summary>
            /// 判断目标对象的类型是否和类型名组中有相同的,有就返回true,反之返回false
            /// </summary>
            /// <param name="dataType"></param>
            /// <param name="types"></param>
            /// <returns></returns>
            private bool TypeEquals(Type dataType,params Type[] types){
                foreach(Type type in types){
                    if(dataType.Equals(type)){
                        return true;
                    }
                }
                return false;
            }
            
        #endregion
    }
}