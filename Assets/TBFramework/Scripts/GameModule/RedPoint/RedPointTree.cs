
using System;
using System.Collections.Generic;
using TBFramework.Pool;

namespace TBFramework.Game.RedPoint
{
    public class RedPointTree : KeyBase
    {

        private Dictionary<string, RedPointTreeNode> _nodes = new Dictionary<string, RedPointTreeNode>();

        public char levelSplite;//层级分隔符

        public char lateralSplite;//同一层级分隔符

        public RedPointTree() { }

        public RedPointTree(char levelSplite, char lateralSplite)
        {
            this.SetPointTree(levelSplite, lateralSplite);
        }

        public void SetPointTree(char levelSplite, char lateralSplite)
        {
            this.levelSplite = levelSplite;
            this.lateralSplite = lateralSplite;
        }

        public virtual int GetPointCount(string key)
        {
            if (key == "")
            {
                return 0;
            }
            else
            {
                string[] keys = key.Split(lateralSplite);
                string[] sKeys = keys[0].Split(levelSplite);
                if (!_nodes.ContainsKey(sKeys[0]))
                {
                    return 0;
                }
                else
                {
                    if (sKeys.Length == 1)
                    {
                        return _nodes[sKeys[0]].GetPointCount("");
                    }
                    else
                    {
                        string[] sKeys2 = new string[sKeys.Length - 1];
                        Array.Copy(sKeys, 1, sKeys2, 0, sKeys2.Length);
                        return _nodes[sKeys[0]].GetPointCount(sKeys2);
                    }

                }

            }
        }

        public virtual void AddPointCount(string key, int count)
        {
            if (key == "")
            {
                return;
            }
            string[] keys = key.Split(lateralSplite);
            for (int i = 0; i < keys.Length; i++)
            {
                string[] sKeys = keys[i].Split(levelSplite);
                if (!_nodes.ContainsKey(sKeys[0]))
                {
                    RedPointTreeNode node = CPoolManager.Instance.Pop<RedPointTreeNode>();
                    node.SetRedPointTreeNode(sKeys[0], levelSplite, lateralSplite, 0);
                    _nodes.Add(sKeys[0], node);
                }
                string[] sKeys2 = new string[sKeys.Length - 1];
                Array.Copy(sKeys, 1, sKeys2, 0, sKeys2.Length);
                _nodes[sKeys[0]].AddPointCount(sKeys2, count);
            }
        }

        public virtual void SubPointCount(string key, int count)
        {
            if (key == "")
            {
                return;
            }
            string[] keys = key.Split(lateralSplite);
            for (int i = 0; i < keys.Length; i++)
            {
                string[] sKeys = keys[i].Split(levelSplite);
                if (!_nodes.ContainsKey(sKeys[0]))
                {
                    RedPointTreeNode node = CPoolManager.Instance.Pop<RedPointTreeNode>();
                    node.SetRedPointTreeNode(sKeys[0], levelSplite, lateralSplite, 0);
                    _nodes.Add(sKeys[0], node);
                }
                string[] sKeys2 = new string[sKeys.Length - 1];
                Array.Copy(sKeys, 1, sKeys2, 0, sKeys2.Length);
                _nodes[sKeys[0]].SubPointCount(sKeys2, count);
            }
        }

        public virtual int GetPointCount(string[] key)
        {
            if (key.Length == 0)
            {
                return 0;
            }
            else
            {
                if (!_nodes.ContainsKey(key[0]))
                {
                    return 0;
                }
                else
                {
                    if (key.Length == 1)
                    {
                        return _nodes[key[0]].GetPointCount("");
                    }
                    else
                    {
                        string[] sKeys2 = new string[key.Length - 1];
                        Array.Copy(key, 1, sKeys2, 0, sKeys2.Length);
                        return _nodes[key[0]].GetPointCount(sKeys2);
                    }
                }
            }
        }

