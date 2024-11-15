using TBFramework.Pool;

namespace TBFramework.AI.FSM
{
    public abstract class FSMKeyBase : CBase
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
