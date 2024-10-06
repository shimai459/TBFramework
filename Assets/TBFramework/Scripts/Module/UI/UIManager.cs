using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using TBFramework.LoadInfo;
using TBFramework.Pool;
using TBFramework.Mono;

namespace TBFramework.UI
{
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// 隐藏多久会销毁（ms）
        /// </summary>
        public int destoryTime = 30000;
        //存储面板的容器
        private Dictionary<string, PanelLoadInfo> panelDict = new Dictionary<string, PanelLoadInfo>();

        private Dictionary<E_UILayer, Transform> layers = new Dictionary<E_UILayer, Transform>();

        //UI的摄像机
        public Camera uiCamera;

        //记录UI的Canvas父对象,方便外部去调用
        public RectTransform canvas;
        //记录UI的EventSystem事件触发器,方便外部调用
        public GameObject eventSystem;

        public UIManager()
        {
            this.DefalutCreate();
            MonoConManager.Instance.AddUpdateListener(CheckDestoryPanel);
        }

        public void DefalutCreate()
        {
            CreateCamera((action) =>
            {
                GameObject cameraObj = new GameObject("UIcamera", typeof(Camera));
                Camera camera = cameraObj.GetComponent<Camera>();
                camera.clearFlags = CameraClearFlags.Depth;
                camera.cullingMask = (0 | (1 << 5));
                action?.Invoke(camera);
            });
            CreateCanvas((action) =>
            {
                GameObject canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                Canvas canvas = canvasObj.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;

                CanvasScaler canvasScaler = canvasObj.GetComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 1f;
                action?.Invoke(canvas, null);
            });
            CreateEventSystem((action) =>
            {
                GameObject eventSystemObj = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                action?.Invoke(eventSystemObj.GetComponent<EventSystem>());
            });
        }

        public void CreateCamera(Action<Action<Camera>> action)
        {
            action?.Invoke((camera) =>
            {
                if (camera != null)
                {
                    if (this.uiCamera != null)
                    {
                        GameObject.Destroy(this.uiCamera.gameObject);
                    }
                    this.uiCamera = camera;
                    if (this.canvas != null)
                    {
                        Canvas canvas = this.canvas.GetComponent<Canvas>();
                        if (canvas != null)
                        {
                            canvas.worldCamera = this.uiCamera;
                        }
                    }
                    GameObject.DontDestroyOnLoad(camera.gameObject);
                }
            });
        }

        public void CreateCanvas(Action<Action<Canvas, Dictionary<E_UILayer, Transform>>> action)
        {
            action?.Invoke((canvas, dic) =>
            {
                if (canvas != null)
                {
                    if (this.canvas != null)
                    {
                        GameObject.Destroy(this.canvas.gameObject);
                    }
                    this.canvas = canvas.transform as RectTransform;
                    GameObject.DontDestroyOnLoad(canvas.gameObject);
                    if (this.uiCamera != null)
                    {
                        canvas.worldCamera = this.uiCamera;
                    }
                    if (dic != null)
                    {
                        this.layers = dic;
                    }
                    else
                    {
                        CreateLayers(canvas.gameObject);
                    }
                }
            });
        }

        private void CreateLayers(GameObject canvas)
        {
            this.layers.Clear();
            Array layers = Enum.GetValues(typeof(E_UILayer));
            for (int i = 0; i < layers.Length; i++)
            {
                E_UILayer layer = (E_UILayer)layers.GetValue(i);
                GameObject obj = new GameObject(layer.ToString());
                obj.transform.SetParent(canvas.transform);
                obj.transform.SetSiblingIndex(i);
                this.layers.Add(layer, obj.transform);
            }
        }

        public void CreateEventSystem(Action<Action<EventSystem>> action)
        {
            action?.Invoke((eventSystem) =>
            {
                if (eventSystem != null)
                {
                    if (this.eventSystem != null)
                    {
                        GameObject.Destroy(this.eventSystem.gameObject);
                    }
                    this.eventSystem = eventSystem.gameObject;
                    GameObject.DontDestroyOnLoad(eventSystem.gameObject);
                }

            });
        }

