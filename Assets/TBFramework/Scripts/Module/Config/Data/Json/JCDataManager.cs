using System.IO;
using System;
using System.Collections.Generic;

namespace TBFramework.Config.Data.Json
{
    public class JCDataManager : Singleton<JCDataManager>
    {
        #region 用于存储读取自动化生成数据文件的方法
            /// <summary>
            /// 存储所有由Excel自动生成的Json文件的数据
            /// </summary>
            /// <typeparam name="string">数据容器类类名</typeparam>
            /// <typeparam name="object">数据容器类对象</typeparam>
            /// <returns></returns>
            private Dictionary<string,object> tableDic=new Dictionary<string, object>();
            
            /// <summary>
            /// 初始化tableDicJ,将自动生成的所有Json数据文件都读到内存中
            /// </summary>
            [Obsolete("方法未实现",true)]
            public void InitTable(){
                FileInfo[] infos=Directory.CreateDirectory(JCDataSet.JSON_DATA_EXCEL_PATH).GetFiles();
                if(infos.Length==0){
                    return;
                }
                for(int i=0;i<infos.Length;i++){
                    if(infos[i].Extension.ToLower()!=".json"){
                        continue;
                    }
                    LoadTable(Type.GetType(infos[i].Name.Split('.')[0]+CDataSet.DATA_CONTAINER_EXTENSION),Type.GetType(infos[i].Name.Split('.')[0]));
                }
            }
            
            /// <summary>
            /// 把Excel文件自动生成的Json文件读到内存中的泛型方法
            /// </summary>
            /// <typeparam name="T">数据容器类类型</typeparam>
            /// <typeparam name="K">数据结构类类型</typeparam>
            [Obsolete("方法未实现",true)]
            public void LoadTable<T,K>(){
                //TODO
            }
            
            /// <summary>
            /// 把Excel文件自动生成的Json文件读到内存中
            /// </summary>
            /// <param name="containerType">数据容器类类型</param>
            /// <param name="classType">数据结构类类型</param>
            [Obsolete("方法未实现",true)]
            public void LoadTable(Type containerType,Type classType){
                //TODO
            }

            /// <summary>
            /// 获取一个Json数据文件生成的数据容器类表
            /// </summary>
            /// <param name="type">数据容器类类型</param>
            /// <returns></returns>
            [Obsolete("方法未实现",true)]
            public object GetTable(Type type){
                //TODO
                return null;
            }
            
            /// <summary>
            /// 获取一个Json数据文件生成的数据容器类表的泛型方法
            /// </summary>
            /// <typeparam name="T">数据容器类类型</typeparam>
            /// <returns></returns>
            [Obsolete("方法未实现",true)]
            public T GetTable<T>(){
                //TODO
                return default(T);
            }
            
            #endregion
    }
}
