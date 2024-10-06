using TBFramework.Pool;

namespace TBFramework.LoadInfo
{
    public class LoadInfo
    {
        public string name;
        public E_LoadType loadType;
        public BaseLoadData loadData;

        public LoadInfo(string name, E_LoadType loadType, BaseLoadData loadData)
        {
            this.name = name;
            this.loadType = loadType;
            this.loadData = loadData;
        }
    }
}
