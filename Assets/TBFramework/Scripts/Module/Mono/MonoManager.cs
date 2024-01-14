using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

namespace TBFramework.Mono
{
    public class MonoManager : Singleton<MonoManager>
    {
        private MonoController controller;

        public MonoManager(){
            controller=new GameObject("MonoController",typeof(MonoController)).GetComponent<MonoController>();
        }

        public void AddUpdateListener(Action action){
            controller.updateEvent+=action;
        }

        public void RemoveUpdateListener(Action action){
            controller.updateEvent-=action;
        }

        public Coroutine StartCoroutine(string methodName){
            return controller.StartCoroutine(methodName);
        }
        public Coroutine StartCoroutine(IEnumerator routine){
            return controller.StartCoroutine(routine);
        }

        public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value){
            return controller.StartCoroutine(methodName,value);
        }

        public void StopAllCoroutines(){
            controller.StopAllCoroutines();
        }

        public void StopCoroutine(IEnumerator routine){
            controller.StopCoroutine(routine);
        } 
        public void StopCoroutine(Coroutine routine){
            controller.StopCoroutine(routine);
        }
        public void StopCoroutine(string methodName){
            controller.StopCoroutine(methodName);
        }
    }
}
