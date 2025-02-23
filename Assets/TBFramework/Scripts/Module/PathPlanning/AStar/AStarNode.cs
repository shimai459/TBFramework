
using System;
using TBFramework.Pool;

namespace TBFramework.PathPlanning.AStar
{
    public class AStarNode<T> : CBase, IComparable<AStarNode<T>>
    {
        public T data;

        public int gCost;

        public int hCost;

        public int cost => gCost + hCost;

        public AStarNode<T> parent;

        public void Set(T data, int gCost, int hCost, AStarNode<T> parent)
        {
            this.data = data;
            this.gCost = gCost;
            this.hCost = hCost;
            this.parent = parent;
        }

        public override void Reset()
        {
            data = default;
            gCost = default;
            hCost = default;
            parent = null;
        }

        public int CompareTo(AStarNode<T> other)
        {
            return this.cost.CompareTo(other.cost);
        }
    }
}