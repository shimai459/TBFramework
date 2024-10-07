using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace TBFramework.UI
{
    public class BasePanel : MonoBehaviour
    {
        protected virtual bool UseTreeName { get => false; }

        public virtual bool IsDestroyInHideSomeTimes { get => true; }

        public bool isHide = false;

        public bool isInQueue = false;

        public long activeTime;

        //保存面板上每一个UI对象上所有的控件对象
        private Dictionary<string, List<UIBehaviour>> controlDict = new Dictionary<string, List<UIBehaviour>>();

        protected CanvasGroup canvasGroup;

        //一开始就完成控件的获取
        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (!UseTreeName)
            {
                FindChildrenControl<UIBehaviour>();
            }
            else
            {
                FindChildrenControlWithTreeName<UIBehaviour>(this.transform, this.name);
            }

        }


        /// <summary>
        /// 提供外部,显示面板的函数
        /// </summary>
        public virtual void OnShow()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            isHide = false;
        }
        /// <summary>
        /// 提供外部,隐藏面板的函数
        /// </summary>
        public virtual void OnHide()
        {
            activeTime = DateTime.Now.Ticks;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isHide = true;
        }

        public virtual void OnPause()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public virtual void OnResume()
        {
            if (canvasGroup.alpha != 0f)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        /// <summary>
        /// 鼠标点击事件的申明
        /// </summary>
        /// <param name="btnName">按钮名</param>
        protected virtual void OnClick(string btnName) { }
        /// <summary>
        /// 单选框或多选框值改变事件的申明
        /// </summary>
        /// <param name="toggleName">单/多选框名</param>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(string toggleName, bool value) { }
        /// <summary>
        /// 滚动条和滑动条监听事件的申明
        /// </summary>
        /// <param name="controlName">滚动条和滑动条控件名</param>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(string controlName, float value) { }
        /// <summary>
        /// 输入框监听事件申明
        /// </summary>
        /// <param name="inputFieldName">输入框名</param>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(string inputFieldName, string value) { }
        /// <summary>
        /// 滚动矩形监听事件申明
        /// </summary>
        /// <param name="inputFieldName">滚动矩形名/param>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(string scrollRectName, Vector2 value) { }
        /// <summary>
        /// 选择框监听事件申明
        /// </summary>
        /// <param name="dropdownName">选择框名</param>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(string dropdownName, int value) { }
        protected virtual void OnSubmit(string inputFieldName, string value) { }
        protected virtual void OnEndEdit(string inputFieldName, string value) { }

        /// <summary>
        /// 根据UI对象名和控件类型获取指定控件
        /// </summary>
        /// <param name="controlName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetControl<T>(string controlName) where T : UIBehaviour
        {
            if (controlDict.ContainsKey(controlName))
            {
                foreach (UIBehaviour item in controlDict[controlName])
                {
                    if (item is T)
                    {
                        return item as T;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 找到面板下所有UI的指定类型控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        private void FindChildrenControl<T>() where T : UIBehaviour
        {
            T[] controls = this.GetComponentsInChildren<T>(true);
            foreach (UIBehaviour item in controls)
            {
                AddControlAndListener(item, item.name);
            }

        }

        private void FindChildrenControlWithTreeName<T>(Transform transform, string treeName) where T : UIBehaviour
        {
            T[] controls = transform.GetComponents<T>();
            foreach (UIBehaviour item in controls)
            {
                AddControlAndListener(item, treeName);
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                string name = treeName + "/" + child.name;
                FindChildrenControlWithTreeName<T>(child, name);
            }
        }

        private void AddControlAndListener(UIBehaviour item, string name)
        {
            if (controlDict.ContainsKey(name))
            {
                controlDict[name].Add(item);
            }
            else
            {
                controlDict.Add(name, new List<UIBehaviour> { item });
            }

            //如果是按钮,就就添加按钮监听事件
            if (item is Button)
            {
                (item as Button).onClick.AddListener(() =>
                {
                    OnClick(name);
                });
            }
            //如果是单选或多选框,就就添加值改变监听事件
            else if (item is Toggle)
            {
                (item as Toggle).onValueChanged.AddListener((value) =>
                {
                    OnValueChanged(name, value);
                });
            }
            //如果是滑动条,就就添加滑动条监听事件
            else if (item is Slider)
            {
                (item as Slider).onValueChanged.AddListener((value) =>
                {
                    OnValueChanged(name, value);
                });
            }
            //如果是滚动条,就就添加滚动条监听事件
            else if (item is Scrollbar)
            {
                (item as Scrollbar).onValueChanged.AddListener((value) =>
                {
                    OnValueChanged(name, value);
                });
            }
            //如果是滚动矩形,就就添加滚动矩形监听事件
            else if (item is ScrollRect)
            {
                (item as ScrollRect).onValueChanged.AddListener((value) =>
                {
                    OnValueChanged(name, value);
                });
            }
            //如果是选择框,就就添加选择框监听事件
            else if (item is Dropdown)
            {
                (item as Dropdown).onValueChanged.AddListener((value) =>
                {
                    OnValueChanged(name, value);
                });
            }
            //如果是输入框,添加输入框监听事件
            else if (item is InputField)
            {
                (item as InputField).onValueChanged.AddListener((value) =>
                {
                    OnValueChanged(name, value);
                });
                (item as InputField).onSubmit.AddListener((value) =>
                {
                    OnSubmit(name, value);
                });
                (item as InputField).onEndEdit.AddListener((value) =>
                {
                    OnEndEdit(name, value);
                });
            }
        }
    }
}