using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameApp
{
    /// <summary>
    /// 数学运算工具。
    /// </summary>
    public static class MathUtility
    {
        // ========== 基础取整操作 ========== //

        /// <summary>
        /// 向上取整 (Ceiling)
        /// 获取不小于当前值的最小整数
        /// 示例：0.1 → 1, 2.0 → 2, -1.5 → -1
        /// </summary>
        public static float Ceil(float value)
        {
            return Mathf.Ceil(value);
        }

        /// <summary>
        /// 向下取整 (Floor)
        /// 获取不大于当前值的最大整数
        /// 示例：0.9 → 0, 2.0 → 2, -1.5 → -2
        /// </summary>
        public static float Floor(float value)
        {
            return Mathf.Floor(value);
        }

        /// <summary>
        /// 四舍五入 (Round)
        /// 获取最接近的整数
        /// 示例：1.4 → 1, 1.5 → 2, 2.5 → 2（Unity默认向偶数取整）
        /// </summary>
        public static float Round(float value)
        {
            return Mathf.Round(value);
        }

        /// <summary>
        /// 银行家舍入法 (Round Half To Even)
        /// 当小数部分为0.5时，向最近的偶数取整
        /// 示例：2.4 → 2, 3.5 → 4, 1.5 → 2
        /// </summary>
        public static float RoundToEven(float value)
        {
            // 使用Unity内置方法，它默认就是银行家舍入
            return Mathf.Round(value);
        }

        // ========== 进阶数值操作 ========== //

        /// <summary>
        /// 取最接近的倍数
        /// 示例：RoundToNearestMultiple(17, 5) → 15
        /// </summary>
        public static float RoundToNearestMultiple(float value, float multiple)
        {
            return Mathf.Round(value / multiple) * multiple;
        }

        /// <summary>
        /// 获取小数部分（始终为正）
        /// 示例：3.7 → 0.7, -2.3 → 0.7
        /// </summary>
        public static float FractionalPart(float value)
        {
            return value - Mathf.Floor(value);
        }

        /// <summary>
        /// 获取整数部分
        /// 示例：3.7 → 3, -2.3 → -2
        /// </summary>
        public static float IntegerPart(float value)
        {
            return (int)value;
        }

        /// <summary>
        /// 数值反转（倒数）
        /// 示例：2 → 0.5, 0.25 → 4
        /// </summary>
        public static float Inverse(float value)
        {
            return 1f / value;
        }

        // ========== 范围操作 ========== //

        /// <summary>
        /// 数值夹紧 (Clamp)
        /// 将值限制在[min, max]范围内
        /// 示例：Clamp(10, 5, 8) → 8
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// 循环包裹 (Wrap)
        /// 将值限制在[min, max)范围内循环
        /// 示例：Wrap(2.3, 0, 2) → 0.3, Wrap(-0.3, 0, 2) → 1.7
        /// </summary>
        public static float Wrap(float value, float min, float max)
        {
            float range = max - min;
            float offset = value - min;
            return offset - Mathf.Floor(offset / range) * range + min;
        }

        /// <summary>
        /// 范围归一化
        /// 将值从[min, max]映射到[0, 1]
        /// 示例：Normalize(75, 0, 100) → 0.75
        /// </summary>
        public static float Normalize(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        /// <summary>
        /// 范围重映射
        /// 将值从[oldMin, oldMax]映射到[newMin, newMax]
        /// 示例：Remap(0.5, 0, 1, 10, 20) → 15
        /// </summary>
        public static float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            float normalized = (value - oldMin) / (oldMax - oldMin);
            return Mathf.Lerp(newMin, newMax, normalized);
        }

        // ========== 浮点数精度处理 ========== //

        /// <summary>
        /// 浮点数近似相等比较
        /// 解决浮点数精度问题
        /// 示例：0.1 + 0.2 ≈ 0.3 → True
        /// </summary>
        public static bool Approximately(float a, float b, float threshold = 0.0001f)
        {
            return Mathf.Abs(a - b) < threshold;
        }

        /// <summary>
        /// 设置浮点数精度（保留指定位数小数）
        /// 示例：SetPrecision(1.23456, 3) → 1.235
        /// </summary>
        public static float SetPrecision(float value, int decimalPlaces)
        {
            float multiplier = Mathf.Pow(10, decimalPlaces);
            return Mathf.Round(value * multiplier) / multiplier;
        }

        // ========== 几何运算 ========== //

        /// <summary>
        /// 角度转弧度
        /// 示例：DegreesToRadians(180) → π ≈ 3.14159
        /// </summary>
        public static float DegreesToRadians(float degrees)
        {
            return degrees * Mathf.Deg2Rad;
        }

        /// <summary>
        /// 弧度转角度
        /// 示例：RadiansToDegrees(Mathf.PI) → 180
        /// </summary>
        public static float RadiansToDegrees(float radians)
        {
            return radians * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 二维向量点积
        /// 示例：DotProduct((1,0), (0,1)) → 0
        /// </summary>
        public static float DotProduct(Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        /// <summary>
        /// 三维向量叉积
        /// 示例：CrossProduct((1,0,0), (0,1,0)) → (0,0,1)
        /// </summary>
        public static Vector3 CrossProduct(Vector3 a, Vector3 b)
        {
            return Vector3.Cross(a, b);
        }

        /// <summary>
        /// 角度差（返回-180到180之间的角度差）
        /// 示例：AngleDifference(10, 350) → -20
        /// </summary>
        public static float AngleDifference(float a, float b)
        {
            float diff = (b - a) % 360;
            if (diff > 180) diff -= 360;
            else if (diff < -180) diff += 360;
            return diff;
        }
        // ==================== 基本数学操作 ==================== //

        /// <summary>
        /// 安全除法（避免除以零错误）
        /// 示例：SafeDivide(10, 0) → 0
        /// </summary>
        public static float SafeDivide(float numerator, float denominator, float fallback = 0f)
        {
            return Mathf.Approximately(denominator, 0) ? fallback : numerator / denominator;
        }

        /// <summary>
        /// 判断数值是否为2的幂
        /// 示例：IsPowerOfTwo(16) → true
        /// </summary>
        public static bool IsPowerOfTwo(int value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }

        /// <summary>
        /// 求最大公约数（GCD）
        /// 示例：GCD(48, 18) → 6
        /// </summary>
        public static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }

            return a;
        }

        /// <summary>
        /// 求最小公倍数（LCM）
        /// 示例：LCM(12, 18) → 36
        /// </summary>
        public static int LCM(int a, int b)
        {
            return Mathf.Abs(a * b) / GCD(a, b);
        }

        // ==================== 几何计算 ==================== //

        /// <summary>
        /// 计算两点间距离（忽略Y轴）
        /// 示例：Distance2D(new Vector3(0,0,0), new Vector3(3,0,4)) → 5
        /// </summary>
        public static float Distance2D(Vector3 a, Vector3 b)
        {
            a.y = b.y;
            return Vector3.Distance(a, b);
        }

        /// <summary>
        /// 计算点到直线的距离
        /// 示例：DistanceToLine(point, lineStart, lineEnd)
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
        /// 计算圆上点
        /// 示例：PointOnCircle(center, radius, 45) → 45度位置的点
        /// </summary>
        public static Vector3 PointOnCircle(Vector3 center, float radius, float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;
            return center + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
        }

        // ==================== 随机数生成 ==================== //

        /// <summary>
        /// 生成高斯分布随机数（均值为0，标准差为1）
        /// 示例：GaussianRandom()
        /// </summary>
        public static float GaussianRandom()
        {
            float u1 = 1.0f - Random.value;
            float u2 = 1.0f - Random.value;
            return Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        }

        /// <summary>
        /// 从列表中随机选择权重项
        /// 示例：WeightedRandom(new float[]{0.2f, 0.3f, 0.5f}) → 按权重返回索引
        /// </summary>
        public static int WeightedRandom(float[] weights)
        {
            float total = 0;
            foreach (float w in weights) total += w;

            float randomPoint = Random.value * total;

            for (int i = 0; i < weights.Length; i++)
            {
                if (randomPoint < weights[i]) return i;
                randomPoint -= weights[i];
            }

            return weights.Length - 1;
        }

        /// <summary>
        /// 生成泊松分布随机数
        /// 示例：PoissonRandom(3.5f)
        /// </summary>
        public static int PoissonRandom(float lambda)
        {
            float L = Mathf.Exp(-lambda);
            float p = 1.0f;
            int k = 0;

            do
            {
                k++;
                p *= Random.value;
            } while (p > L);

            return k - 1;
        }

        // ==================== 曲线与插值 ==================== //

        /// <summary>
        /// 平滑步长插值（SmoothStep的改进版）
        /// 示例：SmootherStep(0.5f, 0, 1) → 0.5
        /// </summary>
        public static float SmootherStep(float t, float min = 0, float max = 1)
        {
            t = Mathf.Clamp01((t - min) / (max - min));
            return t * t * t * (t * (6f * t - 15f) + 10f);
        }

        /// <summary>
        /// 线性贝塞尔曲线，根据 t 值计算两点之间的插值点。
        /// </summary>
        /// <param name="t">插值比例（ 0 - 1 之间）。</param>
        /// <param name="start">起始点坐标。</param>
        /// <param name="end">结束点坐标。</param>
        /// <returns>线性插值点坐标。</returns>
        public static Vector3 CalculateLineBezierPoint(float t, Vector3 start, Vector3 end)
        {
            float u = 1 - t;
            Vector3 p = u * start;
            p += t * end;
            return p;
        }

        /// <summary>
        /// 二次贝塞尔曲线，根据 t 值计算曲线上的点。
        /// </summary>
        /// <param name="t">曲线参数（ 0 - 1 之间）。</param>
        /// <param name="start">起始点坐标。</param>
        /// <param name="control">控制点坐标。</param>
        /// <param name="end">结束点坐标。</param>
        /// <returns>二次贝塞尔曲线上的点坐标。</returns>
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
        /// 三次贝塞尔曲线，根据 t 值计算曲线上的点。
        /// </summary>
        /// <param name="t">曲线参数（ 0 - 1 之间）。</param>
        /// <param name="start">起始点坐标。</param>
        /// <param name="control1">第一控制点坐标。</param>
        /// <param name="control2">第二控制点坐标。</param>
        /// <param name="end">结束点坐标。</param>
        /// <returns>三次贝塞尔曲线上的点坐标。</returns>
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
        /// 获取线性贝塞尔曲线的点集。
        /// </summary>
        /// <param name="startPoint">起始点坐标。</param>
        /// <param name="endPoint">结束点坐标。</param>
        /// <param name="segmentNum">要生成的曲线分段数量。</param>
        /// <returns>线性贝塞尔曲线上的点集数组。</returns>
        public static Vector3[] GetLineBezierList(Vector3 startPoint, Vector3 endPoint, int segmentNum)
        {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                Vector3 pixel = CalculateLineBezierPoint(t, startPoint, endPoint);
                path[i - 1] = pixel;
            }

            return path;
        }

        /// <summary>
        /// 获取二次贝塞尔曲线的点集。
        /// </summary>
        /// <param name="startPoint">起始点坐标。</param>
        /// <param name="controlPoint">控制点坐标。</param>
        /// <param name="endPoint">结束点坐标。</param>
        /// <param name="segmentNum">要生成的曲线分段数量。</param>
        /// <returns>二次贝塞尔曲线上的点集数组。</returns>
        public static Vector3[] GetCubicBezierList(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segmentNum)
        {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                Vector3 pixel = CalculateCubicBezierPoint(t, startPoint, controlPoint, endPoint);
                path[i - 1] = pixel;
            }

            return path;
        }

        /// <summary>
        /// 获取三次贝塞尔曲线的点集。
        /// </summary>
        /// <param name="startPoint">起始点坐标。</param>
        /// <param name="controlPoint1">第一控制点坐标。</param>
        /// <param name="controlPoint2">第二控制点坐标。</param>
        /// <param name="endPoint">结束点坐标。</param>
        /// <param name="segmentNum">要生成的曲线分段数量。</param>
        /// <returns>三次贝塞尔曲线上的点集数组。</returns>
        public static Vector3[] GetThreePowerBezierList(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint, int segmentNum)
        {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                Vector3 pixel = CalculateThreePowerBezierPoint(t, startPoint, controlPoint1, controlPoint2, endPoint);
                path[i - 1] = pixel;
            }

            return path;
        }

        /// <summary>
        /// 抛物线运动计算
        /// 示例：ParabolicMotion(start, end, height, t)
        /// </summary>
        public static Vector3 ParabolicMotion(Vector3 start, Vector3 end, float height, float t)
        {
            t = Mathf.Clamp01(t);
            float y = height * (4 * t * (1 - t));
            return Vector3.Lerp(start, end, t) + Vector3.up * y;
        }

        // ==================== 统计分析 ==================== //

        /// <summary>
        /// 计算平均值
        /// 示例：Mean(new float[]{1,2,3,4}) → 2.5
        /// </summary>
        public static float Mean(IEnumerable<float> values)
        {
            float sum = 0;
            int count = 0;

            foreach (float v in values)
            {
                sum += v;
                count++;
            }

            return count == 0 ? 0 : sum / count;
        }

        /// <summary>
        /// 计算标准差
        /// 示例：StandardDeviation(new float[]{1,2,3,4}) → 1.118
        /// </summary>
        public static float StandardDeviation(IEnumerable<float> values)
        {
            float mean = Mean(values);
            float sumSq = 0;
            int count = 0;

            foreach (float v in values)
            {
                float diff = v - mean;
                sumSq += diff * diff;
                count++;
            }

            return count == 0 ? 0 : Mathf.Sqrt(sumSq / count);
        }

        /// <summary>
        /// 计算中位数
        /// 示例：Median(new float[]{1,3,5,7,9}) → 5
        /// </summary>
        public static float Median(List<float> values)
        {
            if (values.Count == 0) return 0;

            values.Sort();
            int mid = values.Count / 2;

            if (values.Count % 2 == 0)
                return (values[mid - 1] + values[mid]) / 2f;
            else
                return values[mid];
        }

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

        // ==================== 提供网格坐标操作和空间关系计算功能 ==================== //

        /// <summary>
        /// 获取左侧相邻网格坐标
        /// </summary>
        /// <param name="vector3Int">当前网格坐标</param>
        /// <returns>左侧相邻坐标</returns>
        public static Vector3Int Left(this Vector3Int vector3Int)
        {
            return vector3Int + Vector3Int.left;
        }

        /// <summary>
        /// 获取右侧相邻网格坐标
        /// </summary>
        /// <param name="vector3Int">当前网格坐标</param>
        /// <returns>右侧相邻坐标</returns>
        public static Vector3Int Right(this Vector3Int vector3Int)
        {
            return vector3Int + Vector3Int.right;
        }

        /// <summary>
        /// 获取上方相邻网格坐标
        /// </summary>
        /// <param name="vector3Int">当前网格坐标</param>
        /// <returns>上方相邻坐标</returns>
        public static Vector3Int Up(this Vector3Int vector3Int)
        {
            return vector3Int + Vector3Int.up;
        }

        /// <summary>
        /// 获取下方相邻网格坐标
        /// </summary>
        /// <param name="vector3Int">当前网格坐标</param>
        /// <returns>下方相邻坐标</returns>
        public static Vector3Int Down(this Vector3Int vector3Int)
        {
            return vector3Int + Vector3Int.down;
        }

        /// <summary>
        /// 获取四方向相邻坐标（上、右、下、左）
        /// </summary>
        /// <param name="vector3Int">当前网格坐标</param>
        /// <returns>相邻坐标数组（顺序：左、上、右、下）</returns>
        public static Vector3Int[] GetAdjacent4(this Vector3Int vector3Int)
        {
            return new Vector3Int[4]
            {
                vector3Int.Left(), // 左侧
                vector3Int.Up(), // 上方
                vector3Int.Right(), // 右侧
                vector3Int.Down() // 下方
            };
        }

        /// <summary>
        /// 获取螺旋扩展范围内的所有网格坐标
        /// 示例（半径=1）:
        ///     2 3 4
        ///     1 0 5
        ///     8 7 6
        /// </summary>
        /// <param name="center">中心点坐标</param>
        /// <param name="radius">扩展半径</param>
        /// <returns>螺旋范围内的所有坐标列表</returns>
        public static List<Vector3Int> GetSpiralRange(this Vector3Int center, int radius)
        {
            if (radius <= 0)
                throw new ArgumentException("Radius must be greater than zero", nameof(radius));

            var result = new List<Vector3Int>();
            var current = center;

            // 起始点：中心左侧一格
            current = current.Left();
            result.Add(current);

            // 螺旋生成算法
            for (int i = 0; i < radius; i++)
            {
                // 向上移动 (2i+1) 次
                for (int j = 0; j < 2 * i + 1; j++)
                {
                    current = current.Up();
                    result.Add(current);
                }

                // 向右移动 (2i+2) 次
                for (int j = 0; j < 2 * i + 2; j++)
                {
                    current = current.Right();
                    result.Add(current);
                }

                // 向下移动 (2i+2) 次
                for (int j = 0; j < 2 * i + 2; j++)
                {
                    current = current.Down();
                    result.Add(current);
                }

                // 向左移动 (2i+2) 次
                for (int j = 0; j < 2 * i + 2; j++)
                {
                    current = current.Left();
                    result.Add(current);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取矩形边界上的网格坐标
        /// 示例（3x3）:
        ///         3   4   5   6
        ///         2   0   0   7
        ///         1  (0)  0   8
        ///         12 11  10   9
        /// </summary>
        /// <param name="center">左下角起始点</param>
        /// <param name="width">矩形宽度（格数）</param>
        /// <param name="height">矩形高度（格数）</param>
        /// <param name="border">边界厚度（默认为1）</param>
        /// <returns>矩形边界上的坐标列表</returns>
        public static List<Vector3Int> GetRectBorder(this Vector3Int center, int width, int height, int border = 1)
        {
            if (border <= 0)
                throw new ArgumentException("Border thickness must be greater than zero", nameof(border));

            var result = new List<Vector3Int>();
            var startPoint = center;

            // 计算矩形四个角
            var bottomLeft = startPoint;
            var topLeft = startPoint + new Vector3Int(0, height - 1, 0);
            var topRight = startPoint + new Vector3Int(width - 1, height - 1, 0);
            var bottomRight = startPoint + new Vector3Int(width - 1, 0, 0);

            // 左侧边界
            for (int y = 0; y < height; y++)
            {
                for (int b = 0; b < border; b++)
                {
                    result.Add(bottomLeft + new Vector3Int(-b, y, 0));
                }
            }

            // 顶部边界
            for (int x = 0; x < width; x++)
            {
                for (int b = 0; b < border; b++)
                {
                    result.Add(topLeft + new Vector3Int(x, b, 0));
                }
            }

            // 右侧边界
            for (int y = 0; y < height; y++)
            {
                for (int b = 0; b < border; b++)
                {
                    result.Add(bottomRight + new Vector3Int(b, y, 0));
                }
            }

            // 底部边界
            for (int x = 0; x < width; x++)
            {
                for (int b = 0; b < border; b++)
                {
                    result.Add(bottomLeft + new Vector3Int(x, -b, 0));
                }
            }

            return result.Distinct().ToList(); // 去除角点重复
        }

        /// <summary>
        /// 获取指定方向的射线坐标
        /// 示例（方向1，长度3）:
        ///          3
        ///          2
        ///          1
        ///    3 2 1(0)1 2 3
        ///          1
        ///          2
        ///          3
        /// </summary>
        /// <param name="center">中心点坐标</param>
        /// <param name="width">射线宽度（格数）</param>
        /// <param name="height">射线高度（格数）</param>
        /// <param name="direction">方向（1=左,2=上,3=右,4=下）</param>
        /// <param name="length">射线长度</param>
        /// <returns>射线上的坐标列表</returns>
        public static List<Vector3Int> GetDirectionRay(this Vector3Int center, int width, int height, int direction, int length)
        {
            var result = new List<Vector3Int>();

            switch (direction)
            {
                case 1: // 向左
                    for (int w = 0; w < width; w++)
                    {
                        var start = center + new Vector3Int(0, w, 0);
                        for (int l = 1; l <= length; l++)
                        {
                            result.Add(start + Vector3Int.left * l);
                        }
                    }

                    break;

                case 2: // 向上
                    for (int h = 0; h < height; h++)
                    {
                        var start = center + new Vector3Int(h, 0, 0);
                        for (int l = 1; l <= length; l++)
                        {
                            result.Add(start + Vector3Int.up * l);
                        }
                    }

                    break;

                case 3: // 向右
                    for (int w = 0; w < width; w++)
                    {
                        var start = center + new Vector3Int(0, w, 0);
                        for (int l = 1; l <= length; l++)
                        {
                            result.Add(start + Vector3Int.right * l);
                        }
                    }

                    break;

                case 4: // 向下
                    for (int h = 0; h < height; h++)
                    {
                        var start = center + new Vector3Int(h, 0, 0);
                        for (int l = 1; l <= length; l++)
                        {
                            result.Add(start + Vector3Int.down * l);
                        }
                    }

                    break;

                default:
                    throw new ArgumentException("Invalid direction. Use 1-4 for left/up/right/down", nameof(direction));
            }

            return result;
        }

        /// <summary>
        /// 获取矩形外框坐标
        /// 示例（3x2）:
        ///  2 3
        ///1     4
        ///0     5
        ///  7 6
        /// </summary>
        /// <param name="center">中心点坐标</param>
        /// <param name="width">矩形宽度</param>
        /// <param name="height">矩形高度</param>
        /// <returns>外框坐标数组</returns>
        public static Vector3Int[] GetRectOutline(this Vector3Int center, int width, int height)
        {
            if (width <= 0 || height <= 0)
                return Array.Empty<Vector3Int>();

            // 计算矩形左下角
            var bottomLeft = center - new Vector3Int(width / 2, height / 2, 0);
            var result = new List<Vector3Int>();

            // 底部边
            for (int x = 0; x < width; x++)
            {
                result.Add(bottomLeft + new Vector3Int(x, 0, 0));
            }

            // 右侧边
            for (int y = 1; y < height; y++)
            {
                result.Add(bottomLeft + new Vector3Int(width - 1, y, 0));
            }

            // 顶部边
            for (int x = width - 2; x >= 0; x--)
            {
                result.Add(bottomLeft + new Vector3Int(x, height - 1, 0));
            }

            // 左侧边
            for (int y = height - 2; y > 0; y--)
            {
                result.Add(bottomLeft + new Vector3Int(0, y, 0));
            }

            return result.ToArray();
        }

        /// <summary>
        /// 获取圆形范围内的网格坐标
        /// </summary>
        /// <param name="center">圆心坐标</param>
        /// <param name="radius">圆半径</param>
        /// <returns>圆形范围内的坐标列表</returns>
        public static List<Vector3Int> GetCircleRange(this Vector3Int center, float radius)
        {
            if (radius <= 0)
                return new List<Vector3Int> { center };

            var result = new List<Vector3Int>();
            int intRadius = Mathf.CeilToInt(radius);

            for (int x = -intRadius; x <= intRadius; x++)
            {
                for (int y = -intRadius; y <= intRadius; y++)
                {
                    var point = center + new Vector3Int(x, y, 0);
                    float distance = Vector3Int.Distance(center, point);

                    if (distance <= radius + 0.5f) // +0.5f 确保覆盖边缘格子
                    {
                        result.Add(point);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取以左下角为基准的2x2区域坐标
        /// 示例:
        ///  3  2
        ///  0  1
        /// </summary>
        /// <param name="bottomLeft">左下角坐标</param>
        /// <returns>2x2区域坐标数组</returns>
        public static Vector3Int[] Get2x2Area(this Vector3Int bottomLeft)
        {
            return new Vector3Int[4]
            {
                bottomLeft, // 左下 (0,0)
                bottomLeft + Vector3Int.right, // 右下 (1,0)
                bottomLeft + Vector3Int.up, // 左上 (0,1)
                bottomLeft + Vector3Int.one // 右上 (1,1)
            };
        }

        /// <summary>
        /// 检查目标坐标是否与当前坐标相邻（四方向）
        /// </summary>
        /// <param name="self">当前坐标</param>
        /// <param name="other">目标坐标</param>
        /// <returns>如果是相邻坐标返回true，否则false</returns>
        public static bool IsAdjacent(this Vector3Int self, Vector3Int other)
        {
            return self.GetAdjacent4().Any(p => p == other);
        }

        /// <summary>
        /// 检查目标坐标是否在矩形范围内
        /// </summary>
        /// <param name="self">范围中心</param>
        /// <param name="target">目标坐标</param>
        /// <param name="width">范围宽度</param>
        /// <param name="height">范围高度</param>
        /// <returns>如果在范围内返回true，否则false</returns>
        public static bool IsInRectangle(this Vector3Int self, Vector3Int target, int width, int height)
        {
            Vector3Int min = self - new Vector3Int(width / 2, height / 2, 0);
            Vector3Int max = min + new Vector3Int(width, height, 0);

            return target.x >= min.x && target.x < max.x &&
                   target.y >= min.y && target.y < max.y;
        }

        /// <summary>
        /// 检查目标坐标是否在圆形范围内
        /// </summary>
        /// <param name="self">圆心</param>
        /// <param name="target">目标坐标</param>
        /// <param name="radius">圆半径</param>
        /// <returns>如果在范围内返回true，否则false</returns>
        public static bool IsInCircle(this Vector3Int self, Vector3Int target, float radius)
        {
            float distance = Vector3Int.Distance(self, target);
            return distance <= radius;
        }

        /// <summary>
        /// 计算网格坐标的曼哈顿距离
        /// </summary>
        /// <param name="a">起点坐标</param>
        /// <param name="b">终点坐标</param>
        /// <returns>曼哈顿距离</returns>
        public static int ManhattanDistance(this Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        /// <summary>
        /// 获取两个坐标之间的直线路径
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <returns>路径上的坐标列表</returns>
        public static List<Vector3Int> GetLinePath(this Vector3Int start, Vector3Int end)
        {
            var path = new List<Vector3Int>();
            int dx = Mathf.Abs(end.x - start.x);
            int dy = Mathf.Abs(end.y - start.y);
            int steps = Mathf.Max(dx, dy);

            if (steps == 0)
            {
                path.Add(start);
                return path;
            }

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                int x = Mathf.RoundToInt(Mathf.Lerp(start.x, end.x, t));
                int y = Mathf.RoundToInt(Mathf.Lerp(start.y, end.y, t));
                path.Add(new Vector3Int(x, y, 0));
            }

            return path;
        }
    }
}