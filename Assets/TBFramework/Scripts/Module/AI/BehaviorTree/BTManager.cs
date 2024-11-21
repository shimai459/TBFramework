using TBFramework.Pool;

namespace TBFramework.AI.BT
{
    public class BTManager : Singleton<BTManager>
    {
        //#region BT
        public BaseMgrObj<BehaviorTree> bts = new BaseMgrObj<BehaviorTree>();

        public BehaviorTree CreateBT(BTNode root, BaseContext context)
        {
            BehaviorTree bt = CPoolManager.Instance.Pop<BehaviorTree>();
            bt.Set(root, context);
            bts.Create(bt);
            bts.AddUse(bt);
            nodes.AddUse(root);
            contexts.AddUse(context);
            return bt;
        }

        //#endregion

        //region BTNode

        public BaseMgrObj<BTNode> nodes = new BaseMgrObj<BTNode>();

        public BTControl CreateControl(bool isRandom, E_BTControlMemoryType memoryType)
        {
            return null;
        }

        //endregion

        //region Context

        public BaseMgrObj<BaseContext> contexts = new BaseMgrObj<BaseContext>();

        public ContextWithTParam<T> CreateContext<T>(T param)
        {
            ContextWithTParam<T> context = CPoolManager.Instance.Pop<ContextWithTParam<T>>();
            context.Set(param);
            contexts.Create(context);
            return context;
        }

        public ContextWithDic CreateContext()
        {
            ContextWithDic context = CPoolManager.Instance.Pop<ContextWithDic>();
            contexts.Create(context);
            return context;
        }

        //#endregion

    }
}
