using System.IO;
using UnityEngine;

namespace TBFramework.Config.Data
{
    public static class ExcelSet
    {
        #region 设置路径和制表规则

                #region 制表规则
                
                    /// <summary>
                    /// 真正数据开始行
                    /// </summary>
                    public readonly static int BEGIN_DATA_ROW=4;
                    
                    /// <summary>
                    /// 变量名行
                    /// </summary>
                    public readonly static int VARIABLE_NAME_ROW=0;
                    
                    /// <summary>
                    /// 变量类型行
                    /// </summary>
                    public readonly static int VARIABLE_TYPE_ROW=1;
                    
                    /// <summary>
                    /// 主键所在行
                    /// </summary>
                    public readonly static int KEY_ROW=2;
                    
                    /// <summary>
                    /// 主键的标识
                    /// </summary>
                    public readonly static string KEY_NAME="key";
                #endregion
                
                #region 自动生成文件路径
                
                    /// <summary>
                    /// 读取Excel文件存放的位置
                    /// </summary>
                    /// <returns></returns>
                    public readonly static string DATA_EXCEL_PATH=Path.Combine(Application.dataPath,"TBFramework","Automation","Excel","ConfigurationTable");

                    /// <summary>
                    /// 存放由Excel文件自动生成的数据结构类的位置
                    /// </summary>
                    /// <returns></returns>
                    public readonly static string DATA_CLASS_PATH=Path.Combine(Application.dataPath,"TBFramework","Automation","Scripts","ExcelData","ConfigurationTable","DataClass");
                    
                    /// <summary>
                    /// 存放由Excel文件自动生成的数据容器类的位置
                    /// </summary>
                    /// <returns></returns>
                    public readonly static string DATA_CONTAINER_PATH=Path.Combine(Application.dataPath,"TBFramework","Automation","Scripts","ExcelData","ConfigurationTable","DataContainer");

                    
                
                #endregion

            #endregion
    }
}
