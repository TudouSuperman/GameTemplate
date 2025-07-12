using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameApp
{
    public sealed class CursorComponent : GameFrameworkComponent
    {
        [SerializeField]
        private Transform m_InstanceRoot = null;

        /// <summary>
        /// 当前光标位置（屏幕坐标）。
        /// </summary>
        public Vector2 ScreenPosition { get; private set; }

        /// <summary>
        /// 当前光标位置（世界坐标）。
        /// </summary>
        public Vector3 WorldPosition { get; private set; }

        /// <summary>
        /// 当前光标位置（UI坐标）。
        /// </summary>
        public Vector2 LocalPosition { get; private set; }
    }
}