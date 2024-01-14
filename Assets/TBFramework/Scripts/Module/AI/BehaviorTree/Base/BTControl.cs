using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.AI.BT
{
    public abstract class BTControl : BTNode
    {
        protected List<BTNode> nodes=new List<BTNode>();

        public void AddNode(BTNode node){
            nodes.Add(node);
        }

        public void AddNodes(params BTNode[] nodes){
            foreach(BTNode node in nodes){
                this.nodes.Add(node);
            }
        }

        public void AddNodes(List<BTNode> nodes){
            foreach(BTNode node in nodes){
                this.nodes.Add(node);
            }
        }

        public void RemoveNode(BTNode node){
            if(nodes.Contains(node)){
                nodes.Remove(node);
            }   
        }

        public void RemoveNodes(params BTNode[] nodes){
            foreach(BTNode node in nodes){
                if(this.nodes.Contains(node)){
                    this.nodes.Remove(node);
                } 
            }
        }

        public void RemoveNodes(List<BTNode> nodes){
            foreach(BTNode node in nodes){
                if(this.nodes.Contains(node)){
                    this.nodes.Remove(node);
                } 
            }
        }
    }
}