        /// <summary>
        /// 获取层级枚举的父节点
        /// </summary>
        /// <param name="layer">层级枚举</param>
        /// <returns></returns>
        public Transform GetLayerFather(E_UILayer layer)
        {
            if (layers.ContainsKey(layer))
            {
                return layers[layer];
            }
            return layers.Values.FirstOrDefault();
        }

        /// <summary>
        /// 提供外部调用,显示面板函数
        /// </summary>
        /// <param name="panelName">面板名</param>
        /// <param name="layer">面板要添加到的层级</param>
        /// <param name="callBack">但创建或显示面板后,你要做的行为</param>
        /// <typeparam name="T">面板类型</typeparam>
        public void Show<T>(string panelName, E_UILayer layer = E_UILayer.Middle, Action<BasePanel> callBack = null, Action<string, Action<GameObject>> load = null) where T : BasePanel
        {

            //先判断面板是否已经存在,存在则直接调用面板显示
            if (panelDict.ContainsKey(panelName))
            {
                PanelLoadInfo panelInfo = panelDict[panelName];
                if (panelInfo != null)
                {
                    panelInfo.isHide = false;
                    panelInfo.isDestroy = false;
                    panelInfo.isPause = false;
                    if (panelInfo.panel == null)
                    {
                        panelInfo.action += callBack;
                    }
                    else
                    {
                        panelInfo.panel.OnShow();
                        callBack?.Invoke(panelInfo.panel);
                    }
                    return;
                }
                else
                {
                    panelDict.Remove(panelName);
                }
            }
            PanelLoadInfo panelLoadInfo = CPoolManager.Instance.Pop<PanelLoadInfo>();
            panelLoadInfo.SetPanel(null, callBack, false, false, false);
            panelDict.Add(panelName, panelLoadInfo);

            if (load != null)
            {
                load?.Invoke(panelName, DoAfterCreate);
            }
            else
            {
                LoadInfoManager.Instance.DoLoad<GameObject>(LoadInfoManager.Instance.GetName(panelName, typeof(GameObject)), DoAfterCreate, true);
            }
            void DoAfterCreate(GameObject o)
            {
                //创建完成界面，先取出当前界面需要的状态
                bool isHide = false;
                bool isPause = false;
                if (panelDict.ContainsKey(panelName))
                {
                    PanelLoadInfo panelLoadInfo = panelDict[panelName];
                    if (panelLoadInfo != null)
                    {
                        isHide = panelLoadInfo.isHide;
                        isPause = panelLoadInfo.isPause;
                        if (panelLoadInfo.isHide == true)
                        {
                            if (panelLoadInfo.isDestroy == true)
                            {
                                if (!(PrefabUtility.GetPrefabInstanceStatus(o) != PrefabInstanceStatus.NotAPrefab || PrefabUtility.GetPrefabAssetType(o) != PrefabAssetType.NotAPrefab))
                                {
                                    GameObject.Destroy(o);
                                }
                                CPoolManager.Instance.Push<PanelLoadInfo>(panelLoadInfo);
                                panelDict.Remove(panelName);
                                return;
                            }
                        }
                    }
                }

                GameObject obj = o;
                //判断是否实例化
                if (PrefabUtility.GetPrefabInstanceStatus(o) != PrefabInstanceStatus.NotAPrefab || PrefabUtility.GetPrefabAssetType(o) != PrefabAssetType.NotAPrefab)
                {
                    obj = GameObject.Instantiate(o);
                }

                //获取面板层级信息,设置层级父物体
                Transform father = GetLayerFather(layer);
                obj.transform.SetParent(father);

                //初始化面板的大小和相对位置
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;

                (obj.transform as RectTransform).offsetMax = Vector2.zero;
                (obj.transform as RectTransform).offsetMin = Vector2.zero;

                //执行面板创建后的行为
                T panel = obj.GetComponent<T>();
                if (panel != null)
                {
                    panel = obj.AddComponent<T>();
                }
                if (isHide)
                {
                    panel.OnShow();
                    if (isPause)
                    {
                        panel.OnPause();
                    }
                    else
                    {
                        panel.OnResume();
                    }
                }
                else
                {
                    panel.OnHide();
                }
                callBack?.Invoke(panel);
                //将面板添加到存储面板的容器中
                if (panelDict.ContainsKey(panelName))
                {
                    PanelLoadInfo panelLoadInfo = panelDict[panelName];
                    panelLoadInfo.panel = panel;
                    panelLoadInfo.action = null;
                }
                else
                {
                    PanelLoadInfo panelLoadInfo = CPoolManager.Instance.Pop<PanelLoadInfo>();
                    panelLoadInfo.SetPanel(panel, null, false, false, false);
                    panelDict.Add(panelName, panelLoadInfo);
                }
            }
        }

