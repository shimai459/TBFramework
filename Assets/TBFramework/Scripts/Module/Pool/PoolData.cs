using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.Pool
{
    /// <summary>
    /// 一个GameObject缓存池的结构类
    /// </summary>
    public class PoolData
    {
        private int maxNumber=PoolSet.POOL_MAX_NUMBER;//缓存池中最大的对象数量
        private GameObject fatherObj;//没有被使用的对象的父节点
        private Stack<GameObject> poolStack;//存没有被使用的存储容器,这里使用了栈(也可以使用队列和泛型List)
        public int ObjCountInPool{
            get{
                return poolStack.Count;
            }
        }

        /// <summary>
        /// 首次创建一种对象的缓存池
        /// </summary>
        /// <param name="firObj">第一个加入缓存池的对像</param>
        /// <param name="poolObj">缓存池的根节点</param>
        public PoolData(GameObject firObj,GameObject poolObj){
            fatherObj=new GameObject(firObj.name+" "+PoolSet.POOL_SINGLE_PARENT_EXTENSION);
            fatherObj.transform.parent=poolObj.transform;
            poolStack=new Stack<GameObject>();
            Push(firObj);
        }
        //从缓存池容器取对象
        public GameObject Pop(Transform fatherObj){
            GameObject obj = poolStack.Pop();
            obj.transform.parent=fatherObj;
            obj.SetActive(true);
            return obj;
        }

        //将对象压入缓存池容器,如果超过最大容量,就将对象销毁
        public void Push(GameObject obj){
            if(poolStack.Count>maxNumber){
                GameObject.Destroy(obj);
                for(int i=0;i<poolStack.Count-maxNumber;i++){
                    GameObject.Destroy(Pop(null));
                }
            }else if(poolStack.Count==maxNumber){
                GameObject.Destroy(obj);
            }else{
            poolStack.Push(obj);
            obj.SetActive(false);
            obj.transform.parent=fatherObj.transform;
            }
        }
        /// <summary>
        /// 设置缓存池最大容量
        /// </summary>
        /// <param name="max">最大容量数</param>
        public void SetMaxNumber(int max){
            maxNumber=max;
        }
    }
}
