using System.IO;

namespace TBFramework.UI
{
    public class UIResourceSet
    {
        /// <summary>
        /// 总UI存储路径
        /// </summary>
        /// <returns></returns>
        public static string UI_PATH=Path.Combine("UI");
        /// <summary>
        /// 预制体存放路径
        /// </summary>
        /// <returns></returns>
        public static string PREFAB_PATH=Path.Combine("Prefabs");
        /// <summary>
        /// Panel的存放路径
        /// </summary>
        /// <returns></returns>
        public static string PANEL_PATH=Path.Combine("UI","Panels");
    }
}