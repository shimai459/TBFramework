

using System.Collections.Generic;
using TBFramework.Pool;

namespace TBFramework.Game.RedPoint
{
    public class RedPointManager : Singleton<RedPointManager>
    {

        public BaseMgrObj<RedPointTree> trees = new BaseMgrObj<RedPointTree>();

        public RedPointTree CreateTree(char lateralSplite, char levelSplite)
        {
            RedPointTree tree = CPoolManager.Instance.Pop<RedPointTree>();
            tree.SetPointTree(levelSplite, lateralSplite);
            trees.Create(tree);
            return tree;
        }

    }
}