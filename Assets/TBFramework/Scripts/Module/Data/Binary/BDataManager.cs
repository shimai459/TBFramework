using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TBFramework.Data.Binary
{
    public class BDataManager : Singleton<BDataManager>
    {
        #region Binary(二进制文件)方式进行数据的存储和读取

            #region 普通用于用户存储读取的方法
            #region 说明
            /*主要使用的类:
            1.MemoryStream
            2.BinaryFormatter

            */
            /*存储或读取二进制文件
            方法一:(此处使用方法一)
            使用MemoryStream和BinaryFormatter结合使用
            存:
            1.用using语句创建一个MemoryStream内存流文件对象
            2.再创建一个BinaryFormatter二进制格式化对象
            3.使用BinaryFormatter的成员方法Serialize(需要两个参数,一个流对象,一个要序列化的对象(序列化对象中包含自定义类或结构体一定要在前面加特性[System.Serializable]))去序列化
            4.用MemoryStream的成员方法GetBuffer方法获取序列化的字节数组
            5.用File的静态方法WriteAllBytes写入文件中
            示例:
            using(MemoryStream ms =new MemoryStream()){
                BinaryFormatter bf=new BinaryFormatter();
                bf.Serialize(ms,data);
                byte[] bytes=ms.GetBuffer();
                File.WriteAllBytes(path,bytes);
            }
            读:
            1.将字节数据用File的静态方法ReadAllBytes读到内存中
            2.用using语句和刚刚的字节数据创建一个MemoryStream内存流文件对象
            3.再创建一个BinaryFormatter二进制格式化对象
            4.使用BinaryFormatter的成员方法Deserialize(需要一个参数,一个有字节数据的流对象)去反序列化,得到一个object
            示例:
            byte[] bytes=File.ReadAllBytes();
            using(MemoryStream ms =new MemoryStream(bytes)){
                BinaryFormatter bf=new BinaryFormatter();
                object o=bf.Deserialize(ms);
            }
            方法二:
            使用FileStream和BinaryFormatter结合使用
            存:
            1.用using语句传入写入的路径创建一个FileStream内存流文件对象,设置访问模式为(OpenOrCreate,原来有文件也可使用Open,最好还是使用OpenOrCreate),访问权限为(Write)
            2.再创建一个BinaryFormatter二进制格式化对象
            3.使用BinaryFormatter的成员方法Serialize(需要两个参数,一个流对象,一个要序列化的对象(序列化对象中包含自定义类或结构体一定要在前面加特性[System.Serializable]))去序列化
            4.让数据从内存写到硬盘中
            5.关闭流文件
            示例:
            using(FileStream fs=new FileStream(path,FileMode.OpenOrCreate,FileAsset.Write)){
                BinaryFormatter bf=new BinaryFormatter();
                bf.Serialize(fs,data);
                fs.Flush();
                fs.Close();
            }
            读:
            1.用using语句传入读取的路径创建一个FileStream内存流文件对象,设置访问模式为(Open),访问权限为(Read)
            2.再创建一个BinaryFormatter二进制格式化对象
            3.使用BinaryFormatter的成员方法Deserialize(需要一个参数,一个有字节数据的流对象)去反序列化,得到一个object
            示例:
            using(FileStream fs=new FileStream(path,FileMode.Open,FileAsset.Read)){
                BinaryFormatter bf=new BinaryFormatter();
                object o=bf.Deserialize(fs);
            }
            */
            /*注意:
            1.要序列化自定义类或结构体,需要在该类或结构体前添加特性[System.Serializable]
            

            */
            #endregion
            
            /// <summary>
            /// 将数据用二进制方式写入文件中,使用了异或加密
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="fileName">存储的二进制数据文件名</param>
            public void Save(object data,string fileName,bool isConfig=false){
                if(!Directory.Exists(BDataSet.BINARY_DATA_PATH)){
                    Directory.CreateDirectory(BDataSet.BINARY_DATA_PATH);
                }
                if(!Directory.Exists(BDataSet.BINARY_DATA_CONFIGURATION_PATH)){
                    Directory.CreateDirectory(BDataSet.BINARY_DATA_CONFIGURATION_PATH);
                }
                //配置存储地址
                string path=Path.Combine(BDataSet.BINARY_DATA_PATH,fileName+BDataSet.BINARY_EXTENSION);
                if(isConfig){
                    path=Path.Combine(BDataSet.BINARY_DATA_CONFIGURATION_PATH,fileName+BDataSet.BINARY_EXTENSION);
                }
                //使用C#自带系列化类BinaryFormatter成二进制文件的方式,使用MemoryStream可以读取到转换后的二进制,可以对其进行加密或网络传输
                using(MemoryStream ms=new MemoryStream()){
                    BinaryFormatter bf=new BinaryFormatter();
                    bf.Serialize(ms,data);
                    byte[] bytes=ms.GetBuffer();
                    //异或加密
                    for(int i=0;i<bytes.Length;i++){
                        bytes[i]^=BDataSet.BINARY_DATA_ENCRYPT;
                    }
                    File.WriteAllBytes(path,bytes);
                    //关闭流文件
                    ms.Close();
                }
            }
            
            /// <summary>
            /// 从二进制文件中读取数据
            /// </summary>
            /// <param name="type">读取数据类型</param>
            /// <param name="fileName">读取文件名</param>
            /// <returns></returns>
            public object Load(Type type,string fileName){
                //配置存储地址
                string path=Path.Combine(BDataSet.BINARY_DATA_PATH,fileName+BDataSet.BINARY_EXTENSION);
                //判断文件是否存在,没有去读取配置文件信息,如果配置文件也没有,就返回一个新建的类型对象
                if(!File.Exists(path)){
                    path=Path.Combine(BDataSet.BINARY_DATA_CONFIGURATION_PATH,fileName+BDataSet.BINARY_EXTENSION);
                    if(!File.Exists(path)){
                        return Activator.CreateInstance(type);
                    }
                }
                //从文件中读取二进制数据
                byte[] bytes=File.ReadAllBytes(path);
                //异或解密
                for(int i=0;i<bytes.Length;i++){
                    bytes[i]^=BDataSet.BINARY_DATA_ENCRYPT;
                }
                using(MemoryStream ms=new MemoryStream(bytes)){
                    BinaryFormatter bf=new BinaryFormatter();
                    return bf.Deserialize(ms);
                }
            }
            
            /// <summary>
            /// 从二进制文件中读取数据的泛型方法
            /// </summary>
            /// <param name="fileName">读取文件名</param>
            /// <typeparam name="T">数据类型</typeparam>
            /// <returns></returns>
            public T Load<T>(string fileName){
                //配置存储地址
                string path=Path.Combine(BDataSet.BINARY_DATA_PATH,fileName+BDataSet.BINARY_EXTENSION);
                //判断文件是否存在,没有去读取配置文件信息,如果配置文件也没有,就返回一个新建的类型对象
                if(!File.Exists(path)){
                    path=Path.Combine(BDataSet.BINARY_DATA_CONFIGURATION_PATH,fileName+BDataSet.BINARY_EXTENSION);
                    if(!File.Exists(path)){
                        return Activator.CreateInstance<T>();
                    }
                }
                //从文件中读取二进制数据
                byte[] bytes=File.ReadAllBytes(path);
                //异或解密
                for(int i=0;i<bytes.Length;i++){
                    bytes[i]^=BDataSet.BINARY_DATA_ENCRYPT;
                }
                using(MemoryStream ms=new MemoryStream(bytes)){
                    BinaryFormatter bf=new BinaryFormatter();
                    return (T)bf.Deserialize(ms);
                }
            }
            
            #endregion
        
            #region 自定义存储和读取二进制文件的方法
            
            public int ToBytes(byte[] bytes,object data,ref int index,bool isCheckCustomInherit){
                int beginIndex=index;
                try{
                    byte[] bs;
                    switch(data){
                        case int value:
                            BitConverter.GetBytes(value).CopyTo(bytes,index);
                            index+=sizeof(int);
                            break;
                        case long value:
                            BitConverter.GetBytes(value).CopyTo(bytes,index);
                            index+=sizeof(long);
                            break;
                        case float value:
                            BitConverter.GetBytes(value).CopyTo(bytes,index);
                            index+=sizeof(float);
                            break;
                        case double value:
                            BitConverter.GetBytes(value).CopyTo(bytes,index);
                            index+=sizeof(double);
                            break;
                        case char value:
                            BitConverter.GetBytes(value).CopyTo(bytes,index);
                            index+=sizeof(char);
                            break;
                        case bool value:
                            BitConverter.GetBytes(value).CopyTo(bytes,index);
                            index+=sizeof(bool);
                            break;
                        case short value:
                            BitConverter.GetBytes(value).CopyTo(bytes,index);
                            index+=sizeof(short);
                            break;
                        case sbyte value:
                            //BitConverter.GetBytes(value).CopyTo(bytes,index);
                            bytes[index]=(byte)value;
                            index+=sizeof(sbyte);
                            break;
                        case uint value:
                            BitConverter.GetBytes(value).CopyTo(bytes,index);
                            index+=sizeof(uint);
                            break;
                        case ulong value:
                            BitConverter.GetBytes(value).CopyTo(bytes,index);
                            index+=sizeof(ulong);
                            break;
                        case ushort value:
                            BitConverter.GetBytes(value).CopyTo(bytes,index);
                            index+=sizeof(ushort);
                            break;
                        case byte value:
                            bytes[index]=value;
                            index+=sizeof(byte);
                            break;
                        case decimal value:
                            bs=Encoding.UTF8.GetBytes(value.ToString());
                            BitConverter.GetBytes(bs.Length).CopyTo(bytes,index);
                            index+=sizeof(int);
                            bs.CopyTo(bytes,index);
                            index+=bs.Length;
                            break;
                        case string value:
                            bs=Encoding.UTF8.GetBytes(value.ToString());
                            BitConverter.GetBytes(bs.Length).CopyTo(bytes,index);
                            index+=sizeof(int);
                            bs.CopyTo(bytes,index);
                            index+=bs.Length;
                            break;
                        case Array value:
                            //先存储数组的长度
                            BitConverter.GetBytes(value.Length).CopyTo(bytes,index);
                            index+=sizeof(int);
                            //防止object数组读数据读不出
                            bool isAObject=false;
                            if(value.GetType().GetElementType().Equals(typeof(object))){
                                isAObject=true;
                            }
                            foreach(var v in value){
                                if(isAObject){
                                    ToBytes(bytes,v.GetType().FullName,ref index,isCheckCustomInherit);
                                }
                                ToBytes(bytes,v,ref index,isCheckCustomInherit);
                            }
                            break;
                        case IList value:
                            //先存储集合类的长度
                            BitConverter.GetBytes(value.Count).CopyTo(bytes,index);
                            index+=sizeof(int);
                            //防止object数组和非泛型读数据读不出
                            bool isLObject=false;
                            if(value.GetType().GenericTypeArguments.Length>0&&value.GetType().GenericTypeArguments[0].Equals(typeof(object))||value.GetType().GenericTypeArguments.Length==0){
                                isLObject=true;
                            }
                            foreach(var v in value){
                                if(isLObject){
                                    ToBytes(bytes,v.GetType().FullName,ref index,isCheckCustomInherit);
                                }
                                ToBytes(bytes,v,ref index,isCheckCustomInherit);
                            }
                            break;
                        case IDictionary value:
                            //先存储集合类的长度
                            BitConverter.GetBytes(value.Count).CopyTo(bytes,index);
                            index+=sizeof(int);
                            //防止object数组和非泛型读数据读不出
                            bool isK2Object=false;
                            bool isV2Object=false;
                            if(value.GetType().GenericTypeArguments.Length>1){
                                if(value.GetType().GenericTypeArguments[0].Equals(typeof(object))){
                                    isK2Object=true;
                                }
                                if(value.GetType().GenericTypeArguments[1].Equals(typeof(object))){
                                    isV2Object=true;
                                }
                            }else{
                                isK2Object=true;
                                isV2Object=true;
                            }
                            foreach(var v in value.Keys){
                                if(isK2Object){
                                    ToBytes(bytes,v.GetType().FullName,ref index,isCheckCustomInherit);
                                }
                                if(isV2Object){
                                    ToBytes(bytes,value[v].GetType().FullName,ref index,isCheckCustomInherit);
                                }
                                ToBytes(bytes,v,ref index,isCheckCustomInherit);
                                ToBytes(bytes,value[v],ref index,isCheckCustomInherit);
                            }
                            break;
                        // case ICollection value:
                        //     //先存储集合类的长度
                        //     BitConverter.GetBytes(value.Count).CopyTo(bytes,index);
                        //     index+=sizeof(int);
                        //     bool isCObject=false;
                        //     //防止object数组和非泛型读数据读不出
                        //     if(value.GetType().GenericTypeArguments.Length>0&&value.GetType().GenericTypeArguments[0].Equals(typeof(object))){
                        //         isCObject=true;
                        //     }
                        //     foreach(var v in value){
                        //         if(isCObject){
                        //             ToBytes(bytes,v.GetType().FullName,ref index);
                        //         }
                        //         ToBytes(bytes,v,ref index);
                        //     }
                        //     break;
                        default:
                            Type t=data.GetType();
                            string[] strs=t.FullName.Split('.');
                            if(!(strs.Length>1&&!strs[1].Equals("Collections"))){
                                //先存真正的变量类型,防止父类装子类,导致出错,网络由最前面传的识别码来判断传的是什么类
                                if(isCheckCustomInherit){
                                    ToBytes(bytes,t.FullName,ref index,isCheckCustomInherit);
                                }
                                FieldInfo[] fields=t.GetFields();
                                foreach(FieldInfo info in fields){
                                    ToBytes(bytes,info.GetValue(data),ref index,isCheckCustomInherit);
                                }
                            }
                            break;
                    }
                }catch(Exception e){
                    Debug.Log($"({data.GetType().Name})写成byte数据失败:"+e.Message);
                }
                return index-beginIndex;
            
            }

            public object FromBytes(byte[] bytes,Type type,ref int index,bool isCheckCustomInherit){
                try{
                    if(type.Equals(typeof(int))){
                        int i=BitConverter.ToInt32(bytes,index);
                        index+=sizeof(int);
                        return i;
                    }else if(type.Equals(typeof(float))){
                        float f=BitConverter.ToSingle(bytes,index);
                        index+=sizeof(float);
                        return f;
                    }else if(type.Equals(typeof(bool))){
                        bool b=BitConverter.ToBoolean(bytes,index);
                        index+=sizeof(bool);
                        return b;
                    }else if(type.Equals(typeof(char))){
                        char c=BitConverter.ToChar(bytes,index);
                        index+=sizeof(char);
                        return c;
                    }else if(type.Equals(typeof(string))){
                        int length=BitConverter.ToInt32(bytes,index);
                        index+=sizeof(int);
                        string s=Encoding.UTF8.GetString(bytes,index,length);
                        index+=length;
                        return s;
                    }else if(type.Equals(typeof(double))){
                        double d=BitConverter.ToDouble(bytes,index);
                        index+=sizeof(double);
                        return d;
                    }else if(type.Equals(typeof(byte))){
                        byte b=bytes[index];
                        index+=sizeof(byte);
                        return b;
                    }else if(type.Equals(typeof(sbyte))){
                        sbyte sb=(sbyte)bytes[index];
                        index+=sizeof(sbyte);
                        return sb;
                    }else if(type.Equals(typeof(short))){
                        short s=BitConverter.ToInt16(bytes,index);
                        index+=sizeof(short);
                        return s;
                    }else if(type.Equals(typeof(ushort))){
                        ushort us=BitConverter.ToUInt16(bytes,index);
                        index+=sizeof(ushort);
                        return us;
                    }else if(type.Equals(typeof(uint))){
                        uint ui=BitConverter.ToUInt32(bytes,index);
                        index+=sizeof(uint);
                        return ui;
                    }else if(type.Equals(typeof(long))){
                        long l=BitConverter.ToInt64(bytes,index);
                        index+=sizeof(long);
                        return l;
                    }else if(type.Equals(typeof(ulong))){
                        ulong ul=BitConverter.ToUInt64(bytes,index);
                        index+=sizeof(ulong);
                        return ul;
                    }else if(type.Equals(typeof(decimal))){
                        int length=BitConverter.ToInt32(bytes,index);
                        index+=sizeof(int);
                        decimal d=decimal.Parse(Encoding.UTF8.GetString(bytes,index,length));
                        index+=length;
                        return d;
                    }else if(type.Equals(typeof(object))){
                        return FromBytes(bytes,Type.GetType(FromBytes(bytes,typeof(string),ref index,isCheckCustomInherit) as string),ref index,isCheckCustomInherit);
                    }else if(type.Name.Substring(type.Name.Length-2).Equals("[]")){
                        int length=BitConverter.ToInt32(bytes,index);
                        index+=sizeof(int);
                        Type elementType = type.GetElementType();
                        Array array= Array.CreateInstance(elementType,length);
                        for(int i=0;i<length;i++){
                            array.SetValue(FromBytes(bytes,elementType,ref index,isCheckCustomInherit),i);
                        }
                        return array;
                    }else{
                        object o=Activator.CreateInstance(type);
                        int length=0;
                        switch(o){
                            case IList value:
                                length=BitConverter.ToInt32(bytes,index);
                                index+=sizeof(int);
                                IList list=o as IList;
                                if(type.GetGenericArguments().Length>0){
                                    Type elementType = type.GetGenericArguments()[0];
                                    for(int i=0;i<length;i++){
                                        list.Add(FromBytes(bytes,elementType,ref index,isCheckCustomInherit));
                                    }
                                }else{
                                    for(int i=0;i<length;i++){
                                        list.Add(FromBytes(bytes,typeof(object),ref index,isCheckCustomInherit));
                                    }
                                }
                                return list;
                            case IDictionary value:
                                length=BitConverter.ToInt32(bytes,index);
                                index+=sizeof(int);
                                IDictionary dic=o as IDictionary;
                                if(type.GenericTypeArguments.Length>1){
                                    Type elementKeyType=type.GenericTypeArguments[0];
                                    Type elementValueType=type.GenericTypeArguments[1];
                                    for(int i=0;i<length;i++){
                                        dic.Add(FromBytes(bytes,elementKeyType,ref index,isCheckCustomInherit),FromBytes(bytes,elementValueType,ref index,isCheckCustomInherit));
                                    }
                                }else{
                                    for(int i=0;i<length;i++){
                                        dic.Add(FromBytes(bytes,typeof(object),ref index,isCheckCustomInherit),FromBytes(bytes,typeof(object),ref index,isCheckCustomInherit));
                                    }
                                }
                                return dic;
                            default:
                                string[] strs=type.FullName.Split('.');
                                if(!(strs.Length>1&&!strs[1].Equals("Collections"))){
                                    if(isCheckCustomInherit){
                                        type=Type.GetType(FromBytes(bytes,typeof(string),ref index,isCheckCustomInherit) as string);
                                        o=Activator.CreateInstance(type);
                                    }
                                    FieldInfo[] fields=type.GetFields();
                                    foreach(FieldInfo info in fields){
                                        info.SetValue(o,FromBytes(bytes,info.FieldType,ref index,isCheckCustomInherit));
                                    }
                                    return o;
                                }
                                break;
                        }

                        #region if else
                        // if(o is IList){
                        //     length=BitConverter.ToInt32(bytes,index);
                        //         index+=sizeof(int);
                        //         IList list=o as IList;
                        //         if(type.GetGenericArguments().Length>0){
                        //             Type elementType = type.GetGenericArguments()[0];
                        //             for(int i=0;i<length;i++){
                        //                 list.Add(FromBytes(bytes,elementType,ref index));
                        //             }
                        //         }else{
                        //             for(int i=0;i<length;i++){
                        //                 list.Add(FromBytes(bytes,typeof(object),ref index));
                        //             }
                        //         }
                        //         return list;
                        // }else if(o is IDictionary){
                        //     length=BitConverter.ToInt32(bytes,index);
                        //         index+=sizeof(int);
                        //         IDictionary dic=o as IDictionary;
                        //         if(type.GenericTypeArguments.Length>1){
                        //             Type elementKeyType=type.GenericTypeArguments[0];
                        //             Type elementValueType=type.GenericTypeArguments[1];
                        //             for(int i=0;i<length;i++){
                        //                 dic.Add(FromBytes(bytes,elementKeyType,ref index),FromBytes(bytes,elementValueType,ref index));
                        //             }
                        //         }else{
                        //             for(int i=0;i<length;i++){
                        //                 dic.Add(FromBytes(bytes,typeof(object),ref index),FromBytes(bytes,typeof(object),ref index));
                        //             }
                        //         }
                        //         return dic;
                        // }
                        // // else if(o is ICollection){
                        // //     int length=BitConverter.ToInt32(bytes,index);
                        // //     index+=sizeof(int);
                        // //     ICollection collection=o as ICollection;
                        // //     if(type.GenericTypeArguments.Length>0){
                        // //         Type elementType = type.GenericTypeArguments[0];
                        // //         for(int i=0;i<length;i++){
                        // //             //collection.CopyTo()
                                    
                        // //         }
                        // //     }else{
                        // //         for(int i=0;i<length;i++){
                        // //             //collection.CopyTo()
                        // //         }
                        // //     }
                        // //     return collection;
                        // // }
                        // else{
                        //     string[] strs=type.FullName.Split('.');
                        //         if(!(strs.Length>1&&!strs[1].Equals("Collections"))){
                        //             if(isCheckCustomInherit){
                        //                 type=Type.GetType(FromBytes(bytes,typeof(string),ref index) as string);
                        //                 o=Activator.CreateInstance(type);
                        //             }
                        //             FieldInfo[] fields=type.GetFields();
                        //             foreach(FieldInfo info in fields){
                        //                 info.SetValue(o,FromBytes(bytes,info.FieldType,ref index));
                        //             }
                        //             return o;
                        //         }
                        // }
                        #endregion
                    
                    }
                }catch(Exception e){
                    Debug.Log($"({type.Name})读byte数据失败:"+e.Message);
                }
                return null;
            }

            #endregion

        #endregion
    }
}