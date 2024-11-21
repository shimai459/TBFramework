
using System.Collections.Generic;


namespace TBFramework.AI.BT
{
    public abstract class BTControl : BTNode
    {
        protected List<BTNode> nodes = new List<BTNode>();

        protected E_BTControlMemoryType memoryType;

        protected int index = 0;

        protected bool isRandom = false;

        private System.Random random = new System.Random();

        public void Set(E_BTControlMemoryType memoryType, bool isRandom, params BTNode[] nodes)
        {
            this.isRandom = isRandom;
            if (isRandom)
            {
                random = new System.Random();
            }
            index = 0;
            this.memoryType = memoryType;
            this.AddNodes(nodes);
        }

        protected void Random()
        {
            List<BTNode> shuffledNodes = new List<BTNode>(nodes);
            BTNode temp;
            for (int i = shuffledNodes.Count - 1; i > 0; i--)
            {
                int k = random.Next(i);
                temp = shuffledNodes[k];
                shuffledNodes[k] = shuffledNodes[i];
                shuffledNodes[i] = temp;
            }
            nodes = shuffledNodes;
        }

        public void AddNode(BTNode node)
        {
            nodes.Add(node);
        }

        public void AddNodes(params BTNode[] nodes)
        {
            foreach (BTNode node in nodes)
            {
                this.nodes.Add(node);
            }
        }

        public void AddNodes(List<BTNode> nodes)
        {
            foreach (BTNode node in nodes)
            {
                this.nodes.Add(node);
            }
        }

        public void RemoveNode(BTNode node)
        {
            if (nodes.Contains(node))
            {
                nodes.Remove(node);
            }
        }

        public void RemoveNodes(params BTNode[] nodes)
        {
            foreach (BTNode node in nodes)
            {
                if (this.nodes.Contains(node))
                {
                    this.nodes.Remove(node);
                }
            }
        }

        public void RemoveNodes(List<BTNode> nodes)
        {
            foreach (BTNode node in nodes)
            {
                if (this.nodes.Contains(node))
                {
                    this.nodes.Remove(node);
                }
            }
        }

        public void Clear()
        {
            foreach (BTNode node in nodes)
            {
                BTManager.Instance.nodes.Destory(node);
            }
            nodes.Clear();
        }

        public override void Reset()
        {
            this.Clear();
            index = 0;
            random = null;
        }
    }
}
