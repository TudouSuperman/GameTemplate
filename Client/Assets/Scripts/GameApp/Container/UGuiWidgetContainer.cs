using System.Collections.Generic;
using GameFramework;

namespace GameApp
{
    public sealed class UGuiWidgetContainer : IReference
    {
        private readonly List<UGuiWidgetLogic> m_UGuiWidgets = new List<UGuiWidgetLogic>();
        public List<UGuiWidgetLogic> UGuiWidgets => m_UGuiWidgets;

        public UGuiFormLogic Owner
        {
            get; 
            private set;
        }

        public static UGuiWidgetContainer Create(UGuiFormLogic owner)
        {
            UGuiWidgetContainer _uGuiWidgetContainer = ReferencePool.Acquire<UGuiWidgetContainer>();
            _uGuiWidgetContainer.Owner = owner;
            return _uGuiWidgetContainer;
        }

        public void Clear()
        {
            m_UGuiWidgets.Clear();
            Owner = null;
        }

        public void AddUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic, object userData = null)
        {
            if (uGuiWidgetLogic == null)
            {
                throw new GameFrameworkException("Can't add empty!");
            }

            if (m_UGuiWidgets.Contains(uGuiWidgetLogic))
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't duplicate add UGuiWidget : '{0}'!", uGuiWidgetLogic.CachedRectTransform.name));
            }

            m_UGuiWidgets.Add(uGuiWidgetLogic);
            uGuiWidgetLogic.SetInitialized(true);
            uGuiWidgetLogic.SetOwner(Owner);
            uGuiWidgetLogic.OnInit(userData);
        }

        public bool HasUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic)
        {
            return m_UGuiWidgets.Contains(uGuiWidgetLogic);
        }

        public void GetAllUGuiWidgets(List<UGuiWidgetLogic> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (UGuiWidgetLogic _uGuiWidgetLogic in m_UGuiWidgets)
            {
                results.Add(_uGuiWidgetLogic);
            }
        }

        public void RemoveUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic)
        {
            if (uGuiWidgetLogic == null)
            {
                throw new GameFrameworkException("Can't remove empty!");
            }

            if (uGuiWidgetLogic.Available)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't remove available UGuiWidget : '{0}'.", uGuiWidgetLogic.CachedRectTransform.name));
            }

            if (m_UGuiWidgets.Remove(uGuiWidgetLogic))
            {
                uGuiWidgetLogic.SetInitialized(false);
                uGuiWidgetLogic.SetOwner(null);
            }
            else
            {
                throw new GameFrameworkException(Utility.Text.Format("UGuiWidget : '{0}' not in container.", uGuiWidgetLogic.CachedRectTransform.name));
            }
        }

        public void RemoveAllUGuiWidget()
        {
            if (m_UGuiWidgets.Count > 0)
            {
                foreach (UGuiWidgetLogic _uGuiWidgetLogic in m_UGuiWidgets)
                {
                    _uGuiWidgetLogic.SetInitialized(false);
                    _uGuiWidgetLogic.SetOwner(null);
                }

                m_UGuiWidgets.Clear();
            }
        }

        /// <summary>
        /// 打开 UGuiWidget，不刷新 Depth，一般在 UIForm 的 OnOpen 中调用。
        /// </summary>
        /// <param name="uGuiWidgetLogic"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void OpenUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic, object userData = null)
        {
            if (uGuiWidgetLogic == null)
            {
                throw new GameFrameworkException("Can't open empty!");
            }

            if (!m_UGuiWidgets.Contains(uGuiWidgetLogic))
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't open UGuiWidget, UGuiWidget '{0}' not in the container '{1}'!", uGuiWidgetLogic.name, Owner.Name));
            }

            if (uGuiWidgetLogic.Available)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't open UGuiWidget, UGuiWidget '{0}' is already opened!", uGuiWidgetLogic.name));
            }

            uGuiWidgetLogic.OnOpen(userData);
        }

        /// <summary>
        /// 动态打开 UGuiWidget，刷新 Depth。
        /// </summary>
        /// <param name="uGuiWidgetLogic"></param>
        /// <param name="userData"></param>
        public void DynamicOpenUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic, object userData = null)
        {
            OpenUGuiWidget(uGuiWidgetLogic, userData);
            uGuiWidgetLogic.OnDepthChanged(Owner.UIForm.UIGroup.Depth, Owner.UIForm.DepthInUIGroup);
        }

        public void CloseUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic, bool isShutdown, object userData = null)
        {
            if (uGuiWidgetLogic == null)
            {
                throw new GameFrameworkException("Can't open empty!");
            }

            if (!m_UGuiWidgets.Contains(uGuiWidgetLogic))
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't open UGuiWidget, UGuiWidget '{0}' not in the container '{1}'!", uGuiWidgetLogic.name, Owner.Name));
            }

            if (!uGuiWidgetLogic.Available)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't close UGuiWidget, UGuiWidget '{0}' is not opened!", uGuiWidgetLogic.name));
            }

            uGuiWidgetLogic.OnClose(isShutdown, userData);
        }

        public void CloseAllUGuiWidgets(bool isShutdown, object userData = null)
        {
            if (m_UGuiWidgets.Count > 0)
            {
                foreach (var uGuiWidget in m_UGuiWidgets)
                {
                    if (uGuiWidget.Available)
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
                if (uGuiWidget.Available)
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
                if (uGuiWidget.Available)
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
                if (uGuiWidget.Available)
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
                if (uGuiWidget.Available)
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
                if (uGuiWidget.Available)
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
                if (uGuiWidget.Available)
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
                if (uGuiWidget.Available)
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
                if (uGuiWidget.Available)
                {
                    uGuiWidget.OnDepthChanged(uiGroupDepth, depthInUIGroup);
                }
            }
        }
    }
}