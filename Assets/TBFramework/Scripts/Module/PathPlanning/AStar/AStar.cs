using System;
using System.Collections.Generic;
using TBFramework.Net;
using TBFramework.Pool;

namespace TBFramework.PathPlanning.AStar
{
    public class AStar<T> : BaseAStar
    {
        public List<T> nodes = new List<T>();

        public Func<T, List<T>, List<T>> getNerighbors;

        public Func<T, T, List<T>, int> heuristic;

        public bool notFindEndReturnPath = true;

        public void Init(List<T> nodes, Func<T, List<T>, List<T>> getNerighbors, Func<T, T, List<T>, int> heuristic, bool notFindEndReturnPath)
        {
            this.nodes = nodes;
            this.getNerighbors = getNerighbors;
            this.heuristic = heuristic;
            this.notFindEndReturnPath = notFindEndReturnPath;
        }

        public List<T> FindPath(T start, T end)
        {
            List<AStarNode<T>> openList = new List<AStarNode<T>>();
            List<AStarNode<T>> closedList = new List<AStarNode<T>>();
            AStarNode<T> startNode = CPoolManager.Instance.Pop<AStarNode<T>>();
            startNode.Set(start, 0, heuristic(start, end, nodes), null);
            closedList.Add(startNode);
            List<T> nerighbors;
            while (!startNode.data.Equals(end))
            {
                nerighbors = getNerighbors(startNode.data, nodes);
                foreach (T neighbor in nerighbors)
                {
                    AStarNode<T> node = CPoolManager.Instance.Pop<AStarNode<T>>();
                    node.Set(neighbor, startNode.gCost + heuristic(startNode.data, neighbor, nodes), heuristic(neighbor, end, nodes), startNode);
                    openList.Add(node);
                }
                openList.Sort();
                startNode = openList[0];
                openList.RemoveAt(0);
                closedList.Add(startNode);
            }
            if (notFindEndReturnPath || startNode.data.Equals(end))
            {
                return GetPath(startNode);
            }
            return null;
        }

        private List<T> GetPath(AStarNode<T> node)
        {
            List<T> path = new List<T>();
            while (node != null)
            {
                path.Add(node.data);
                node = node.parent;
            }
            path.Reverse();
            return path;
        }


    }
}