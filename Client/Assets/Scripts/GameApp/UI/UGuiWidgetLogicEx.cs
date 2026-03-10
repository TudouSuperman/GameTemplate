using System;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;
using Cysharp.Threading.Tasks;

namespace GameApp
{
    public abstract class UGuiWidgetLogicEx : UGuiWidgetLogic
    {
        private UGuiFormLogicEx m_ParentUGuiForm;
        private UGuiWidgetLogicEx m_ParentUGuiWidget;
        private UGuiWidgetContainer m_UGuiWidgetContainer;
        private EventContainer m_EventContainer;
        private EntityContainer m_EntityContainer;
        private ResourceContainer m_ResourceContainer;

        /// <summary>
        /// 父 UGuiForm（扩展）。
        /// </summary>
        public UGuiFormLogicEx ParentUGuiForm => m_ParentUGuiForm;

        /// <summary>
        /// 父 UGuiWidget（扩展）。
        /// </summary>
        public UGuiWidgetLogicEx ParentUGuiWidget => m_ParentUGuiWidget;

        public override void SetOwner(UGuiFormLogic uGuiFormLogic)
        {
            base.SetOwner(uGuiFormLogic);

            m_ParentUGuiForm = uGuiFormLogic as UGuiFormLogicEx;
        }

        public void Open(object userData = null)
        {
            if (m_ParentUGuiWidget != null)
            {
                m_ParentUGuiWidget.OpenUGuiWidget(this, userData);
                return;
            }

            if (m_ParentUGuiForm != null)
            {
                m_ParentUGuiForm.OpenUGuiWidget(this, userData);
                return;
            }

            throw new GameFrameworkException("UGuiWidget is invalid.");
        }

        public void TryOpen(object userData = null)
        {
            if (Available) return;
            if (m_ParentUGuiWidget != null)
            {
                m_ParentUGuiWidget.OpenUGuiWidget(this, userData);
                return;
            }

            if (m_ParentUGuiForm != null)
            {
                m_ParentUGuiForm.OpenUGuiWidget(this, userData);
                return;
            }
        }

        public void DynamicOpen(object userData = null)
        {
            if (m_ParentUGuiWidget != null)
            {
                m_ParentUGuiWidget.DynamicOpenUGuiWidget(this, userData);
                return;
            }

            if (m_ParentUGuiForm != null)
            {
                m_ParentUGuiForm.DynamicOpenUGuiWidget(this, userData);
                return;
            }

            throw new GameFrameworkException("UGuiWidget is invalid.");
        }

        public void TryDynamicOpen(object userData = null)
        {
            if (Available) return;
            if (m_ParentUGuiWidget != null)
            {
                m_ParentUGuiWidget.DynamicOpenUGuiWidget(this, userData);
                return;
            }

            if (m_ParentUGuiForm != null)
            {
                m_ParentUGuiForm.DynamicOpenUGuiWidget(this, userData);
                return;
            }
        }

        public void Close()
        {
            if (m_ParentUGuiWidget != null)
            {
                m_ParentUGuiWidget.CloseUGuiWidget(this);
                return;
            }

            if (m_ParentUGuiForm != null)
            {
                m_ParentUGuiForm.CloseUGuiWidget(this);
                return;
            }

            throw new GameFrameworkException("UGuiWidget is invalid.");
        }

        public void TryClose()
        {
            if (Available) return;
            if (m_ParentUGuiWidget != null)
            {
                m_ParentUGuiWidget.CloseUGuiWidget(this);
                return;
            }

            if (m_ParentUGuiForm != null)
            {
                m_ParentUGuiForm.CloseUGuiWidget(this);
                return;
            }
        }

        public bool Has()
        {
            if (m_ParentUGuiWidget != null)
            {
                return m_ParentUGuiWidget.HasUGuiWidget(this);
            }

            if (m_ParentUGuiForm != null)
            {
                return m_ParentUGuiForm.HasUGuiWidget(this);
            }

            return false;
        }

