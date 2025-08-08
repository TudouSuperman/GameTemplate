using UnityEngine;

namespace GameApp.Hotfix.UI.Extension
{
    public sealed class RotationCom : MonoBehaviour
    {
        [SerializeField, Header("旋转速度"), Tooltip("旋转速度（度/秒）")]
        private float m_RotationSpeed = 180f;

        [SerializeField, Header("旋转方向"), Tooltip("旋转方向（顺时针或逆时针）")]
        private ERotationDirection m_RotationDirection = ERotationDirection.Clockwise;

        private void Update()
        {
            // 计算每帧的旋转角度
            float _rotationAmount = m_RotationSpeed * Time.deltaTime;

            // 根据旋转方向调整角度
            if (m_RotationDirection == ERotationDirection.Clockwise)
            {
                _rotationAmount *= -1; // 逆时针旋转
            }

            // 应用旋转
            transform.Rotate(0, 0, _rotationAmount);
        }
    }
}