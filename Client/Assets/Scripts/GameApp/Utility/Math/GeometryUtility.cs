using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 几何工具类
    /// </summary>
    public static class GeometryUtility
    {
        // ========== 距离和范围检测 ========== //

        /// <summary>
        /// 检查点是否在圆形/球形区域内（高效版，使用平方距离）
        /// </summary>
        /// <param name="point">要检查的点</param>
        /// <param name="center">圆心/球心</param>
        /// <param name="radius">半径</param>
        /// <returns>是否在区域内</returns>
        public static bool IsPointInCircle(Vector3 point, Vector3 center, float radius)
        {
            float sqrDistance = (point - center).sqrMagnitude;
            return sqrDistance <= radius * radius;
        }

        /// <summary>
        /// 检查点是否在轴对齐的矩形/立方体内 (AABB)
        /// </summary>
        /// <param name="point">要检查的点</param>
        /// <param name="center">中心点</param>
        /// <param name="size">区域大小（x=宽，y=高，z=深）</param>
        /// <returns>是否在区域内</returns>
        public static bool IsPointInRectangle(Vector3 point, Vector3 center, Vector3 size)
        {
            Vector3 halfSize = size * 0.5f;
            return Mathf.Abs(point.x - center.x) <= halfSize.x &&
                   Mathf.Abs(point.y - center.y) <= halfSize.y &&
                   Mathf.Abs(point.z - center.z) <= halfSize.z;
        }

        /// <summary>
        /// 检查点是否在扇形区域内
        /// </summary>
        /// <param name="point">要检查的点</param>
        /// <param name="sectorCenter">扇形中心</param>
        /// <param name="sectorDirection">扇形方向</param>
        /// <param name="sectorAngle">扇形角度（度）</param>
        /// <param name="sectorRadius">扇形半径</param>
        /// <returns>是否在扇形内</returns>
        public static bool IsPointInSector(Vector3 point, Vector3 sectorCenter, Vector3 sectorDirection,
            float sectorAngle, float sectorRadius)
        {
            // 检查距离
            Vector3 toPoint = point - sectorCenter;
            float sqrDistance = toPoint.sqrMagnitude;
            if (sqrDistance > sectorRadius * sectorRadius)
                return false;

            // 检查角度
            float angle = Vector3.Angle(sectorDirection, toPoint);
            return angle <= sectorAngle * 0.5f;
        }

        // ========== 向量操作 ========== //

        /// <summary>
        /// 计算向量在另一个向量上的投影
        /// </summary>
        public static Vector3 ProjectVector(Vector3 vector, Vector3 onNormal)
        {
            return Vector3.Project(vector, onNormal);
        }

        /// <summary>
        /// 计算向量在平面上的投影
        /// </summary>
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            return Vector3.ProjectOnPlane(vector, planeNormal);
        }

        /// <summary>
        /// 绕轴旋转向量
        /// </summary>
        /// <param name="vector">要旋转的向量</param>
        /// <param name="axis">旋转轴（单位向量）</param>
        /// <param name="angle">旋转角度（度）</param>
        /// <returns>旋转后的向量</returns>
        public static Vector3 RotateVectorAroundAxis(Vector3 vector, Vector3 axis, float angle)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            return rotation * vector;
        }

        /// <summary>
        /// 计算两个向量之间的夹角（0-180度）
        /// </summary>
        public static float AngleBetweenVectors(Vector3 from, Vector3 to)
        {
            return Vector3.Angle(from, to);
        }

        /// <summary>
        /// 计算带符号的两个向量之间的夹角（-180到180度）
        /// </summary>
        public static float SignedAngleBetweenVectors(Vector3 from, Vector3 to, Vector3 axis)
        {
            return Vector3.SignedAngle(from, to, axis);
        }

        // ========== 几何计算 ========== //

        /// <summary>
        /// 计算点到线段的最近点和距离
        /// </summary>
        /// <param name="point">点</param>
        /// <param name="lineStart">线段起点</param>
        /// <param name="lineEnd">线段终点</param>
        /// <param name="closestPoint">输出最近点</param>
        /// <returns>点到线段的距离</returns>
        public static float DistanceToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd, out Vector3 closestPoint)
        {
            Vector3 lineDirection = lineEnd - lineStart;
            float lineLength = lineDirection.magnitude;

            if (lineLength < Mathf.Epsilon)
            {
                closestPoint = lineStart;
                return Vector3.Distance(point, lineStart);
            }

            Vector3 normalizedDirection = lineDirection.normalized;
            float projection = Vector3.Dot(point - lineStart, normalizedDirection);

            if (projection <= 0)
            {
                closestPoint = lineStart;
                return Vector3.Distance(point, lineStart);
            }

            if (projection >= lineLength)
            {
                closestPoint = lineEnd;
                return Vector3.Distance(point, lineEnd);
            }

            closestPoint = lineStart + normalizedDirection * projection;
            return Vector3.Distance(point, closestPoint);
        }

        /// <summary>
        /// 计算两条线段之间的最短距离
        /// </summary>
        public static float DistanceBetweenLineSegments(Vector3 aStart, Vector3 aEnd, Vector3 bStart, Vector3 bEnd)
        {
            Vector3 u = aEnd - aStart;
            Vector3 v = bEnd - bStart;
            Vector3 w = aStart - bStart;

            float a = Vector3.Dot(u, u);
            float bVal = Vector3.Dot(u, v);
            float c = Vector3.Dot(v, v);
            float d = Vector3.Dot(u, w);
            float e = Vector3.Dot(v, w);

            float denominator = a * c - bVal * bVal;
            float sc, tc;

            if (denominator < 0.0001f)
            {
                sc = 0.0f;
                tc = d / c;
            }
            else
            {
                sc = (bVal * e - c * d) / denominator;
                tc = (a * e - bVal * d) / denominator;
            }

            sc = Mathf.Clamp01(sc);
            tc = Mathf.Clamp01(tc);

            Vector3 pointOnA = aStart + sc * u;
            Vector3 pointOnB = bStart + tc * v;

            return Vector3.Distance(pointOnA, pointOnB);
        }

        /// <summary>
        /// 计算三角形面积
        /// </summary>
        public static float TriangleArea(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            return Vector3.Cross(ab, ac).magnitude * 0.5f;
        }
    }
}