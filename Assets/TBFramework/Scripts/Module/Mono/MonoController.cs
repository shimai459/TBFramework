using System;
using UnityEngine;

namespace TBFramework.Mono
{
    public class MonoController : MonoBehaviour
    {
        public event Action updateEvent;

        public event Action fixedUpdateEvent;

        public event Action lateUpdateEvent;

        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        void Update()
        {
            if (updateEvent != null)
            {
                updateEvent.Invoke();
            }

        }

        private void FixedUpdate()
        {
            fixedUpdateEvent?.Invoke();
        }

        private void LateUpdate()
        {
            lateUpdateEvent?.Invoke();
        }
    }
}
