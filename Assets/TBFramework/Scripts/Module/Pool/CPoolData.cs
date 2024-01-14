using System.Collections.Generic;
using UnityEngine;

namespace TBFramework.Pool
{
    public class CPoolData<T>:I_CPoolData where T:Behaviour
    {
        private Stack<T> poolStack;
        public int ObjCountInPool{get {return poolStack.Count;}}
        public bool IsHaving{get{return poolStack.Count!=0;}}

        public CPoolData(GameObject cPoolObj)
        {
            fatherObj=new GameObject(typeof(T).Name+" "+PoolSet.POOL_SINGLE_PARENT_EXTENSION);
            fatherObj.transform.SetParent(cPoolObj.transform);
            GameObject.DontDestroyOnLoad(fatherObj);
            poolStack=new Stack<T>();
        }

        public T Pop(){
            T monoC=poolStack.Pop();
            monoC.enabled=true;
            return monoC;
        }

        public void Push(T monoC){
            if(poolStack.Count>maxNumber){
                GameObject.Destroy(monoC);
                for(int i=0;i<poolStack.Count-maxNumber;i++){
                    GameObject.Destroy(Pop());
                }
            }else if(poolStack.Count==maxNumber){
                GameObject.Destroy(monoC);
            }else{
                poolStack.Push(monoC);
                monoC.enabled=false;
            }
        }
    }
}
