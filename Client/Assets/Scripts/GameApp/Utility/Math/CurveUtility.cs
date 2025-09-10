using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 曲线与插值工具类
    /// </summary>
    public static class CurveUtility
    {
        /// <summary>
        /// 平滑步长插值（SmoothStep的改进版）
        /// </summary>
        public static float SmootherStep(float t, float min = 0, float max = 1)
        {
            t = Mathf.Clamp01((t - min) / (max - min));
            return t * t * t * (t * (6f * t - 15f) + 10f);
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
        /// 平滑阻尼（类似于Mathf.SmoothDamp但适用于向量）
        /// </summary>
        public static Vector3 SmoothDampVector(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime) => Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime);

        /// <summary>
        /// 角度平滑转向（处理360度跳转）
        /// </summary>
        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime)
        {
            float delta = Mathf.DeltaAngle(current, target);
            float newAngle = Mathf.SmoothDamp(current, current + delta, ref currentVelocity, smoothTime);
            return Mathf.Repeat(newAngle, 360);
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

        /// <summary>
        /// 线性贝塞尔曲线，根据 t 值计算两点之间的插值点
        /// </summary>
        public static Vector3 CalculateLineBezierPoint(float t, Vector3 start, Vector3 end)
        {
            float u = 1 - t;
            Vector3 p = u * start;
            p += t * end;
            return p;
        }

        /// <summary>
        /// 二次贝塞尔曲线，根据 t 值计算曲线上的点
        /// </summary>
        public static Vector3 CalculateCubicBezierPoint(float t, Vector3 start, Vector3 control, Vector3 end)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            Vector3 p = uu * start;
            p += 2 * u * t * control;
            p += tt * end;
            return p;
        }

        /// <summary>
        /// 三次贝塞尔曲线，根据 t 值计算曲线上的点
        /// </summary>
        public static Vector3 CalculateThreePowerBezierPoint(float t, Vector3 start, Vector3 control1, Vector3 control2, Vector3 end)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float ttt = tt * t;
            float uuu = uu * u;
            Vector3 p = uuu * start;
            p += 3 * t * uu * control1;
            p += 3 * tt * u * control2;
            p += ttt * end;
            return p;
        }

        /// <summary>
        /// 获取线性贝塞尔曲线的点集
        /// </summary>
        public static Vector3[] GetLineBezierList(Vector3 startPoint, Vector3 endPoint, int segmentNum)
        {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                path[i - 1] = CalculateLineBezierPoint(t, startPoint, endPoint);
            }

            return path;
        }

        /// <summary>
        /// 获取二次贝塞尔曲线的点集
        /// </summary>
        public static Vector3[] GetCubicBezierList(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segmentNum)
        {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                path[i - 1] = CalculateCubicBezierPoint(t, startPoint, controlPoint, endPoint);
            }

            return path;
        }

        /// <summary>
        /// 获取三次贝塞尔曲线的点集
        /// </summary>
        public static Vector3[] GetThreePowerBezierList(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint, int segmentNum)
        {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                path[i - 1] = CalculateThreePowerBezierPoint(t, startPoint, controlPoint1, controlPoint2, endPoint);
            }

            return path;
        }

        /// <summary>
        /// 抛物线运动计算
        /// </summary>
        public static Vector3 ParabolicMotion(Vector3 start, Vector3 end, float height, float t)
        {
            t = Mathf.Clamp01(t);
            float y = height * (4 * t * (1 - t));
            return Vector3.Lerp(start, end, t) + Vector3.up * y;
        }
    }
}