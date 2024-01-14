using System;
using UnityEngine;

namespace TBFramework.Mono
{
    public class MonoController : MonoBehaviour
    {
        public event Action updateEvent;
        
        void Start() {
            DontDestroyOnLoad(this.gameObject);
        }

        void Update() {
            if(updateEvent!=null){
                updateEvent.Invoke();
            }

        }
    }
}
