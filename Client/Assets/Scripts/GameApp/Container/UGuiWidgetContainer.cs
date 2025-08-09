using System.Collections.Generic;
using GameFramework;

namespace GameApp
{
    public sealed class UGuiWidgetContainer : IReference
    {
        private readonly List<UGuiWidget> m_UGuiWidgets = new List<UGuiWidget>();
        public List<UGuiWidget> UGuiWidgets => m_UGuiWidgets;

        public UGuiFormLogic Owner
        {
            get; 
            private set;
        }

        public static UGuiWidgetContainer Create(UGuiFormLogic owner)
        {
            UGuiWidgetContainer uGuiWidgetContainer = ReferencePool.Acquire<UGuiWidgetContainer>();
            uGuiWidgetContainer.Owner = owner;
            return uGuiWidgetContainer;
        }

        public void Clear()
        {
            m_UGuiWidgets.Clear();
            Owner = null;
        }

        public void AddUIWidget(UGuiWidget uGuiWidget, object userData)
        {
            if (uGuiWidget == null)
            {
                throw new GameFrameworkException("Can't add empty!");
            }

            if (m_UGuiWidgets.Contains(uGuiWidget))
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't duplicate add UGuiWidget : '{0}'!", uGuiWidget.CachedTransform.name));
            }

            m_UGuiWidgets.Add(uGuiWidget);
            uGuiWidget.OnInit(userData);
        }

        public void RemoveUIWidget(UGuiWidget uGuiWidget)
        {
            if (uGuiWidget == null)
            {
                throw new GameFrameworkException("Can't remove empty!");
            }

            if (!m_UGuiWidgets.Remove(uGuiWidget))
            {
                throw new GameFrameworkException(Utility.Text.Format("UGuiWidget : '{0}' not in container.", uGuiWidget.CachedTransform.name));
            }
        }

        public void RemoveAllUIWidget()
        {
            if (m_UGuiWidgets.Count > 0)
            {
                m_UGuiWidgets.Clear();
            }
        }

        /// <summary>
        /// 打开 UGuiWidget，不刷新 Depth，一般在 UIForm 的 OnOpen 中调用。
        /// </summary>
        /// <param name="uGuiWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void OpenUIWidget(UGuiWidget uGuiWidget, object userData)
        {
            if (uGuiWidget == null)
            {
                throw new GameFrameworkException("Can't open empty!");
            }

            if (!m_UGuiWidgets.Contains(uGuiWidget))
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't open UGuiWidget, UGuiWidget '{0}' not in the container '{1}'!", uGuiWidget.name, Owner.Name));
            }

            if (uGuiWidget.IsOpen)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't open UGuiWidget, UGuiWidget '{0}' is already opened!", uGuiWidget.name));
            }

            uGuiWidget.OnOpen(userData);
        }

        /// <summary>
        /// 动态打开 UGuiWidget，刷新 Depth。
        /// </summary>
        /// <param name="uGuiWidget"></param>
        /// <param name="userData"></param>
        public void DynamicOpenUIWidget(UGuiWidget uGuiWidget, object userData)
        {
            OpenUIWidget(uGuiWidget, userData);
            uGuiWidget.OnDepthChanged(Owner.UIForm.UIGroup.Depth, Owner.UIForm.DepthInUIGroup);
        }

        public void CloseUIWidget(UGuiWidget uGuiWidget, object userData, bool isShutdown)
        {
            if (uGuiWidget == null)
            {
                throw new GameFrameworkException("Can't open empty!");
            }

            if (!m_UGuiWidgets.Contains(uGuiWidget))
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't open UGuiWidget, UGuiWidget '{0}' not in the container '{1}'!", uGuiWidget.name, Owner.Name));
            }

            if (!uGuiWidget.IsOpen)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't close UGuiWidget, UGuiWidget '{0}' is not opened!", uGuiWidget.name));
            }

            uGuiWidget.OnClose(isShutdown, userData);
        }

        public void CloseAllUGuiWidgets(object userData, bool isShutdown)
        {
            if (m_UGuiWidgets.Count > 0)
            {
                foreach (var uGuiWidget in m_UGuiWidgets)
                {
                    if (uGuiWidget.IsOpen)
                    {
                        uGuiWidget.OnClose(isShutdown, userData);
                    }
                }
            }
        }

        /// <summary>
        /// 界面回收。
        /// </summary>
        public void OnRecycle()
        {
            foreach (var uGuiWidget in m_UGuiWidgets)
            {
                uGuiWidget.OnRecycle();
            }
        }

        /// <summary>
        /// 界面关闭。
        /// </summary>
        /// <param name="isShutdown">是否是关闭界面管理器时触发。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void OnClose(bool isShutdown, object userData)
        {
            foreach (var uGuiWidget in m_UGuiWidgets)
            {
                if (uGuiWidget.IsOpen)
                {
                    uGuiWidget.OnClose(isShutdown, userData);
                }
            }
        }

        /// <summary>
        /// 界面暂停。
        /// </summary>
        public void OnPause()
        {
            foreach (var uGuiWidget in m_UGuiWidgets)
            {
                if (uGuiWidget.IsOpen)
                {
                    uGuiWidget.OnPause();
                }
            }
        }

        /// <summary>
        /// 界面暂停恢复。
        /// </summary>
        public void OnResume()
        {
            foreach (var uGuiWidget in m_UGuiWidgets)
            {
                if (uGuiWidget.IsOpen)
                {
                    uGuiWidget.OnResume();
                }
            }
        }

        /// <summary>
        /// 界面遮挡。
        /// </summary>
        public void OnCover()
        {
            foreach (var uGuiWidget in m_UGuiWidgets)
            {
                if (uGuiWidget.IsOpen)
                {
                    uGuiWidget.OnCover();
                }
            }
        }

        /// <summary>
        /// 界面遮挡恢复。
        /// </summary>
        public void OnReveal()
        {
            foreach (var uGuiWidget in m_UGuiWidgets)
            {
                if (uGuiWidget.IsOpen)
                {
                    uGuiWidget.OnReveal();
                }
            }
        }

        /// <summary>
        /// 界面激活。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void OnRefocus(object userData)
        {
            foreach (var uGuiWidget in m_UGuiWidgets)
            {
                if (uGuiWidget.IsOpen)
                {
                    uGuiWidget.OnRefocus(userData);
                }
            }
        }

        /// <summary>
        /// 界面轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var uGuiWidget in m_UGuiWidgets)
            {
                if (uGuiWidget.IsOpen)
                {
                    uGuiWidget.OnUpdate(elapseSeconds, realElapseSeconds);
                }
            }
        }

        /// <summary>
        /// 界面深度改变。
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度。</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度。</param>
        public void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            foreach (var uGuiWidget in m_UGuiWidgets)
            {
                if (uGuiWidget.IsOpen)
                {
                    uGuiWidget.OnDepthChanged(uiGroupDepth, depthInUIGroup);
                }
            }
        }
    }
}