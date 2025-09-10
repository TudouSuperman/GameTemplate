using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 基础数学运算工具类
    /// </summary>
    public static class MathBasicUtility
    {
        // ========== 基础取整操作 ========== //

        /// <summary>
        /// 向上取整 (Ceiling)
        /// </summary>
        public static float Ceil(float value) => Mathf.Ceil(value);

        /// <summary>
        /// 向下取整 (Floor)
        /// </summary>
        public static float Floor(float value) => Mathf.Floor(value);

        /// <summary>
        /// 四舍五入 (Round)
        /// </summary>
        public static float Round(float value) => Mathf.Round(value);

        /// <summary>
        /// 银行家舍入法 (Round Half To Even)
        /// </summary>
        public static float RoundToEven(float value) => Mathf.Round(value);

        /// <summary>
        /// 取最接近的倍数
        /// </summary>
        public static float RoundToNearestMultiple(float value, float multiple) =>
            Mathf.Round(value / multiple) * multiple;

        /// <summary>
        /// 获取小数部分（始终为正）
        /// </summary>
        public static float FractionalPart(float value) => value - Mathf.Floor(value);

        /// <summary>
        /// 获取整数部分
        /// </summary>
        public static float IntegerPart(float value) => (int)value;

        /// <summary>
        /// 数值反转（倒数）
        /// </summary>
        public static float Inverse(float value) => 1f / value;

        // ========== 范围操作 ========== //

        /// <summary>
        /// 数值夹紧 (Clamp)
        /// </summary>
        public static float Clamp(float value, float min, float max) => Mathf.Clamp(value, min, max);

        /// <summary>
        /// 循环包裹 (Wrap)
        /// </summary>
        public static float Wrap(float value, float min, float max)
        {
            float range = max - min;
            float offset = value - min;
            return offset - Mathf.Floor(offset / range) * range + min;
        }

        /// <summary>
        /// 范围归一化
        /// </summary>
        public static float Normalize(float value, float min, float max) => (value - min) / (max - min);

        /// <summary>
        /// 范围重映射
        /// </summary>
        public static float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            float normalized = (value - oldMin) / (oldMax - oldMin);
            return Mathf.Lerp(newMin, newMax, normalized);
        }

        // ========== 浮点数精度处理 ========== //

        /// <summary>
        /// 浮点数近似相等比较
        /// </summary>
        public static bool Approximately(float a, float b, float threshold = 0.0001f) =>
            Mathf.Abs(a - b) < threshold;

        /// <summary>
        /// 设置浮点数精度（保留指定位数小数）
        /// </summary>
        public static float SetPrecision(float value, int decimalPlaces)
        {
            float multiplier = Mathf.Pow(10, decimalPlaces);
            return Mathf.Round(value * multiplier) / multiplier;
        }

        /// <summary>
        /// 安全除法（避免除以零错误）
        /// </summary>
        public static float SafeDivide(float numerator, float denominator, float fallback = 0f) =>
            Mathf.Approximately(denominator, 0) ? fallback : numerator / denominator;

        /// <summary>
        /// 判断数值是否为2的幂
        /// </summary>
        public static bool IsPowerOfTwo(int value) => value != 0 && (value & (value - 1)) == 0;

        /// <summary>
        /// 求最大公约数（GCD）
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
        /// </summary>
        public static int LCM(int a, int b) => Mathf.Abs(a * b) / GCD(a, b);

        /// <summary>
        /// 角度转弧度
        /// </summary>
        public static float DegreesToRadians(float degrees) => degrees * Mathf.Deg2Rad;

        /// <summary>
        /// 弧度转角度
        /// </summary>
        public static float RadiansToDegrees(float radians) => radians * Mathf.Rad2Deg;

        /// <summary>
        /// 角度差（返回-180到180之间的角度差）
        /// </summary>
        public static float AngleDifference(float a, float b)
        {
            float diff = (b - a) % 360;
            if (diff > 180) diff -= 360;
            else if (diff < -180) diff += 360;
            return diff;
        }

        /// <summary>
        /// 将角度标准化到0-360范围
        /// </summary>
        public static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0) angle += 360f;
            return angle;
        }

        /// <summary>
        /// 将角度标准化到-180到180范围
        /// </summary>
        public static float NormalizeSignedAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }

        /// <summary>
        /// 计算两个角度之间的最小差值（考虑角度环绕）
        /// </summary>
        public static float AngleDifferenceWithWrap(float a, float b)
        {
            float diff = Mathf.Abs(a - b) % 360f;
            return diff > 180f ? 360f - diff : diff;
        }
    }
}