using TBFramework.Pool;

namespace TBFramework.AI
{
    public abstract class KeyBase : CBase
    {
        public int key;
        public void SetKey(int key)
        {
            this.key = key;
        }

        public override void Reset()
        {
            this.key = default(int);
        }
    }
}
