using UnityEngine;

namespace TBFramework
{
    public class MonoSingleton<T> : MonoBehaviour where T:MonoSingleton<T>
    {
        private static T instance;
        public static T Instance{
            get{
                if(instance==null){
                    instance=new GameObject(typeof(T).Name,typeof(T)).GetComponent<T>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        protected virtual void Awake() {
            if(instance!=null){
                Destroy(this);
                return;
            }
            instance=this as T;
            DontDestroyOnLoad(instance.gameObject);
        }

        protected virtual void OnDestroy() {
            if(instance==this){
                instance=null;
            }
        }
    }
}
