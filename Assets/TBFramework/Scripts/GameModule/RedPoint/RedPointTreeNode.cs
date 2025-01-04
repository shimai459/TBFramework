
using System;
using System.Collections.Generic;
using System.Linq;
using TBFramework.Pool;

namespace TBFramework.Game.RedPoint
{
    public class RedPointTreeNode : RedPointTree
    {
        private Dictionary<string, RedPointTreeNode> _nodes = new Dictionary<string, RedPointTreeNode>();

        public string name;

        private int pointCount = 0;

        public RedPointTreeNode() { }

        public RedPointTreeNode(string key, char levelSplite, char lateralSplite, int pointCount)
        {
            this.name = key;
            this.levelSplite = levelSplite;
            this.lateralSplite = lateralSplite;
            this.pointCount = pointCount;
        }

        public void SetRedPointTreeNode(string key, char levelSplite, char lateralSplite, int pointCount)
        {
            this.name = key;
            this.levelSplite = levelSplite;
            this.lateralSplite = lateralSplite;
            this.pointCount = pointCount;
        }

        public override int GetPointCount(string key)
        {
            if (key == "")
            {
                return pointCount;
            }
            return base.GetPointCount(key);
        }

        public override void AddPointCount(string key, int count)
        {
            pointCount += count;
            base.AddPointCount(key, count);
        }

        public override void SubPointCount(string key, int count)
        {
            pointCount -= count;
            pointCount = Math.Max(pointCount, 0);
            base.SubPointCount(key, count);
        }

        public override int GetPointCount(string[] key)
        {
            if (key.Length == 0)
            {
                return pointCount;
            }
            return base.GetPointCount(key);
        }

        public override void AddPointCount(string[] key, int count)
        {
            pointCount += count;
            base.AddPointCount(key, count);
        }

        public override void SubPointCount(string[] key, int count)
        {
            pointCount -= count;
            pointCount = Math.Max(pointCount, 0);
            base.SubPointCount(key, count);
        }
        public override void Reset()
        {
            base.Reset();
            pointCount = default;
        }
    }
}