        public void Remove()
        {
            if (m_ParentUGuiWidget != null)
            {
                m_ParentUGuiWidget.RemoveUGuiWidget(this);
                return;
            }

            if (m_ParentUGuiForm != null)
            {
                m_ParentUGuiForm.RemoveUGuiWidget(this);
                return;
            }
        }

        private void ClearContainer()
        {
            if (m_EventContainer != null)
            {
                ReferencePool.Release(m_EventContainer);
                m_EventContainer = null;
            }

            if (m_EntityContainer != null)
            {
                ReferencePool.Release(m_EntityContainer);
                m_EntityContainer = null;
            }

            if (m_UGuiWidgetContainer != null)
            {
                ReferencePool.Release(m_UGuiWidgetContainer);
                m_UGuiWidgetContainer = null;
            }

            if (m_ResourceContainer != null)
            {
                ReferencePool.Release(m_ResourceContainer);
                m_ResourceContainer = null;
            }
        }

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);

            UGFList<UGuiWidgetLogicEx> _uGuiWidgetLogicExList = new UGFList<UGuiWidgetLogicEx>();
            GetComponentsInChildren(true, _uGuiWidgetLogicExList);
            foreach (UGuiWidgetLogicEx uGuiWidgetLogicEx in _uGuiWidgetLogicExList)
            {
                if (uGuiWidgetLogicEx == this
                    || uGuiWidgetLogicEx.Owner != null
                    || uGuiWidgetLogicEx.GetComponentInParent<UGuiWidgetLogicEx>(true) != this)
                    continue;
                AddUGuiWidget(uGuiWidgetLogicEx, userData);
            }

            _uGuiWidgetLogicExList.Dispose();
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();

