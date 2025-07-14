using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameApp
{
    #region TODO 案例。

    // // 过场动画：光标自动移动到关键位置
    // IEnumerator CutsceneAnimation()
    // {
    //     // 禁用玩家控制
    //     GameApp.GameEntry.Cursor.SetCursorEnableFollow(false);
    //
    //     // 移动到位置A
    //     GameApp.GameEntry.Cursor.SetCursorPosition(positionA);
    //     yield return new WaitForSeconds(1f);
    //
    //     // 移动到位置B
    //     cursor.SetCursorPosition(positionB);
    //     yield return new WaitForSeconds(1f);
    //
    //     // 移动到位置C
    //     GameApp.GameEntry.Cursor.SetCursorPosition(positionC);
    //     yield return new WaitForSeconds(2f);
    //
    //     // 恢复控制
    //     GameApp.GameEntry.Cursor.SetCursorEnableFollow(true);
    //     GameApp.GameEntry.Cursor.ClearPositionOverride();
    // }

    #endregion

    public sealed class CursorComponent : GameFrameworkComponent
    {
        [SerializeField]
        private Transform m_InstanceRoot = null;

        [SerializeField]
        [Tooltip("主摄像机（自动获取或手动指定）。")]
        private Camera m_MainCamera;

        [SerializeField]
        [Tooltip("UI画布（用于坐标转换）。")]
        private Canvas m_UICanvas;

        [SerializeField]
        [Tooltip("光标在世界空间中的高度（Y轴偏移）。")]
        private float m_WorldHeightOffset = 0.1f;

        [Header("光标偏移设置")]
        [SerializeField]
        [Tooltip("光标相对于鼠标位置的偏移量（屏幕坐标）。")]
        private Vector2 m_CursorOffset = Vector2.zero;

        [SerializeField]
        [Tooltip("是否启用光标跟随鼠标。")]
        private bool m_CursorFollowFlag = false;

        private bool m_UseScreenSpaceUIFlag;
        private bool m_PositionOverriddenFlag = false;
        private Vector2 m_OverrideScreenPosition;

        /// <summary>
        /// 当前光标位置（屏幕坐标）。
        /// </summary>
        [field: SerializeField]
        public Vector2 ScreenPosition { get; private set; }

        /// <summary>
        /// 当前光标位置（世界坐标）。
        /// </summary>
        [field: SerializeField]
        public Vector3 WorldPosition { get; private set; }

        /// <summary>
        /// 当前光标位置（UI坐标）。
        /// </summary>
        [field: SerializeField]
        public Vector2 LocalPosition { get; private set; }

        /// <summary>
        /// 光标列表。
        /// </summary>
        [field: Header("光标列表")]
        [field: SerializeField]
        public RectTransform[] CursorRects { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            // 检测UI渲染模式。
            m_UseScreenSpaceUIFlag = m_UICanvas != null && m_UICanvas.renderMode == RenderMode.ScreenSpaceOverlay;
        }

        private void Update()
        {
            UpdateCursorPosition();
        }

        private void UpdateCursorPosition()
        {
            // 如果启用了位置覆盖，使用覆盖的位置。
            if (m_PositionOverriddenFlag)
            {
                ScreenPosition = m_OverrideScreenPosition;
            }
            // 否则根据设置更新位置。
            else if (m_CursorFollowFlag)
            {
                // 获取原始鼠标位置并应用偏移。
                ScreenPosition = UnityEngine.Input.mousePosition + (Vector3)m_CursorOffset;
            }
            else
            {
                return;
            }

            // 更新世界坐标。
            UpdateWorldPosition();
            // 更新UI坐标。
            UpdateLocalPosition();
            // 移动光标实例。
            MoveCursorInstance();
        }

        private void UpdateWorldPosition()
        {
            // 创建从摄像机发射的射线。
            Ray _ray = m_MainCamera.ScreenPointToRay(ScreenPosition);
            // 检测与地面的交点（假设地面在y=0）。
            if (Physics.Raycast(_ray, out RaycastHit _hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                WorldPosition = _hit.point + Vector3.up * m_WorldHeightOffset;
            }
            else
            {
                // 无碰撞时使用默认距离。
                float _defaultDistance = 10f;
                WorldPosition = _ray.GetPoint(_defaultDistance);
            }
        }

        private void UpdateLocalPosition()
        {
            Camera _uiCamera = m_UseScreenSpaceUIFlag ? null : m_UICanvas.worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)m_UICanvas.transform,
                ScreenPosition,
                _uiCamera,
                out Vector2 _localPos
            );
            LocalPosition = _localPos;
        }

        private void MoveCursorInstance()
        {
            foreach (RectTransform _rect in CursorRects)
            {
                _rect.anchoredPosition = LocalPosition;
            }
        }

        /// <summary>
        /// 设置光标偏移量（屏幕坐标）。
        /// </summary>
        /// <param name="offset">偏移量。</param>
        public void SetCursorOffset(Vector2 offset)
        {
            m_CursorOffset = offset;
        }

        /// <summary>
        /// 启用或禁用光标跟随鼠标。
        /// </summary>
        /// <param name="enable">是否启用。</param>
        public void SetCursorEnableFollow(bool enable)
        {
            m_CursorFollowFlag = enable;
        }

        /// <summary>
        /// 直接设置光标位置（屏幕坐标）。
        /// 注意：这会覆盖鼠标位置。
        /// </summary>
        /// <param name="screenPosition">屏幕坐标位置。</param>
        public void SetCursorPosition(Vector2 screenPosition)
        {
            m_OverrideScreenPosition = screenPosition;
            m_PositionOverriddenFlag = true;
            // 立即更新位置。
            ScreenPosition = screenPosition;
            UpdateWorldPosition();
            UpdateLocalPosition();
            MoveCursorInstance();
        }

        /// <summary>
        /// 设置光标位置并指定是否保持覆盖。
        /// </summary>
        /// <param name="screenPosition">屏幕坐标位置。</param>
        /// <param name="keepOverride">是否保持覆盖（直到调用ClearPositionOverride）。</param>
        public void SetCursorPosition(Vector2 screenPosition, bool keepOverride)
        {
            SetCursorPosition(screenPosition);
            m_PositionOverriddenFlag = keepOverride;
        }

        /// <summary>
        /// 清除位置覆盖，恢复跟随鼠标。
        /// </summary>
        public void ClearPositionOverride()
        {
            m_PositionOverriddenFlag = false;
        }
    }
}