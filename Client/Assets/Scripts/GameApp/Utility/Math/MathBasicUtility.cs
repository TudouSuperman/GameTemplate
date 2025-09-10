using System.Collections.Generic;
using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 数学运算工具。
    /// </summary>
    public static class MathBasicUtility
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

        /// <summary>
        /// 重新映射一个值从一個范围到另一个范围
        /// </summary>
        public static float Remap2(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
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
        /// 检查两个向量是否近似相等
        /// </summary>
        public static bool Approximately(Vector3 a, Vector3 b, float threshold = 0.001f)
        {
            return Vector3.SqrMagnitude(a - b) < threshold * threshold;
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

        // ========== 实用函数 ========== //

        /// <summary>
        /// 将向量限制在最小和最大值之间
        /// </summary>
        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Clamp(value.x, min.x, max.x),
                Mathf.Clamp(value.y, min.y, max.y),
                Mathf.Clamp(value.z, min.z, max.z)
            );
        }

        /// <summary>
        /// 计算斐波那契数列的第n项
        /// </summary>
        public static int Fibonacci(int n)
        {
            if (n <= 1) return n;
            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }
    }
}