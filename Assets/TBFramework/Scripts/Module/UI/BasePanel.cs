using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TBFramework.UI
{
    public class BasePanel : MonoBehaviour
    {
        //保存面板上每一个UI对象上所有的控件对象
        private Dictionary<string,List<UIBehaviour>> controlDict=new Dictionary<string, List<UIBehaviour>>();

        //一开始就完成控件的获取
        protected virtual void Awake() {
            FindChildrenControl<Button>();
            FindChildrenControl<Text>();
            FindChildrenControl<Image>();
            FindChildrenControl<RawImage>();
            FindChildrenControl<Toggle>();
            FindChildrenControl<Slider>();
            FindChildrenControl<ScrollRect>();
            FindChildrenControl<InputField>();
            FindChildrenControl<Scrollbar>();
            FindChildrenControl<Dropdown>();
            FindChildrenControl<Selectable>();
            FindChildrenControl<ToggleGroup>();
            FindChildrenControl<RectMask2D>();
            FindChildrenControl<Mask>();
            FindChildrenControl<Outline>();
            FindChildrenControl<PositionAsUV1>();
            FindChildrenControl<Shadow>();
        }
        /// <summary>
        /// 提供外部,显示面板的函数
        /// </summary>
        public virtual void Show(){
            this.gameObject.SetActive(true);
        }
        /// <summary>
        /// 提供外部,隐藏面板的函数
        /// </summary>
        public virtual void Hide(){
            this.gameObject.SetActive(false);
        }
        /// <summary>
        /// 鼠标点击事件的申明
        /// </summary>
        /// <param name="btnName">按钮名</param>
        protected virtual void OnClick(string btnName){}
        /// <summary>
        /// 单选框或多选框值改变事件的申明
        /// </summary>
        /// <param name="toggleName">单/多选框名</param>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(string toggleName,bool value){}
        /// <summary>
        /// 滚动条和滑动条监听事件的申明
        /// </summary>
        /// <param name="controlName">滚动条和滑动条控件名</param>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(string controlName,float value){}
        /// <summary>
        /// 输入框监听事件申明
        /// </summary>
        /// <param name="inputFieldName">输入框名</param>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(string inputFieldName,string value){}
        /// <summary>
        /// 滚动矩形监听事件申明
        /// </summary>
        /// <param name="inputFieldName">滚动矩形名/param>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(string scrollRectName,Vector2 value){}
        /// <summary>
        /// 选择框监听事件申明
        /// </summary>
        /// <param name="dropdownName">选择框名</param>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(string dropdownName,int value){}
        protected virtual void OnSubmit(string inputFieldName,string value){}
        protected virtual void OnEndEdit(string inputFieldName,string value){}

        /// <summary>
        /// 根据UI对象名和控件类型获取指定控件
        /// </summary>
        /// <param name="controlName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetControl<T>(string controlName)where T:UIBehaviour
        {
            if(controlDict.ContainsKey(controlName)){
                foreach(UIBehaviour item in controlDict[controlName]){
                    if(item is T){
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
        private void FindChildrenControl<T>()where T:UIBehaviour
        {
            T[] controls=this.GetComponentsInChildren<T>();
            foreach(T item in controls){
                if(controlDict.ContainsKey(item.name)){
                    controlDict[item.name].Add(item);
                }else{
                    controlDict.Add(item.name,new List<UIBehaviour>{item});
                }
                //因为Button,Toggle,Slider,Scrollbar,Dropdown,InputField都继承于Selectable,为防止多次添加事件,要将T为Selectable的时候,添加事件去除一下
                //如果是按钮,就就添加按钮监听事件
                if(item is Button&&typeof(T).Name!="Selectable"){
                    (item as Button).onClick.AddListener(()=>{
                        OnClick(item.name);
                    });
                }
                //如果是单选或多选框,就就添加值改变监听事件
                else if(item is Toggle&&typeof(T).Name!="Selectable"){
                    (item as Toggle).onValueChanged.AddListener((value)=>{
                        OnValueChanged(item.name,value);
                    });
                }
                //如果是滑动条,就就添加滑动条监听事件
                else if(item is Slider&&typeof(T).Name!="Selectable"){
                    (item as Slider).onValueChanged.AddListener((value)=>{
                        OnValueChanged(item.name,value);
                    });
                }
                //如果是滚动条,就就添加滚动条监听事件
                else if(item is Scrollbar&&typeof(T).Name!="Selectable"){
                    (item as Scrollbar).onValueChanged.AddListener((value)=>{
                        OnValueChanged(item.name,value);
                    });
                }
                //如果是滚动矩形,就就添加滚动矩形监听事件
                else if(item is ScrollRect){
                    (item as ScrollRect).onValueChanged.AddListener((value)=>{
                        OnValueChanged(item.name,value);
                    });
                }
                //如果是选择框,就就添加选择框监听事件
                else if(item is Dropdown&&typeof(T).Name!="Selectable"){
                    (item as Dropdown).onValueChanged.AddListener((value)=>{
                        OnValueChanged(item.name,value);
                    });
                }
                //如果是输入框,添加输入框监听事件
                else if(item is InputField&&typeof(T).Name!="Selectable"){
                    (item as InputField).onValueChanged.AddListener((value)=>{
                        OnValueChanged(item.name,value);
                    });
                    (item as InputField).onSubmit.AddListener((value)=>{
                        OnSubmit(item.name,value);
                    });
                    (item as InputField).onEndEdit.AddListener((value)=>{
                        OnEndEdit(item.name,value);
                    });
                }
            }
        }
        
    }
}