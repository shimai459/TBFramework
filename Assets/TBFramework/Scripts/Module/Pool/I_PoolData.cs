using UnityEngine;

namespace TBFramework.Pool
{
    public abstract class I_PoolData{
        protected int maxNumber;//缓存池中最大的对象数量
        protected E_PoolMaxType maxType;//缓存池中对象数量的最大类型
        protected GameObject fatherObj;//没有被使用的对象的父节点

        public GameObject FatherObj{
            get{ return fatherObj;}
        }

        /// <summary>
        /// 设置缓存池最大容量
        /// </summary>
        /// <param name="max">最大容量数</param>
        public void SetMaxNumber(int max)
        {
            maxNumber = max;
        }

        /// <summary>
        /// 设置缓存池中对象数量的最大类型
        /// </summary>
        /// <param name="type"></param>
        public void SetMaxType(E_PoolMaxType type)
        {
            maxType = type;
        }

    }
}