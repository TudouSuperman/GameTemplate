using System;
using System.Threading;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using Cysharp.Threading.Tasks;

namespace GameApp
{
    public abstract class UGuiFormLogicEx : UGuiFormLogic
    {
        private CancellationTokenSource m_CancellationTokenSource;

        private UGuiWidgetContainer m_UGuiWidgetContainer;
        private EventContainer m_EventContainer;
        private EntityContainer m_EntityContainer;
        private ResourceContainer m_ResourceContainer;

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

        protected override void OnRecycle()
        {
            base.OnRecycle();

            m_UGuiWidgetContainer?.OnRecycle();
        }

        private void OnDestroy()
        {
            RemoveAllUGuiWidget();
            ClearContainer();
        }

        protected override void OnClose(bool isShutdown, object userData)
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

        protected override void OnPause()
        {
            base.OnPause();
            m_UGuiWidgetContainer?.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            m_UGuiWidgetContainer?.OnResume();
        }

        protected override void OnCover()
        {
            base.OnCover();
            m_UGuiWidgetContainer?.OnCover();
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            m_UGuiWidgetContainer?.OnReveal();
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            m_UGuiWidgetContainer?.OnRefocus(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            m_UGuiWidgetContainer?.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            m_UGuiWidgetContainer?.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        public void AddUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic, object userData = null)
        {
            if (m_UGuiWidgetContainer == null)
            {
                m_UGuiWidgetContainer = UGuiWidgetContainer.Create(this);
            }

            m_UGuiWidgetContainer.AddUGuiWidget(uGuiWidgetLogic, userData);
        }

        public bool HasUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic)
        {
            if (m_UGuiWidgetContainer == null)
            {
                return false;
            }

            return m_UGuiWidgetContainer.HasUGuiWidget(uGuiWidgetLogic);
        }

        public void RemoveUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic)
        {
            if (m_UGuiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }

            m_UGuiWidgetContainer.RemoveUGuiWidget(uGuiWidgetLogic);
        }

        public void RemoveAllUGuiWidget()
        {
            if (m_UGuiWidgetContainer == null)
                return;
            m_UGuiWidgetContainer.RemoveAllUGuiWidget();
        }

        /// <summary>
        /// 打开 UGuiWidget，不刷新 Depth，一般在 UIForm 的 OnOpen 中调用。
        /// </summary>
        /// <param name="uGuiWidgetLogic"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void OpenUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic, object userData = null)
        {
            if (m_UGuiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }

            m_UGuiWidgetContainer.OpenUGuiWidget(uGuiWidgetLogic, userData);
        }

        /// <summary>
        /// 动态打开 UGuiWidget，刷新 Depth。
        /// </summary>
        /// <param name="uGuiWidgetLogic"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void DynamicOpenUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic, object userData = null)
        {
            if (m_UGuiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }

            m_UGuiWidgetContainer.DynamicOpenUGuiWidget(uGuiWidgetLogic, userData);
        }

        public void CloseUGuiWidget(UGuiWidgetLogic uGuiWidgetLogic, bool isShutdown = false, object userData = null)
        {
            if (m_UGuiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }

            m_UGuiWidgetContainer.CloseUGuiWidget(uGuiWidgetLogic, isShutdown, userData);
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