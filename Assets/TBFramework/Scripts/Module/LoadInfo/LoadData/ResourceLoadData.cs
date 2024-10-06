using System;

namespace TBFramework.LoadInfo
{
    public class ResourceLoadData : BaseLoadData
    {
        public string path;
        public Type type;

        public ResourceLoadData(string path, Type type)
        {
            this.path = path;
            this.type = type;
        }


    }
}
