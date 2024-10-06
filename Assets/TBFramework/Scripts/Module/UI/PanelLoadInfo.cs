using System;
using TBFramework.Pool;

namespace TBFramework.UI
{
    public class PanelLoadInfo : CBase
    {
        public BasePanel panel;
        public Action<BasePanel> action;
        public bool isHide;
        public bool isDestroy;

        public bool isPause;

        public PanelLoadInfo()
        {
        }

        public PanelLoadInfo(BasePanel panel, Action<BasePanel> action, bool isHide, bool isDestroy, bool isPause)
        {
            this.SetPanel(panel, action, isHide, isDestroy, isPause);
        }

        public void SetPanel(BasePanel panel, Action<BasePanel> action, bool isHide, bool isDestroy, bool isPause)
        {
            this.panel = panel;
            this.action = action;
            this.isHide = isHide;
            this.isDestroy = isDestroy;
            this.isPause = isPause;
        }

        public override void Reset()
        {
            this.panel = null;
            this.action = null;
            this.isHide = false;
            this.isDestroy = false;
            this.isPause = false;
        }
    }
}
