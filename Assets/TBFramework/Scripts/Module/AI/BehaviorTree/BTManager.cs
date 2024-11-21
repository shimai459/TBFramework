using System;
using TBFramework.Pool;

namespace TBFramework.AI.BT
{
    public class BTManager : Singleton<BTManager>
    {
        #region BT
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

        #endregion

        #region BTNode

        public BaseMgrObj<BTNode> nodes = new BaseMgrObj<BTNode>();

        #region Control

        public BTSelector CreateSelector(E_BTControlMemoryType memoryType, bool isRandom, params BTNode[] nodes)
        {
            BTSelector selector = CPoolManager.Instance.Pop<BTSelector>();
            selector.Set(memoryType, isRandom, nodes);
            this.nodes.Create(selector);
            return selector;
        }

        public BTSequence CreateSequence(E_BTControlMemoryType memoryType, bool isRandom, params BTNode[] nodes)
        {
            BTSequence sequence = CPoolManager.Instance.Pop<BTSequence>();
            sequence.Set(memoryType, isRandom, nodes);
            this.nodes.Create(sequence);
            return sequence;
        }

        public BTParallel_ST CreateParallel_ST(E_BTControlMemoryType memoryType, bool isRandom, bool isAllSuccess, bool haveRunningReturnDirectly, bool haveResultReturnDirectly, bool isRunningPriority, params BTNode[] nodes)
        {
            BTParallel_ST parallel = CPoolManager.Instance.Pop<BTParallel_ST>();
            parallel.Set(memoryType, isRandom, nodes);
            parallel.SetParallel(isAllSuccess, haveRunningReturnDirectly, haveResultReturnDirectly, isRunningPriority);
            this.nodes.Create(parallel);
            return parallel;
        }

        public BTParallel_MT CreateParallel_MT(E_BTControlMemoryType memoryType, bool isRandom, bool isAllSuccess, bool isRunningPriority, params BTNode[] nodes)
        {
            BTParallel_MT parallel = CPoolManager.Instance.Pop<BTParallel_MT>();
            parallel.Set(memoryType, isRandom, nodes);
            parallel.SetParallel(isAllSuccess, isRunningPriority);
            this.nodes.Create(parallel);
            return parallel;
        }

        #endregion

        #region Execute

        public BTAction CreateAction(Func<BaseContext, E_BTNodeState> action, E_BTNodeState nullReturnState)
        {
            BTAction btA = CPoolManager.Instance.Pop<BTAction>();
            btA.Set(action, nullReturnState);
            this.nodes.Create(btA);
            return btA;
        }

        public BTCondition CreateCondition(Func<BaseContext, E_BTNodeState> action, E_BTNodeState nullReturnState)
        {
            BTCondition btA = CPoolManager.Instance.Pop<BTCondition>();
            btA.Set(action, nullReturnState);
            this.nodes.Create(btA);
            return btA;
        }

        #endregion

        #region Decoration

        public BTInvert CreateInvert(BTNode node)
        {
            BTInvert invert = CPoolManager.Instance.Pop<BTInvert>();
            invert.SetNode(node);
            this.nodes.Create(invert);
            return invert;
        }

        public BTRepeat CreateRepeat(BTNode node, int repeat)
        {
            BTRepeat repeatNode = CPoolManager.Instance.Pop<BTRepeat>();
            repeatNode.SetNode(node);
            repeatNode.SetRepeat(repeat);
            this.nodes.Create(repeatNode);
            return repeatNode;
        }

        public BTReturnFailure CreateReturnFailure(BTNode node)
        {
            BTReturnFailure returnFailure = CPoolManager.Instance.Pop<BTReturnFailure>();
            returnFailure.SetNode(node);
            this.nodes.Create(returnFailure);
            return returnFailure;
        }

        public BTReturnSuccess CreateReturnSuccess(BTNode node)
        {
            BTReturnSuccess returnSuccess = CPoolManager.Instance.Pop<BTReturnSuccess>();
            returnSuccess.SetNode(node);
            this.nodes.Create(returnSuccess);
            return returnSuccess;
        }

        #endregion

        #endregion

        #region Context

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

        #endregion

    }
}
