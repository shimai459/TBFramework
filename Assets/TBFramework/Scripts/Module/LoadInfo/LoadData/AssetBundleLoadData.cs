
using System;

namespace TBFramework.LoadInfo
{
    public class AssetBundleLoadData : BaseLoadData
    {
        public string pathURL;
        public string mainName;
        public string abName;
        public string resName;
        public Type type;

        public AssetBundleLoadData(string pathURL, string mainName, string abName, string resName, Type type)
        {
            this.pathURL = pathURL;
            this.mainName = mainName;
            this.abName = abName;
            this.resName = resName;
            this.type = type;
        }


    }
}
