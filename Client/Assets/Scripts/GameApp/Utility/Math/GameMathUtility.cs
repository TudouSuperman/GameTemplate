using System.Collections.Generic;
using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 游戏数学工具类
    /// </summary>
    public static class GameMathUtility
    {
        // ==================== 游戏专用数学 ==================== //

        /// <summary>
        /// 计算伤害衰减（基于距离）
        /// 示例：DamageFalloff(100, 10, 50, 30) → 50m处伤害
        /// </summary>
        public static float DamageFalloff(float maxDamage, float minDamage, float maxRange, float distance)
        {
            distance = Mathf.Clamp(distance, 0, maxRange);
            float t = distance / maxRange;
            return Mathf.Lerp(maxDamage, minDamage, t * t); // 平方衰减更真实
        }

        /// <summary>
        /// 角度平滑转向（处理360度跳转）
        /// 示例：SmoothDampAngle(currentAngle, targetAngle, ref velocity, smoothTime)
        /// </summary>
        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime)
        {
            // 处理360度跳转问题
            float delta = Mathf.DeltaAngle(current, target);
            float newAngle = Mathf.SmoothDamp(current, current + delta, ref currentVelocity, smoothTime);
            return Mathf.Repeat(newAngle, 360);
        }

        /// <summary>
        /// 计算经验值需求（指数增长）
        /// 示例：ExperienceForLevel(10, 100, 1.5f) → 第10级所需经验
        /// </summary>
        public static int ExperienceForLevel(int level, int baseExp, float growthFactor)
        {
            return Mathf.FloorToInt(baseExp * Mathf.Pow(growthFactor, level - 1));
        }

        /// <summary>
        /// 视野锥检测（点是否在视野范围内）
        /// 示例：IsInFieldOfView(viewerPosition, viewerDirection, targetPosition, 90)
        /// </summary>
        public static bool IsInFieldOfView(Vector3 viewerPosition, Vector3 viewerDirection, Vector3 targetPosition, float fovAngle)
        {
            Vector3 toTarget = (targetPosition - viewerPosition).normalized;
            float angle = Vector3.Angle(viewerDirection, toTarget);
            return angle <= fovAngle / 2;
        }

        // ==================== 物理计算 ==================== //

        /// <summary>
        /// 计算抛射物初始速度（忽略空气阻力）
        /// 示例：CalculateProjectileVelocity(start, target, gravity, angle)
        /// </summary>
        public static Vector3 CalculateProjectileVelocity(Vector3 start, Vector3 target, float gravity, float angle)
        {
            Vector3 direction = target - start;
            float height = direction.y;
            direction.y = 0;
            float horizontalDistance = direction.magnitude;

            float radianAngle = angle * Mathf.Deg2Rad;
            float velocity = Mathf.Sqrt(horizontalDistance * gravity / Mathf.Sin(2 * radianAngle));

            Vector3 launchDirection = direction.normalized;
            launchDirection.y = Mathf.Tan(radianAngle);

            return launchDirection.normalized * velocity;
        }

        /// <summary>
        /// 计算碰撞后的反弹向量
        /// 示例：CalculateBounceDirection(velocity, normal, 0.8f)
        /// </summary>
        public static Vector3 CalculateBounceDirection(Vector3 velocity, Vector3 normal, float elasticity)
        {
            float velocityDotNormal = Vector3.Dot(velocity, normal);
            if (velocityDotNormal > 0) return velocity;

            Vector3 bounce = velocity - (1 + elasticity) * velocityDotNormal * normal;
            return bounce;
        }

        // ==================== 颜色与图形 ==================== //

        /// <summary>
        /// RGB转HSV颜色空间
        /// </summary>
        public static Vector3 RGBToHSV(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            return new Vector3(h, s, v);
        }

        /// <summary>
        /// HSV转RGB颜色空间
        /// </summary>
        public static Color HSVToRGB(Vector3 hsv)
        {
            return Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
        }

        /// <summary>
        /// 判断点是否在多边形内（2D）
        /// 示例：IsPointInPolygon(point, polygonVertices)
        /// </summary>
        public static bool IsPointInPolygon(Vector2 point, List<Vector2> polygon)
        {
            bool inside = false;
            int count = polygon.Count;

            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                    (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) /
                        (polygon[j].y - polygon[i].y) + polygon[i].x))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}