using TBFramework.Mono;
using TBFramework.Pool;

namespace TBFramework.AI.BT
{
    public class BehaviorTree : KeyBase
    {
        private BTNode root;//行为树的根节点

        private BaseContext context;//行为树的共享数据

        private bool isAddListen = false;

        public BehaviorTree()
        {
            MonoConManager.Instance.AddUpdateListener(Update);
            isAddListen = true;
        }

        public void Set(BTNode root, BaseContext context)
        {
            if (!isAddListen)
            {
                MonoConManager.Instance.AddUpdateListener(Update);
                isAddListen = true;
            }
            BTManager.Instance.nodes.Destory(this.root);
            BTManager.Instance.contexts.Destory(this.context);
            this.root = root;
            this.context = context;
            BTManager.Instance.nodes.AddUse(root);
            BTManager.Instance.contexts.AddUse(context);
        }

        public BaseContext GetContext()
        {
            return context;
        }

        private void Update()
        {
            root?.Evaluate(context);
        }

        public override void Reset()
        {
            MonoConManager.Instance.RemoveUpdateListener(Update);
            isAddListen = false;
            BTManager.Instance.nodes.Destory(root);
            BTManager.Instance.contexts.Destory(context);
            root = null;
            context = null;
        }
    }
}