        /// <summary>
        /// 提供外部,隐藏面板的函数
        /// </summary>
        /// <param name="panelName">面板名</param>
        /// <param name="isDestroy">隐藏后是否销毁面板,默认销毁</param>
        public void Hide(string panelName, bool isDestroy)
        {
            if (panelDict.ContainsKey(panelName))
            {
                PanelLoadInfo panelLoadInfo = panelDict[panelName];
                if (panelLoadInfo != null)
                {
                    if (panelLoadInfo.panel != null)
                    {
                        panelLoadInfo.panel.OnHide();
                        if (isDestroy)
                        {
                            GameObject.Destroy(panelLoadInfo.panel.gameObject);
                            CPoolManager.Instance.Push<PanelLoadInfo>(panelLoadInfo);
                            panelDict.Remove(panelName);
                        }
                    }
                    else
                    {
                        panelLoadInfo.isHide = true;
                        panelLoadInfo.isDestroy = isDestroy;
                    }
                }

            }
        }

        public void Pause(string panelName)
        {
            if (panelDict.ContainsKey(panelName))
            {
                PanelLoadInfo panelLoadInfo = panelDict[panelName];
                if (panelLoadInfo != null)
                {
                    if (panelLoadInfo.panel != null)
                    {
                        panelLoadInfo.panel.OnPause();
                    }
                    else
                    {
                        panelLoadInfo.isPause = true;
                    }
                }

            }
        }

        public void Resume(string panelName)
        {
            if (panelDict.ContainsKey(panelName))
            {
                PanelLoadInfo panelLoadInfo = panelDict[panelName];
                if (panelLoadInfo != null)
                {
                    if (panelLoadInfo.panel != null)
                    {
                        panelLoadInfo.panel.OnResume();
                    }
                    else
                    {
                        panelLoadInfo.isPause = false;
                    }
                }

            }
        }

        /// <summary>
        /// 提供外部获取面板的函数
        /// </summary>
        /// <param name="panelName">面板名</param>
        /// <returns>面板对象</returns>
        public void PanelDo(string panelName, Action<BasePanel> action)
        {
            if (panelDict.ContainsKey(panelName))
            {
                if (panelDict[panelName] != null)
                {
                    if (panelDict[panelName].panel != null)
                    {
                        action?.Invoke(panelDict[panelName].panel);
                    }
                    else
                    {
                        panelDict[panelName].action += action;
                    }
                }
            }
        }

        private void CheckDestoryPanel()
        {
            List<string> panels = new List<string>();
            foreach (string panelName in panelDict.Keys)
            {
                BasePanel basePanel = panelDict[panelName].panel;
                if (basePanel != null)
                {
                    if (basePanel.isDestroyInHideSomeTimes && basePanel.isHide && !basePanel.isInQueue && DateTime.Now.Ticks - basePanel.activeTime > this.destoryTime * TimeSpan.TicksPerMillisecond)
                    {
                        panels.Add(panelName);
                    }
                }
            }
            foreach (string panelName in panels)
            {
                Hide(panelName, true);
            }
        }

        /// <summary>
        /// 给控件添加自定义事件监听
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <param name="type">要添加的事件类型</param>
        /// <param name="callBack">事件触发后执行的行为(事件响应函数)</param>
        public void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
        {
            EventTrigger eventTrigger = control.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = control.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(callBack);

            eventTrigger.triggers.Add(entry);
        }
    }
}