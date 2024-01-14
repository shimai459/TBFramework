using UnityEngine;

namespace TBFramework.AStar
{
    /// <summary>
    /// A*寻路格子类
    /// </summary>
    public class AStarNode
    {
        public Vector2 pos;//格子对象的坐标
        public float pathFindConsumption;//寻路消耗
        public float startDis;//离起点的距离
        public float endDis;//离终点的距离
        public AStarNode preNode;//前一个格子
        public E_AStarNodeType type;//格子的通行类型
        
        /// <summary>
        /// 格子的构造函数
        /// </summary>
        /// <param name="pos">格子的坐标</param>
        /// <param name="type">格子的类型</param>
        public AStarNode(Vector2 pos,E_AStarNodeType type){
            this.pos=pos;
            this.type= type;
        }

    }
}
