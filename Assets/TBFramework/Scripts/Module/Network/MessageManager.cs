using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBFramework.Data.Binary;
using TBFramework.Config.Data.Binary;


namespace TBFramework.Net
{
    public class MessageManager : Singleton<MessageManager>
    {
        private Dictionary<int,string> messageDic=new Dictionary<int, string>();

        /// <summary>
        /// 读取
        /// </summary>
        private void GetMessageICData(){
            
            if(messageDic.Count==0){
                MessageICDataContainer mdc=BCDataManager.Instance.LoadTable<MessageICDataContainer,MessageICData>();
                foreach(MessageICData mc in mdc.dataDic.Values){
                    messageDic.Add(mc.IdentificationCode,mc.MessageClassName);
                }
            }
        }
        
        public int MessageToBytes(byte[] bytes,object data,ref int index,bool isFirst){
            //更新识别码和类名信息
            GetMessageICData();
            int beginIndex=index;
            Type type=data.GetType();
            //如果是消息类,先将消息类的识别码存储下来,方便去读取数据(也可以防止父类装子类,读数据时创建真正的消息类型,让数据读取正确)
            if(messageDic.ContainsValue(type.FullName)){
                BitConverter.GetBytes(messageDic.Keys.ToList()[messageDic.Values.ToList().IndexOf(type.FullName)]).CopyTo(bytes,index);
                index+=sizeof(int);
            
                //第一次类传入,判断是否是消息类
                if(isFirst){
                    //预留总长度的数组空间
                    index+=sizeof(int);
                }
                //使用反射取出消息类中的所有字段,去赋值
                FieldInfo[] fields =type.GetFields();
                object fValue;
                int length=-1;
                foreach(FieldInfo field in fields){
                    fValue=field.GetValue(data);
                    length=NetToBytes(bytes,fValue,ref index);
                    if(length==0){
                        return 0;
                    }
                }
                //在前面加入字节总长度信息
                if(isFirst){
                    BitConverter.GetBytes(index-beginIndex).CopyTo(bytes,beginIndex+sizeof(int));
                }
                return index-beginIndex;
            }else{
                Debug.Log("对象不属于有标识码的类或标识码对象中存在不属于有标识码的自定义类,不可进入网络传递");
                return 0;
            }
        }

        /// <summary>
        /// 数组和集合类的字节化函数
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private int NetToBytes(byte[] bytes,object data,ref int index){
            bool isMessage=false;
            int beginIndex=index;
            int length=-1;
            if(TypeIsBaseType(data.GetType())){
                length=length==0?0:BDataManager.Instance.ToBytes(bytes,data,ref index,false);
            }else if(data is Array){
                Array array =data as Array;
                BitConverter.GetBytes(array.Length).CopyTo(bytes,index);
                index+=sizeof(int);
                if(messageDic.ContainsValue(data.GetType().GetElementType().FullName)){
                    isMessage=true;
                }
                foreach(var v in array){
                    if(isMessage){
                        length=length==0?0:MessageToBytes(bytes,v,ref index,false);
                    }else{
                        length=length==0?0:NetToBytes(bytes,v,ref index);
                    }
                    
                }
            }else if(data is IList){
                IList iList =data as IList;
                BitConverter.GetBytes(iList.Count).CopyTo(bytes,index);
                index+=sizeof(int);
                if(data.GetType().GenericTypeArguments.Length>0&&messageDic.ContainsValue(data.GetType().GenericTypeArguments[0].FullName)){
                    isMessage=true; 
                }
                foreach(var v in iList){
                    if(isMessage){
                        length=length==0?0:MessageToBytes(bytes,v,ref index,false);
                    }else{
                        length=length==0?0:NetToBytes(bytes,v,ref index);
                    }
                }
            }else if(data is IDictionary&&data.GetType().GenericTypeArguments.Length>1){
                bool keyIsMessage=false;
                bool valueIsMessage=false;
                if(messageDic.ContainsValue(data.GetType().GenericTypeArguments[0].FullName)){
                    keyIsMessage=true;
                }
                if(messageDic.ContainsValue(data.GetType().GenericTypeArguments[1].FullName)){
                    valueIsMessage=true;
                }
                IDictionary id=data as IDictionary;
                BitConverter.GetBytes(id.Count).CopyTo(bytes,index);
                index+=sizeof(int);
                foreach(var v in id.Keys){
                    if(keyIsMessage){
                        length=length==0?0:MessageToBytes(bytes,v,ref index,false);
                    }else{
                        length=length==0?0:NetToBytes(bytes,v,ref index);
                    }
                    if(valueIsMessage){
                        length=length==0?0:MessageToBytes(bytes,id[v],ref index,false);
                    }else{
                        length=length==0?0:NetToBytes(bytes,id[v],ref index);
                    }
                }
            }else if(messageDic.ContainsValue(data.GetType().FullName)){
                length=length==0?0:MessageToBytes(bytes,data,ref index,false);
            }
            return length==0?length:(index-beginIndex);
        }

