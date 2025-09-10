using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 插值工具类
    /// </summary>
    public static class InterpolationUtility
    {
        // ========== 插值和平滑 ========== //

        /// <summary>
        /// 平滑阻尼（类似于Mathf.SmoothDamp但适用于向量）
        /// </summary>
        public static Vector3 SmoothDampVector(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
        {
            return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime);
        }

        /// <summary>
        /// 双曲插值，比线性插值更平滑
        /// </summary>
        public static float LerpHyperbolic(float a, float b, float t)
        {
            t = Mathf.Clamp01(t);
            return a + (b - a) * (t * t * (3f - 2f * t));
        }

        /// <summary>
        /// 基于Perlin噪声的平滑随机值
        /// </summary>
        public static float PerlinNoiseLerp(float a, float b, float t, float noiseScale = 0.1f)
        {
            float noise = Mathf.PerlinNoise(t * noiseScale, 0);
            return Mathf.Lerp(a, b, noise);
        }

        /// <summary>
        /// 平滑地旋转朝向目标方向
        /// </summary>
        public static Quaternion SmoothRotateTowards(Quaternion current, Quaternion target, ref float currentVelocity, float smoothTime)
        {
            Vector3 currentEuler = current.eulerAngles;
            Vector3 targetEuler = target.eulerAngles;

            currentEuler.x = Mathf.SmoothDampAngle(currentEuler.x, targetEuler.x, ref currentVelocity, smoothTime);
            currentEuler.y = Mathf.SmoothDampAngle(currentEuler.y, targetEuler.y, ref currentVelocity, smoothTime);
            currentEuler.z = Mathf.SmoothDampAngle(currentEuler.z, targetEuler.z, ref currentVelocity, smoothTime);

            return Quaternion.Euler(currentEuler);
        }
    }
}