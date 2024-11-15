using TBFramework.Pool;

namespace TBFramework.AI.FSM.Detail
{
    public abstract class FSMDKeyBase : CBase
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
