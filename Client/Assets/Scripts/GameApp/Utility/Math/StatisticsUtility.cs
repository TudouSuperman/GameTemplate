using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 统计分析工具类
    /// </summary>
    public static class StatisticsUtility
    {
        /// <summary>
        /// 计算平均值
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

        /// <summary>
        /// 计算众数（出现频率最高的值）
        /// </summary>
        public static float Mode(IEnumerable<float> values)
        {
            var frequency = new Dictionary<float, int>();

            foreach (float value in values)
            {
                if (frequency.ContainsKey(value))
                    frequency[value]++;
                else
                    frequency[value] = 1;
            }

            return frequency.OrderByDescending(pair => pair.Value).FirstOrDefault().Key;
        }

        /// <summary>
        /// 计算范围（最小值和最大值）
        /// </summary>
        public static Vector2 Range(IEnumerable<float> values)
        {
            if (!values.Any()) return Vector2.zero;

            float min = float.MaxValue;
            float max = float.MinValue;

            foreach (float value in values)
            {
                if (value < min) min = value;
                if (value > max) max = value;
            }

            return new Vector2(min, max);
        }

        /// <summary>
        /// 计算百分位数
        /// </summary>
        public static float Percentile(List<float> values, float percentile)
        {
            if (values.Count == 0) return 0;

            values.Sort();
            int index = Mathf.RoundToInt(values.Count * percentile / 100f);
            index = Mathf.Clamp(index, 0, values.Count - 1);

            return values[index];
        }
    }
}