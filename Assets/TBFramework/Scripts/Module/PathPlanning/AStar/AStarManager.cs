
using System;
using System.Collections.Generic;
using TBFramework.Pool;

namespace TBFramework.PathPlanning.AStar
{
    public class AStarManager : Singleton<AStarManager>
    {
        public BaseMgrObj<BaseAStar> astars = new BaseMgrObj<BaseAStar>();

        public AStar<T> CreateAStar<T>(List<T> nodes, Func<T, List<T>, List<T>> getNerighbors, Func<T, T, List<T>, int> heuristic, bool notFindEndReturnPath, bool isAddUseMeself = false)
        {
            AStar<T> aStar = CPoolManager.Instance.Pop<AStar<T>>();
            aStar.Init(nodes, getNerighbors, heuristic, notFindEndReturnPath);
            astars.Create(aStar);
            if (isAddUseMeself)
            {
                astars.AddUse(aStar);
            }
            return aStar;
        }
    }
}