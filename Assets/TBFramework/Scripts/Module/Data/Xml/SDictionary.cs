using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Schema;
using System.Xml;

namespace TBFramework.Data.Xml
{
    public class SDictionary<TKey,TValue> : Dictionary<TKey,TValue>,IXmlSerializable
    {
        //该方法中的内容直接返回空,这只是自定义XML序列化和反序列化规则的规范
        public XmlSchema GetSchema()
        {
            return null;
        }

        //反序列化
        public void ReadXml(XmlReader reader)
        {
            //用于反序列化的工具类
            XmlSerializer keySer=new XmlSerializer(typeof(TKey));
            XmlSerializer valueSer=new XmlSerializer(typeof(TValue));
            //跳过根节点
            reader.Read();
            //一直循环到结束节点
            while(reader.NodeType!=XmlNodeType.EndElement){
                //反序列化键,用key类型自带的反序列化方法
                TKey key=(TKey)keySer.Deserialize(reader);
                //反序列化值,用value类型自带的反序列化方法
                TValue value=(TValue)valueSer.Deserialize(reader);
                //将读取到的数据保存到字典中
                this.Add(key,value);
            }

        }
        //序列化
        public void WriteXml(XmlWriter writer)
        {
            //用于序列化的工具类
            XmlSerializer keySer=new XmlSerializer(typeof(TKey));
            XmlSerializer valueSer=new XmlSerializer(typeof(TValue));
            //遍历字典,将其的键值一起序列化存储到文件中
            foreach(KeyValuePair<TKey,TValue> kv in this){
                //序列化键,用key类型自带的序列化方法
                keySer.Serialize(writer,kv.Key);
                //序列化值,用value类型自带的序列化方法
                valueSer.Serialize(writer,kv.Value);
            }
        }
    }
}