        public object MessageFromBytes(byte[] bytes,ref int index,ref int bLength){
            GetMessageICData();
            //判断长度是否等于消息头部(消息识别码+消息总长度)的长度,或者是消息类中的消息类
            if(bLength>=sizeof(int)*2||bLength==-1){
                //获取消息类的设备码
                int length=0;
                int beginIndex=index;
                int ic=BitConverter.ToInt32(bytes,index);
                index+=sizeof(int);
                //获取该消息类的总长度,用于分包和黏包情况的处理
                if(bLength!=-1){
                    length=BitConverter.ToInt32(bytes,index);
                    index+=sizeof(int);
                    if(length<bLength){
                        index-=sizeof(int)*2;
                        return null;
                    }
                }
                //通过识别码获取消息类型,如果该类型不存在,且当前有该类的全部数据,就将该类的数据直接移除
                Type type;
                if(messageDic.ContainsKey(ic)){
                    type=Type.GetType(messageDic[ic]);
                }else{
                    Debug.Log("传入数据非消息类!!");
                    if(bLength!=-1){
                        Array.Copy(bytes,beginIndex+length,bytes,beginIndex,bLength-beginIndex-length);
                        bLength-=length;
                    }   
                    return null;
                }
                //将消息类初始化,并将它的每一个值填入
                object o=Activator.CreateInstance(type);
                object value;
                FieldInfo[] fields=type.GetFields();
                foreach(FieldInfo field in fields){
                    value=NetFromBytes(bytes,field.FieldType,ref index);
                    //传出的数据如果为null,则说明该数据有问题,则直接将null传给上级
                    if(value!=null){
                        field.SetValue(o,value);
                    }else{
                        //到达消息类级,得null则说明该数据存在问题,直接将该数据全部清楚
                        if(bLength!=-1){
                            Array.Copy(bytes,beginIndex+length,bytes,beginIndex,bLength-beginIndex-length);
                            bLength-=length;
                        }
                        return null;
                    }
                }
                return o;
            }
            return null;
        }

        private object NetFromBytes(byte[] bytes,Type type,ref int index){
            int mLength=-1;
            if(TypeIsBaseType(type)){
                return BDataManager.Instance.FromBytes(bytes,type,ref index,false);
            }
            //消息类型处理
            else if(messageDic.ContainsValue(type.FullName)){
                return MessageFromBytes(bytes,ref index,ref mLength);
            }
            //数组处理
            else if(type.Name.Substring(type.Name.Length-2).Equals("[]")){
                int length=BitConverter.ToInt32(bytes,index);
                index+=sizeof(int);
                Type elementType = type.GetElementType();
                Array array= Array.CreateInstance(elementType,length);
                for(int i=0;i<length;i++){
                    if(messageDic.ContainsValue(elementType.FullName)){
                        array.SetValue(MessageFromBytes(bytes,ref index,ref mLength),i);
                    }else{
                        array.SetValue(BDataManager.Instance.FromBytes(bytes,elementType,ref index,false),i);
                    } 
                }
                return array;
            }else{
                object o=Activator.CreateInstance(type);
                int length=0;
                switch(o){
                    //IList处理
                    case IList value:
                        length=BitConverter.ToInt32(bytes,index);
                        index+=sizeof(int);
                        IList list=o as IList;
                        if(type.GetGenericArguments().Length>0){
                            Type elementType = type.GetGenericArguments()[0];
                            if(messageDic.ContainsValue(elementType.FullName)){
                                for(int i=0;i<length;i++){
                                    list.Add(MessageFromBytes(bytes,ref index,ref mLength));
                                }
                            }else{
                                for(int i=0;i<length;i++){
                                    list.Add(BDataManager.Instance.FromBytes(bytes,elementType,ref index,false));
                                }
                            }
                            return list;
                        }else{
                            Debug.Log($"输入的数据中存在元素为Object的集合类({o.GetType().FullName}),这是不允许的");
                            return null;
                        }
                    //IDictionary处理
                    case IDictionary value:
                        length=BitConverter.ToInt32(bytes,index);
                        index+=sizeof(int);
                        IDictionary dic=o as IDictionary;
                        if(type.GenericTypeArguments.Length>1){
                            Type elementKeyType=type.GenericTypeArguments[0];
                            Type elementValueType=type.GenericTypeArguments[1];
                            if(messageDic.ContainsValue(elementKeyType.FullName)&&messageDic.ContainsValue(elementValueType.FullName)){
                                for(int i=0;i<length;i++){
                                    dic.Add(MessageFromBytes(bytes,ref index,ref mLength),MessageFromBytes(bytes,ref index,ref mLength));
                                }
                            }else if(messageDic.ContainsValue(elementKeyType.FullName)){
                                for(int i=0;i<length;i++){
                                    dic.Add(MessageFromBytes(bytes,ref index,ref mLength),BDataManager.Instance.FromBytes(bytes,elementValueType,ref index,false));
                                }
                            }else if(messageDic.ContainsValue(elementValueType.FullName)){
                                for(int i=0;i<length;i++){
                                    dic.Add(BDataManager.Instance.FromBytes(bytes,elementKeyType,ref index,false),MessageFromBytes(bytes,ref index,ref mLength));
                                }
                            }else{
                                for(int i=0;i<length;i++){
                                    dic.Add(BDataManager.Instance.FromBytes(bytes,elementKeyType,ref index,false),BDataManager.Instance.FromBytes(bytes,elementValueType,ref index,false));
                                }
                            }
                            return dic;
                        }else{
                            Debug.Log($"输入的数据中存在元素为Object的集合类({o.GetType().FullName}),这是不允许的");
                            return null;
                        }
                    default:
                        Debug.Log($"传输的数据中包含了未经设置的类({type.FullName})");
                        break;
                }
                
            }
            return null;
        }

        /// <summary>
        /// 判断一个类型是不是基础数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool TypeIsBaseType(Type type){
            if(TypeEquals(type,typeof(int),typeof(bool),typeof(float),typeof(double),typeof(string),
                                typeof(byte),typeof(sbyte),typeof(short),typeof(ushort),typeof(char),
                                typeof(uint),typeof(long),typeof(ulong),typeof(decimal))
                            ){
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断类型,是否与之后类型数组中的其中一个相同
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        private bool TypeEquals(Type dataType,params Type[] types){
            foreach(Type type in types){
                if(dataType.Name.Equals(type.Name)){
                    return true;
                }
            }
            return false;
        }
    }
}