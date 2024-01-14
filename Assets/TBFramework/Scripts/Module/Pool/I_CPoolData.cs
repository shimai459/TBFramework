using UnityEngine;

namespace TBFramework.Pool
{
    public abstract class I_CPoolData{
        protected int maxNumber=PoolSet.POOL_MAX_NUMBER;
        public GameObject fatherObj;

        public void SetMaxNUmber(int max){
            maxNumber=max;
        }

    }
}