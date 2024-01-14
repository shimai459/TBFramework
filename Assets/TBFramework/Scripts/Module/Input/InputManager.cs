using UnityEngine;
using TBFramework.Mono;
using TBFramework.Event;

namespace TBFramework.Input
{

    public class InputManager : Singleton<InputManager>
    {
        private bool isStart;
        public InputManager(){
            MonoManager.Instance.AddUpdateListener(MyUpdate);
        }

        public void StartOrEndCheck(bool isOpen){
            isStart=isOpen;
        }

        private void CheckKeyCode(KeyCode key){
            if(UnityEngine.Input.GetKeyDown(key)){
                EventCenter.Instance.EventTrigger<KeyCode>("KeyDown",key);
            }
            if(UnityEngine.Input.GetKeyUp(key)){
                EventCenter.Instance.EventTrigger<KeyCode>("KeyUp",key);
            }
        }

        private void MyUpdate(){
            if(isStart){
                CheckKeyCode(KeyCode.W);
                CheckKeyCode(KeyCode.A);
                CheckKeyCode(KeyCode.S);
                CheckKeyCode(KeyCode.D);
            }
            
        }
    }
}
