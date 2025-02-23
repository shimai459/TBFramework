

namespace TBFramework.Pool
{
    public abstract class KeyBase : CBase
    {
        public int key;
        public bool inPool;
        public void SetKey(int key)
        {
            this.key = key;
            this.inPool = false;
        }

        public override void Reset()
        {
            this.key = default(int);
            this.inPool = true;
        }

        public static bool IsLegal(KeyBase obj) => obj != null && !obj.inPool;
    }
}
