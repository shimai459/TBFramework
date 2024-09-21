using System;

namespace TBFramework.Mono
{
    public class MonoConManager : MonoSingleton<MonoConManager>
    {
        private event Action updateEvent;

        private event Action fixedUpdateEvent;

        private event Action lateUpdateEvent;

        private void Update()
        {
            updateEvent?.Invoke();
        }

        private void FixedUpdate(){
            fixedUpdateEvent?.Invoke();
        }

        private void LateUpdate()
        {
            lateUpdateEvent?.Invoke();
        }

        public void AddUpdateListener(Action action)
        {
            updateEvent += action;
        }

        public void RemoveUpdateListener(Action action)
        {
            updateEvent -= action;
        }

        public void AddFixedUpdateListener(Action action)
        {
            fixedUpdateEvent += action;
        }

        public void RemoveFixedUpdateListener(Action action)
        {
            fixedUpdateEvent -= action;
        }

        public void AddLateUpdateListener(Action action)
        {
            lateUpdateEvent += action;
        }

        public void RemoveLateUpdateListener(Action action)
        {
            lateUpdateEvent -= action;
        }

    }
}
