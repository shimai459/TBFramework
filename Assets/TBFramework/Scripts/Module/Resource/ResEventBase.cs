namespace TBFramework.Resource
{
    public abstract class ResEventBase
    {
        protected int refCount = 0;

        public int RefCount
        {
            get { return refCount; }
        }
    }
}
