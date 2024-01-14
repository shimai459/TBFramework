using System.Xml.Serialization;
using System.IO;
using System;

namespace TBFramework.Data.Xml
{
    public class XDataManager : Singleton<XDataManager>
    {
        #region XML方式进行数据的存储和读取
            
            #region 普通用于用户存储读取的方法
            
            #region 说明
            /*主要使用的类:XmlSerializer(用于序列化对象),StreamWriter(用于将数据写入文件),StreamReader(用于将数据从文件读出),using(方便流对象的释放和销毁)
            特点:
            1.只能存储公共成员,要存储字典的话,要使用我们重新定义的字典SDictionary
            2.将一个数据以属性的形式存储,要在是属性的字段前添加特性[XmlAttribute(可选填,填就会改变存储时的属性名)]
            3.同样元素需要改名,也需要在字段前加特性[XmlElement(可选填,填就会改变存储时的元素名)]
            4.改变数组内部单个数据的元素名,在数组前添加特性[XmlArrayItem(填入单个数据的元素名)]
            5.该数组的元素名,则在数组前添加特性[XmlArray(填入数组的元素名)]
            6.反序列化时,如果List中有默认值,不会覆盖,而是往后面加值,所以建议写List时建议不要给默认值
            7.且如果对象为引用类型为null,则不会被写入文件中
            8.继承IXmlSerializable接口,实现里面的方法,可以自定义类的序列化和反序列化的规则,其中要使用到XmlWriter和XmlReader中的方法,注意读写的规则要一致,否则会出错
            写的方法:
            WriteAttributeString(写属性)
            WriteElementString(写元素)
            WriterStartElement和WriterEndElement(这两者之间写包裹的节点),可以使用XmlSerializer的序列化方法调用其他类型的默认的序列化方法去写
            读的方法:
            使用索引器加入属性名去读取具体的数据(读属性)
            Read()去一步步读(顺序是:当前元素头部包裹节点->元素包裹的内容->元素尾部包裹节点)
            读包裹节点实现需要Read()一次去跳过根节点ReaderStartElement和ReaderEndElement(这两者之间读包裹的节点),可以使用XmlSerializer的反序列化方法调用其他类型的默认的反序列化方法去读
            */
            #endregion

            /// <summary>
            /// 将一个数据按XML格式存储到文件中
            /// </summary>
            /// <param name="data">要存储的数据</param>
            /// <param name="fileName">存储的数据文件名</param>
            public void Save(object data,string fileName,bool isConfig=false){
                if(!Directory.Exists(XDataSet.XML_DATA_PATH)){
                    Directory.CreateDirectory(XDataSet.XML_DATA_PATH);
                }
                if(!Directory.Exists(XDataSet.XML_DATA_CONFIGURATION_PATH)){
                    Directory.CreateDirectory(XDataSet.XML_DATA_CONFIGURATION_PATH);
                }
                string path=Path.Combine(XDataSet.XML_DATA_PATH,fileName+".xml");
                if(isConfig){
                    path=Path.Combine(XDataSet.XML_DATA_CONFIGURATION_PATH,fileName+".xml");
                }
                //使用C#自带的XML文件序列化类进行序列化,并用StreamWriter写入指定文件中
                using(StreamWriter writer =new StreamWriter(path)){
                    XmlSerializer s=new XmlSerializer(data.GetType());
                    s.Serialize(writer,data);
                }
            }
            
            /// <summary>
            /// 从一个文件中按XML格式将数据读出来
            /// </summary>
            /// <param name="type">数据的类型</param>
            /// <param name="fileName">存储数据的文件名</param>
            /// <returns></returns>
            public object Load(Type type,string fileName){
                string path=Path.Combine(XDataSet.XML_DATA_PATH,fileName+".xml");
                //判断是否有这个文件,没有就直接返回一个默认是数据类型
                if(!File.Exists(path)){
                    path=Path.Combine(XDataSet.XML_DATA_CONFIGURATION_PATH,fileName+".xml");
                    if(!File.Exists(path)){
                        return Activator.CreateInstance(type);
                    }
                }
                //使用C#自带的XML文件反序列化类进行反序列化,并用StreamReader从指定文件中读出返回
                using(StreamReader reader=new StreamReader(path)){
                    XmlSerializer s=new XmlSerializer(type);
                    return s.Deserialize(reader);
                }
            }

            /// <summary>
            /// 从一个文件中按XML格式将数据读出来的泛型方法
            /// </summary>
            /// <param name="fileName">存储数据的文件名</param>
            /// <typeparam name="T">数据的类型</typeparam>
            /// <returns></returns>
            public T Load<T>(string fileName){
                string path=Path.Combine(XDataSet.XML_DATA_PATH,fileName+".xml");
                if(!File.Exists(path)){
                    path=Path.Combine(XDataSet.XML_DATA_CONFIGURATION_PATH,fileName+".xml");
                    if(!File.Exists(path)){
                        return Activator.CreateInstance<T>();
                    }
                }
                using(StreamReader reader=new StreamReader(path)){
                    XmlSerializer s=new XmlSerializer(typeof(T));
                    return (T)s.Deserialize(reader);
                }
            }
            
            #endregion
        
            
        
        #endregion
    }
}