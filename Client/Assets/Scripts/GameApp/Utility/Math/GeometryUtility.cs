using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 几何数学工具类
    /// </summary>
    public static class GeometryUtility
    {
        /// <summary>
        /// 二维向量点积
        /// </summary>
        public static float DotProduct(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

        /// <summary>
        /// 三维向量叉积
        /// </summary>
        public static Vector3 CrossProduct(Vector3 a, Vector3 b) => Vector3.Cross(a, b);

        /// <summary>
        /// 计算两点间距离（忽略Y轴）
        /// </summary>
        public static float Distance2D(Vector3 a, Vector3 b)
        {
            a.y = b.y;
            return Vector3.Distance(a, b);
        }

        /// <summary>
        /// 计算点到直线的距离
        /// </summary>
        public static float DistanceToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 lineVec = lineEnd - lineStart;
            Vector3 pointVec = point - lineStart;
            float lineLength = lineVec.magnitude;

            if (lineLength < Mathf.Epsilon)
                return Vector3.Distance(point, lineStart);

            Vector3 normalizedLineVec = lineVec / lineLength;
            float projection = Vector3.Dot(pointVec, normalizedLineVec);
            projection = Mathf.Clamp(projection, 0, lineLength);

            Vector3 closestPoint = lineStart + normalizedLineVec * projection;
            return Vector3.Distance(point, closestPoint);
        }

        /// <summary>
        /// 计算点到线段的最近点和距离
        /// </summary>
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

        /// <summary>
        /// 计算圆上点
        /// </summary>
        public static Vector3 PointOnCircle(Vector3 center, float radius, float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;
            return center + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
        }

        /// <summary>
        /// 检查点是否在圆形/球形区域内（高效版，使用平方距离）
        /// </summary>
        public static bool IsPointInCircle(Vector3 point, Vector3 center, float radius)
        {
            float sqrDistance = (point - center).sqrMagnitude;
            return sqrDistance <= radius * radius;
        }

        /// <summary>
        /// 检查点是否在轴对齐的矩形/立方体内 (AABB)
        /// </summary>
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

        /// <summary>
        /// 判断点是否在多边形内（2D）
        /// </summary>
        public static bool IsPointInPolygon(Vector2 point, System.Collections.Generic.List<Vector2> polygon)
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

        /// <summary>
        /// 视野锥检测（点是否在视野范围内）
        /// </summary>
        public static bool IsInFieldOfView(Vector3 viewerPosition, Vector3 viewerDirection, Vector3 targetPosition, float fovAngle)
        {
            Vector3 toTarget = (targetPosition - viewerPosition).normalized;
            float angle = Vector3.Angle(viewerDirection, toTarget);
            return angle <= fovAngle / 2;
        }

        /// <summary>
        /// 计算向量在另一个向量上的投影
        /// </summary>
        public static Vector3 ProjectVector(Vector3 vector, Vector3 onNormal) => Vector3.Project(vector, onNormal);

        /// <summary>
        /// 计算向量在平面上的投影
        /// </summary>
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal) => Vector3.ProjectOnPlane(vector, planeNormal);

        /// <summary>
        /// 绕轴旋转向量
        /// </summary>
        public static Vector3 RotateVectorAroundAxis(Vector3 vector, Vector3 axis, float angle)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            return rotation * vector;
        }

        /// <summary>
        /// 计算两个向量之间的夹角（0-180度）
        /// </summary>
        public static float AngleBetweenVectors(Vector3 from, Vector3 to) => Vector3.Angle(from, to);

        /// <summary>
        /// 计算带符号的两个向量之间的夹角（-180到180度）
        /// </summary>
        public static float SignedAngleBetweenVectors(Vector3 from, Vector3 to, Vector3 axis) => Vector3.SignedAngle(from, to, axis);
    }
}