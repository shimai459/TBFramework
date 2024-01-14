using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using System.IO;
using TBFramework.Resource;

namespace TBFramework.UI
{
    public class UIManager : Singleton<UIManager>
    {
        //存储面板的容器
        private Dictionary<string,BasePanel> panelDict=new Dictionary<string, BasePanel>();
        //面板层级_底层
        private Transform bottom;
        //面板层级_中层
        private Transform middle;
        //面板层级_顶层
        private Transform top;
        //面板层级_系统层
        private Transform system;

        //记录UI的Canvas父对象,方便外部去调用
        public RectTransform canvas;
        //记录UI的EventSystem事件触发器,方便外部调用
        public GameObject eventSystem;

        public UIManager(){
            //使用UI就创建一个Canvas,并使其在过场景时不销毁
            GameObject obj=ResourceManager.Instance.Load<GameObject>(Path.Combine(UIResourceSet.UI_PATH,"Canvas"));
            canvas=obj.transform as RectTransform;
            GameObject.DontDestroyOnLoad(obj);

            //获取UI层级父节点
            bottom=canvas.Find("Bottom");
            middle=canvas.Find("Middle");
            top=canvas.Find("Top");
            system=canvas.Find("System");

            //创建一个EventSystem用于UI事件检测,并过场景不销毁
            obj=ResourceManager.Instance.Load<GameObject>(Path.Combine(UIResourceSet.UI_PATH,"EventSystem"));
            eventSystem=obj;
            GameObject.DontDestroyOnLoad(obj);
        }
        /// <summary>
        /// 获取层级枚举的父节点
        /// </summary>
        /// <param name="layer">层级枚举</param>
        /// <returns></returns>
        public Transform GetLayerFather(E_UILayer layer){
            switch(layer){
                case E_UILayer.Bottom:
                    return bottom;
                case E_UILayer.Middle:
                    return middle;
                case E_UILayer.Top:
                    return top;
                case E_UILayer.System:
                    return system;
            }
            return null;
        }
        /// <summary>
        /// 提供外部调用,显示面板函数
        /// </summary>
        /// <param name="panelName">面板名</param>
        /// <param name="layer">面板要添加到的层级</param>
        /// <param name="callBack">但创建或显示面板后,你要做的行为</param>
        /// <typeparam name="T">面板类型</typeparam>
        public void Show<T>(string panelName,E_UILayer layer=E_UILayer.Middle,Action<T> callBack=null)where T:BasePanel
        {
            //先判断面板是否已经存在,存在则直接调用面板显示
            if(panelDict.ContainsKey(panelName)){
                panelDict[panelName].Show();
                if(callBack!=null){
                    callBack.Invoke(panelDict[panelName] as T);
                }
                return;
            }
            //否则动态创建
            ResourceManager.Instance.LoadAsync<GameObject>(Path.Combine(UIResourceSet.PANEL_PATH,panelName),(o)=>{
                GameObject obj=o as GameObject;
                //获取面板层级信息,设置层级父物体
                Transform father=bottom;
                switch(layer){
                    case E_UILayer.Middle:
                        father=middle;
                        break;
                    case E_UILayer.Top:
                        father=top;
                        break;
                    case E_UILayer.System:
                        father=system;
                        break;
                }
                obj.transform.SetParent(father);
                
                //初始化面板的大小和相对位置
                obj.transform.localPosition=Vector3.zero;
                obj.transform.localScale=Vector3.one;

                (obj.transform as RectTransform).offsetMax=Vector2.zero;
                (obj.transform as RectTransform).offsetMin=Vector2.zero;

                //执行面板创建后的行为
                T panel=obj.GetComponent<T>();
                panel.Show();  
                if(callBack!=null){
                    callBack(panel);
                }
                //将面板添加到存储面板的容器中
                panelDict.Add(panelName,panel);
            });
        }
        /// <summary>
        /// 提供外部,隐藏面板的函数
        /// </summary>
        /// <param name="panelName">面板名</param>
        /// <param name="isDestroy">隐藏后是否销毁面板,默认销毁</param>
        public void Hide(string panelName,bool isDestroy=true){
            if(panelDict.ContainsKey(panelName)){
                panelDict[panelName].Hide();
                if(isDestroy){
                    GameObject.Destroy(panelDict[panelName].gameObject);
                    panelDict.Remove(panelName);
                }
            }
        }

        /// <summary>
        /// 提供外部获取面板的函数
        /// </summary>
        /// <param name="panelName">面板名</param>
        /// <typeparam name="T">面板类型</typeparam>
        /// <returns>面板对象</returns>
        public T GetPanel<T>(string panelName)where T:BasePanel
        {
            if(panelDict.ContainsKey(panelName)){
                return panelDict[panelName] as T;
            }
            return null;
        }

        /// <summary>
        /// 给控件添加自定义事件监听
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <param name="type">要添加的事件类型</param>
        /// <param name="callBack">事件触发后执行的行为(事件响应函数)</param>
        public void AddCustomEventListener(UIBehaviour control,EventTriggerType type,UnityAction<BaseEventData> callBack){
            EventTrigger eventTrigger=control.GetComponent<EventTrigger>();
            if(eventTrigger==null){
                eventTrigger = control.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry=new EventTrigger.Entry();
            entry.eventID=type;
            entry.callback.AddListener(callBack);

            eventTrigger.triggers.Add(entry);
        }
    }
}