        public virtual void AddPointCount(string[] key, int count)
        {
            if (key.Length == 0)
            {
                return;
            }
            if (!_nodes.ContainsKey(key[0]))
            {
                RedPointTreeNode node = CPoolManager.Instance.Pop<RedPointTreeNode>();
                node.SetRedPointTreeNode(key[0], levelSplite, lateralSplite, 0);
                _nodes.Add(key[0], node);
            }
            string[] sKeys2 = new string[key.Length - 1];
            Array.Copy(key, 1, sKeys2, 0, sKeys2.Length);
            _nodes[key[0]].AddPointCount(sKeys2, count);
        }

        public virtual void SubPointCount(string[] key, int count)
        {
            if (key.Length == 0)
            {
                return;
            }
            if (!_nodes.ContainsKey(key[0]))
            {
                RedPointTreeNode node = CPoolManager.Instance.Pop<RedPointTreeNode>();
                node.SetRedPointTreeNode(key[0], levelSplite, lateralSplite, 0);
                _nodes.Add(key[0], node);
            }
            string[] sKeys2 = new string[key.Length - 1];
            Array.Copy(key, 1, sKeys2, 0, sKeys2.Length);
            _nodes[key[0]].SubPointCount(sKeys2, count);
        }


        public void AddNode(string key)
        {
            if (key == "")
            {
                return;
            }
            string[] keys = key.Split(lateralSplite);
            for (int i = 0; i < keys.Length; i++)
            {
                string[] sKeys = keys[i].Split(levelSplite);
                if (!_nodes.ContainsKey(sKeys[0]))
                {
                    RedPointTreeNode node = CPoolManager.Instance.Pop<RedPointTreeNode>();
                    node.SetRedPointTreeNode(sKeys[0], levelSplite, lateralSplite, 0);
                    _nodes.Add(sKeys[0], node);
                }
                string[] sKeys2 = new string[sKeys.Length - 1];
                Array.Copy(sKeys, 1, sKeys2, 0, sKeys2.Length);
                _nodes[sKeys[0]].AddNode(sKeys2);
            }
        }

        public void RemoveNode(string key)
        {
            if (key == "")
            {
                return;
            }
            string[] keys = key.Split(lateralSplite);
            for (int i = 0; i < keys.Length; i++)
            {
                string[] sKeys = keys[i].Split(levelSplite);
                if (_nodes.ContainsKey(sKeys[0]))
                {
                    if (sKeys.Length > 1)
                    {
                        string[] sKeys2 = new string[sKeys.Length - 1];
                        Array.Copy(sKeys, 1, sKeys2, 0, sKeys2.Length);
                        _nodes[sKeys[0]].RemoveNode(sKeys2);
                    }
                    else
                    {
                        CPoolManager.Instance.Push(_nodes[sKeys[0]]);
                        _nodes.Remove(sKeys[0]);
                    }

                }

            }
        }

        public void AddNode(string[] key)
        {
            if (key.Length == 0)
            {
                return;
            }
            if (!_nodes.ContainsKey(key[0]))
            {
                RedPointTreeNode node = CPoolManager.Instance.Pop<RedPointTreeNode>();
                node.SetRedPointTreeNode(key[0], levelSplite, lateralSplite, 0);
                _nodes.Add(key[0], node);
            }
            string[] sKeys2 = new string[key.Length - 1];
            Array.Copy(key, 1, sKeys2, 0, sKeys2.Length);
            _nodes[key[0]].AddNode(sKeys2);
        }

        public void RemoveNode(string[] key)
        {
            if (key.Length == 0)
            {
                return;
            }
            if (_nodes.ContainsKey(key[0]))
            {
                if (key.Length > 1)
                {
                    string[] sKeys2 = new string[key.Length - 1];
                    Array.Copy(key, 1, sKeys2, 0, sKeys2.Length);
                    _nodes[key[0]].RemoveNode(sKeys2);
                }
                else
                {
                    CPoolManager.Instance.Push(_nodes[key[0]]);
                    _nodes.Remove(key[0]);
                }

            }

        }

        public void ClearNode()
        {
            foreach (RedPointTreeNode node in _nodes.Values)
            {
                CPoolManager.Instance.Push(node);
            }
            _nodes.Clear();
        }

        public override void Reset()
        {
            this.lateralSplite = default;
            this.levelSplite = default;
            this.ClearNode();
        }

    }
}