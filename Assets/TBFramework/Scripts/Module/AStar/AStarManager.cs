using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.AStar
{
    public class AStarManager : Singleton<AStarManager>
    {
        public AStarNode[,] nodes;//存储所有格子的数组
        List<AStarNode> openList=new List<AStarNode>();//存加入开放列表的格子
        List<AStarNode> closeList=new List<AStarNode>();//存加入关闭列表的格子
        int mapW;//地图的宽
        int mapH;//地图的高

        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <param name="w">地图的宽</param>
        /// <param name="h">地图的高</param>
        public void InitMap(int w,int h){
            mapW=w;
            mapH=h;
            nodes=new AStarNode[mapW,mapH];
            //确定每个节点通行状态逻辑
            //测试代码:
            for(int i=0;i<mapW;i++){
                for(int j=0;j<mapH;j++){
                    nodes[i,j]=new AStarNode(new Vector2(i,j),(Random.Range(0,100)<40)?E_AStarNodeType.Stop:E_AStarNodeType.Pass);
                }
            }
            //TODO
        }

        /// <summary>
        /// 寻路具体算法
        /// </summary>
        /// <param name="startPos">起点的坐标</param>
        /// <param name="endPos">终点的坐标</param>
        /// <returns>起点到达终点的路径数组</returns>
        public List<AStarNode> FindPath(Vector2 startPos,Vector2 endPos){
            if(startPos.x>=mapW||startPos.x<0||
            startPos.y>=mapH||startPos.y<0||
            endPos.x>=mapW||endPos.x<0||
            endPos.y>=mapH||endPos.y<0){
                return null;
            }
            AStarNode startNode=nodes[Mathf.FloorToInt(startPos.x),Mathf.FloorToInt(startPos.y)];
            AStarNode endNode=nodes[Mathf.FloorToInt(endPos.x),Mathf.FloorToInt(endPos.y)];
            if(startNode.type==E_AStarNodeType.Stop||endNode.type==E_AStarNodeType.Stop){
                return null;
            }

            startNode.startDis=0;
            startNode.endDis=Mathf.Sqrt(Mathf.Pow((endNode.pos.x-startNode.pos.x),2)+Mathf.Pow((endNode.pos.y-startNode.pos.y),2));
            startNode.pathFindConsumption=0;
            startNode.preNode=null;
            closeList.Clear();
            openList.Clear();

            closeList.Add(startNode);
            while(startNode!=endNode){
                //遍历四周点:
                #region 方法一
                //左
                // if(!(startNode.pos.x<1)){
                //     SetNode(nodes[Mathf.FloorToInt(startNode.pos.x-1),Mathf.FloorToInt(startNode.pos.y)]);
                // }
                // //上
                // if(!(startNode.pos.y<1)){
                //     SetNode(nodes[Mathf.FloorToInt(startNode.pos.x),Mathf.FloorToInt(startNode.pos.y-1)]);
                // }
                // //右
                // if(!(startNode.pos.x>mapW)){
                //     SetNode(nodes[Mathf.FloorToInt(startNode.pos.x+1),Mathf.FloorToInt(startNode.pos.y)]);
                // }
                // //下
                // if(!(startNode.pos.y>mapH)){
                //     SetNode(nodes[Mathf.FloorToInt(startNode.pos.x),Mathf.FloorToInt(startNode.pos.y+1)]);
                // }
                // //左上
                // if(!(startNode.pos.x<1)&&!(startNode.pos.y<1)){
                //     SetNode(nodes[Mathf.FloorToInt(startNode.pos.x-1),Mathf.FloorToInt(startNode.pos.y-1)]);
                // }
                // //右上
                // if(!(startNode.pos.x>mapW)&&!(startNode.pos.y<1)){
                //     SetNode(nodes[Mathf.FloorToInt(startNode.pos.x+1),Mathf.FloorToInt(startNode.pos.y-1)]);
                // }
                // //右下
                // if(!(startNode.pos.x>mapW)&&!(startNode.pos.y>mapH)){
                //     SetNode(nodes[Mathf.FloorToInt(startNode.pos.x+1),Mathf.FloorToInt(startNode.pos.y+1)]);
                // }
                // //左下
                // if(!(startNode.pos.x<1)&&!(startNode.pos.y>mapH)){
                //     SetNode(nodes[Mathf.FloorToInt(startNode.pos.x-1),Mathf.FloorToInt(startNode.pos.y+1)]);
                // }
                #endregion

                #region 方法二
                    OpenListAddNode(Mathf.FloorToInt(startNode.pos.x-1),Mathf.FloorToInt(startNode.pos.y));//左
                    OpenListAddNode(Mathf.FloorToInt(startNode.pos.x),Mathf.FloorToInt(startNode.pos.y-1));//上
                    OpenListAddNode(Mathf.FloorToInt(startNode.pos.x+1),Mathf.FloorToInt(startNode.pos.y));//右
                    OpenListAddNode(Mathf.FloorToInt(startNode.pos.x),Mathf.FloorToInt(startNode.pos.y+1));//下
                    OpenListAddNode(Mathf.FloorToInt(startNode.pos.x-1),Mathf.FloorToInt(startNode.pos.y-1));//左上
                    OpenListAddNode(Mathf.FloorToInt(startNode.pos.x+1),Mathf.FloorToInt(startNode.pos.y-1));//右上
                    OpenListAddNode(Mathf.FloorToInt(startNode.pos.x+1),Mathf.FloorToInt(startNode.pos.y+1));//右下
                    OpenListAddNode(Mathf.FloorToInt(startNode.pos.x-1),Mathf.FloorToInt(startNode.pos.y+1));//左下

                    if(openList.Count==0){
                        break;
                    }
                #endregion

                //找到寻路消耗最短的点:
                #region 方法一:通过遍历openList去找最小点
                    // AStarNode minNode=openList[0];
                    // for(int i=1;i<openList.Count;i++){
                    //     if(openList[i].pathFindConsumption<minNode.pathFindConsumption){
                    //         minNode=openList[i];
                    //     }
                    // }
                    // startNode=minNode;
                    // closeList.Add(startNode);
                    // openList.Remove(startNode);
                #endregion

                #region 方法二:使用List的自定义排序方法对openList进行升序排序,得到openList的第一个对象就是最小的对象
                    openList.Sort(OpenListSort);
                    startNode=openList[0];
                    closeList.Add(startNode);
                    openList.RemoveAt(0);
                #endregion
            }

            AStarNode lastNode;
            if(startNode==endNode){
                lastNode=endNode;
            }else{
                foreach(AStarNode n in closeList){
                    if(n.endDis<startNode.endDis){
                        startNode=n;
                    }
                }
                lastNode=startNode;
            }

            List<AStarNode> path=new List<AStarNode>();
            path.Add(lastNode);
            while(lastNode.preNode!=null){
                lastNode=lastNode.preNode;
                path.Add(lastNode);
            }
            path.Reverse();

            return path;
            #region 遍历四周点方法一的函数
                // void SetNode(AStarNode node){
                //     if(node.type==AStarNodeType.Pass&&!openList.Contains(node)&&!closeList.Contains(node)){
                //         node.preNode=startNode;
                //         node.startDis=Mathf.Sqrt(Mathf.Pow((startNode.pos.x-node.pos.x),2)+Mathf.Pow((startNode.pos.y-node.pos.y),2))+startNode.startDis;
                //         node.endDis=Mathf.Sqrt(Mathf.Pow((endNode.pos.x-node.pos.x),2)+Mathf.Pow((endNode.pos.y-node.pos.y),2));
                //         node.pathFindConsumption=node.startDis+node.endDis;
                //         openList.Add(node);
                //     }
                // }
            #endregion
            
            #region 遍历四周点方法二的函数
                void OpenListAddNode(int x,int y){
                    if(x<0||x>=mapW||y<0||y>=mapH){
                        return;
                    }
                    AStarNode node=nodes[x,y];
                    if(node.type==E_AStarNodeType.Pass&&!openList.Contains(node)&&!closeList.Contains(node)){
                        node.preNode=startNode;
                        node.startDis=Mathf.Sqrt(Mathf.Pow((startNode.pos.x-node.pos.x),2)+Mathf.Pow((startNode.pos.y-node.pos.y),2))+startNode.startDis;
                        node.endDis=Mathf.Sqrt(Mathf.Pow((endNode.pos.x-node.pos.x),2)+Mathf.Pow((endNode.pos.y-node.pos.y),2));
                        node.pathFindConsumption=node.startDis+node.endDis;
                        openList.Add(node);
                    }
                }
            #endregion
        }

        //用于开放数组List排序函数
        private int OpenListSort(AStarNode a, AStarNode b){
            if(a.pathFindConsumption>b.pathFindConsumption){
                return 1;
            }else{
                return -1;
            }
        }
    }
}