            m_UGuiWidgetContainer?.OnRecycle();
        }

        protected virtual void OnDestroy()
        {
            RemoveAllUGuiWidget();
            ClearContainer();
        }

        protected internal override void OnClose(bool isShutdown, object userData)
        {
            m_UGuiWidgetContainer?.OnClose(isShutdown, userData);
            HideAllEntity(isShutdown);
            UnsubscribeAll(isShutdown);
            UnloadAllAssets(isShutdown);
            CloseAllUGuiWidgets(isShutdown, userData);
            if (isShutdown)
            {
                RemoveAllUGuiWidget();
                ClearContainer();
            }

            base.OnClose(isShutdown, userData);
        }

        protected internal override void OnPause()
        {
            base.OnPause();

            m_UGuiWidgetContainer?.OnPause();
        }

        protected internal override void OnResume()
        {
            base.OnResume();

            m_UGuiWidgetContainer?.OnResume();
        }

        protected internal override void OnCover()
        {
            base.OnCover();

            m_UGuiWidgetContainer?.OnCover();
        }

        protected internal override void OnReveal()
        {
            base.OnReveal();

            m_UGuiWidgetContainer?.OnReveal();
        }

        protected internal override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);

            m_UGuiWidgetContainer?.OnRefocus(userData);
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            m_UGuiWidgetContainer?.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected internal override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);

            m_UGuiWidgetContainer?.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        public void AddUGuiWidget(UGuiWidgetLogicEx uGuiWidgetLogicEx, object userData = null)
        {
            if (m_UGuiWidgetContainer == null)
            {
                m_UGuiWidgetContainer = UGuiWidgetContainer.Create(Owner);
            }

            uGuiWidgetLogicEx.m_ParentUGuiWidget = this;
            m_UGuiWidgetContainer.AddUGuiWidget(uGuiWidgetLogicEx, userData);
        }

        public bool HasUGuiWidget(UGuiWidgetLogicEx uGuiWidgetLogicEx)
        {
            return m_UGuiWidgetContainer != null && m_UGuiWidgetContainer.HasUGuiWidget(uGuiWidgetLogicEx);
        }

        public void RemoveUGuiWidget(UGuiWidgetLogicEx uGuiWidgetLogicEx)
        {
            if (m_UGuiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }

            m_UGuiWidgetContainer.RemoveUGuiWidget(uGuiWidgetLogicEx);
            uGuiWidgetLogicEx.m_ParentUGuiWidget = null;
        }

        public void RemoveAllUGuiWidget()
        {
            if (m_UGuiWidgetContainer == null) return;
            using UGFList<UGuiWidgetLogicEx> _uGuiWidgetLogicExList = UGFList<UGuiWidgetLogicEx>.Create();
            foreach (UGuiWidgetLogic _uGuiWidgetLogic in m_UGuiWidgetContainer.UGuiWidgets)
            {
                UGuiWidgetLogicEx _uGuiWidgetLogicEx = (UGuiWidgetLogicEx)_uGuiWidgetLogic;
                _uGuiWidgetLogicExList.Add(_uGuiWidgetLogicEx);
            }

            m_UGuiWidgetContainer.RemoveAllUGuiWidget();
            foreach (UGuiWidgetLogicEx _uGuiWidgetLogicEx in _uGuiWidgetLogicExList)
            {
                _uGuiWidgetLogicEx.m_ParentUGuiWidget = null;
            }
        }

        /// <summary>
        /// 打开 UGuiWidget，不刷新 Depth，一般在 UIForm 的 OnOpen 中调用。
        /// </summary>
        public void OpenUGuiWidget(UGuiWidgetLogicEx uGuiWidgetLogicEx, object userData = null)
        {
            if (m_UGuiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }

            m_UGuiWidgetContainer.OpenUGuiWidget(uGuiWidgetLogicEx, userData);
        }
        
        /// <summary>
        /// 打开所有的 UGuiWidget，不刷新 Depth，一般在 UIForm 的 OnOpen 中调用。
        /// </summary>
        public void OpenAllUGuiWidget()
        {
            if (m_UGuiWidgetContainer == null) return;
            UGFList<UGuiWidgetLogic> _uGuiWidgetLogicList = UGFList<UGuiWidgetLogic>.Create();
            m_UGuiWidgetContainer.GetAllUGuiWidgets(_uGuiWidgetLogicList);
            foreach (UGuiWidgetLogic _uGuiWidgetLogic in _uGuiWidgetLogicList)
            {
                if (!_uGuiWidgetLogic.Available)
                {
                    m_UGuiWidgetContainer.OpenUGuiWidget(_uGuiWidgetLogic);
                }
            }

            _uGuiWidgetLogicList.Dispose();
        }

        /// <summary>
        /// 动态打开 UGuiWidget，刷新 Depth。
        /// </summary>
        public void DynamicOpenUGuiWidget(UGuiWidgetLogicEx uGuiWidgetLogicEx, object userData = null)
        {
            if (m_UGuiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }

            m_UGuiWidgetContainer.DynamicOpenUGuiWidget(uGuiWidgetLogicEx, userData);
        }

        public void CloseUGuiWidget(UGuiWidgetLogicEx uGuiWidgetLogicEx, bool isShutdown = false, object userData = null)
        {
            if (m_UGuiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }

            m_UGuiWidgetContainer.CloseUGuiWidget(uGuiWidgetLogicEx, isShutdown, userData);
        }

        public void CloseAllUGuiWidgets(bool isShutdown = false, object userData = null)
        {
            if (m_UGuiWidgetContainer == null) return;
            m_UGuiWidgetContainer.CloseAllUGuiWidgets(isShutdown, userData);
        }

        public void Subscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventContainer == null)
            {
                m_EventContainer = EventContainer.Create(this);
            }

            m_EventContainer.Subscribe(id, handler);
        }

        public void Unsubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventContainer == null) return;
            m_EventContainer.Unsubscribe(id, handler);
        }

        public void TryUnsubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventContainer == null) return;
            m_EventContainer.TryUnsubscribe(id, handler);
        }

        public void UnsubscribeAll()
        {
            UnsubscribeAll(false);
        }

        public void UnsubscribeAll(bool isShutdown)
        {
            if (m_EventContainer == null) return;
            m_EventContainer.UnsubscribeAll(isShutdown);
        }

        public void TryUnsubscribeAll()
        {
            TryUnsubscribeAll(false);
        }

        public void TryUnsubscribeAll(bool isShutdown)
        {
            if (m_EventContainer == null) return;
            m_EventContainer.TryUnsubscribeAll(isShutdown);
        }

        public int? ShowEntity<T>(int entityTypeId, Action<Entity> onShowSuccess, Action onShowFailure = null) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }

            return m_EntityContainer.ShowEntity<T>(entityTypeId, onShowSuccess, onShowFailure);
        }

        public int? ShowEntity(int entityTypeId, Type logicType, Action<Entity> onShowSuccess, Action onShowFailure = null)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }

            return m_EntityContainer.ShowEntity(entityTypeId, logicType, onShowSuccess, onShowFailure);
        }

        public int? ShowEntity<T>(int entityTypeId, object userData = null) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }

            return m_EntityContainer.ShowEntity<T>(entityTypeId, userData);
        }

        public int? ShowEntity(int entityTypeId, Type logicType, object userData = null)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }

            return m_EntityContainer.ShowEntity(entityTypeId, logicType, userData);
        }

        public UniTask<Entity> ShowEntityAsync<T>(int entityTypeId, object userData = null) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }

            return m_EntityContainer.ShowEntityAsync(entityTypeId, typeof(T), userData);
        }

        public UniTask<Entity> ShowEntityAsync(int entityTypeId, Type logicType, object userData = null)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }

            return m_EntityContainer.ShowEntityAsync(entityTypeId, logicType, userData);
        }

        public void HideAllEntity()
        {
            HideAllEntity(false);
        }

        public void HideAllEntity(bool isShutdown)
        {
            if (m_EntityContainer == null) return;
            m_EntityContainer.HideAllEntity(isShutdown);
        }

        public void TryHideAllEntity()
        {
            TryHideAllEntity(false);
        }

        public void TryHideAllEntity(bool isShutdown)
        {
            if (m_EntityContainer == null) return;
            m_EntityContainer.TryHideAllEntity(isShutdown);
        }

        public void HideEntity(int serialId)
        {
            if (m_EntityContainer == null) return;
            m_EntityContainer.HideEntity(serialId);
        }

        public void HideEntity(Entity entity)
        {
            if (m_EntityContainer == null) return;
            m_EntityContainer.HideEntity(entity);
        }

        public void TryHideEntity(int serialId)
        {
            if (m_EntityContainer == null) return;
            m_EntityContainer.TryHideEntity(serialId);
        }

        public void TryHideEntity(Entity entity)
        {
            if (m_EntityContainer == null) return;
            m_EntityContainer.TryHideEntity(entity);
        }

        public void LoadAsset<T>
        (
            string assetName,
            Action<T> onLoadSuccess,
            Action onLoadFailure = null,
            int priority = 0,
            Action<float> updateEvent = null,
            Action<string> dependencyAssetEvent = null
        ) where T : UnityEngine.Object
        {
            if (m_ResourceContainer == null)
            {
                m_ResourceContainer = ResourceContainer.Create(this);
            }

            m_ResourceContainer.LoadAsset(assetName, onLoadSuccess, onLoadFailure, priority, updateEvent, dependencyAssetEvent);
        }

        public UniTask<T> LoadAssetAsync<T>
        (
            string assetName,
            int priority = 0,
            Action<float> updateEvent = null,
            Action<string> dependencyAssetEvent = null
        ) where T : UnityEngine.Object
        {
            if (m_ResourceContainer == null)
            {
                m_ResourceContainer = ResourceContainer.Create(this);
            }

            return m_ResourceContainer.LoadAssetAsync<T>(assetName, priority, updateEvent, dependencyAssetEvent);
        }

        public void UnloadAsset(UnityEngine.Object asset)
        {
            if (m_ResourceContainer == null) return;
            m_ResourceContainer.UnloadAsset(asset);
        }

        public void UnloadAllAssets()
        {
            if (m_ResourceContainer == null) return;
            m_ResourceContainer.UnloadAllAssets();
        }

        public void UnloadAllAssets(bool isShutdown)
        {
            if (m_ResourceContainer == null) return;
            m_ResourceContainer.UnloadAllAssets(isShutdown);
        }
    }
}