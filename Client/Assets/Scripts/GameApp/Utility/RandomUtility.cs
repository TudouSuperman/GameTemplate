using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameApp
{
    /// <summary>
    /// 随机数生成工具类，提供常用的随机数生成和随机选择功能。
    /// </summary>
    public static class RandomUtility
    {
        /// <summary>
        /// 生成浮点数 0 - 1 随机数。
        /// </summary>
        public static float Random01()
        {
            return UnityEngine.Random.value;
        }

        /// <summary>
        /// 生成整数随机数（包含最小值，不包含最大值）。
        /// </summary>
        /// <param name="min">最小值（包含）。</param>
        /// <param name="max">最大值（不包含）。</param>
        /// <returns>范围内的随机整数。</returns>
        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// 生成浮点数随机数（包含最小值和最大值）。
        /// </summary>
        /// <param name="min">最小值（包含）。</param>
        /// <param name="max">最大值（包含）。</param>
        /// <returns>范围内的随机浮点数。</returns>
        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// 百分比概率检测（整数版）。
        /// </summary>
        /// <param name="percent">触发概率值（ 0 - 100 ）。</param>
        /// <returns>若随机值小于概率值返回 true，否则 false。</returns>
        public static bool Random100(int percent)
        {
            return UnityEngine.Random.Range(0, 101) < percent;
        }

        /// <summary>
        /// 百分比概率检测（浮点数版）。
        /// </summary>
        /// <param name="percent">触发概率值（ 0.0 - 100.0 ）。</param>
        /// <returns>若随机值小于概率值返回 true，否则 false。</returns>
        public static bool Random100(float percent)
        {
            return UnityEngine.Random.Range(0f, 100f) < percent;
        }

        /// <summary>
        /// 从数组中随机选择一个元素。
        /// </summary>
        /// <typeparam name="T">数组元素类型。</typeparam>
        /// <param name="array">源数组。</param>
        /// <returns>随机选择的数组元素。</returns>
        public static T RandomArray<T>(T[] array)
        {
            if (array == null || array.Length == 0)
            {
                Log.Error("RandomArray: 数组为空或长度为零");
                return default;
            }

            return array[Random(0, array.Length)];
        }

        /// <summary>
        /// 从列表中随机选择一个元素。
        /// </summary>
        /// <typeparam name="T">列表元素类型。</typeparam>
        /// <param name="list">源列表。</param>
        /// <returns>随机选择的列表元素。</returns>
        public static T RandomList<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                Log.Error("RandomList: 列表为空或长度为零");
                return default;
            }

            return list[Random(0, list.Count)];
        }

        /// <summary>
        /// 从数组中随机选择指定数量的不重复元素。
        /// </summary>
        /// <typeparam name="T">数组元素类型。</typeparam>
        /// <param name="array">源数组。</param>
        /// <param name="count">需要选择的数量。</param>
        /// <returns>包含随机元素的数组。</returns>
        public static T[] RandomMultiple<T>(T[] array, int count)
        {
            if (array == null || array.Length == 0)
            {
                Log.Error("RandomMultiple: 数组为空或长度为零");
                return new T[0];
            }

            count = Mathf.Min(count, array.Length);
            var result = new T[count];
            var indices = new List<int>();

            for (int i = 0; i < array.Length; i++)
            {
                indices.Add(i);
            }

            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random(0, indices.Count);
                result[i] = array[indices[randomIndex]];
                indices.RemoveAt(randomIndex);
            }

            return result;
        }

        /// <summary>
        /// 根据权重数组随机选择索引。
        /// </summary>
        /// <param name="weights">权重数组。</param>
        /// <returns>被选中的权重索引。</returns>
        public static int RandomByWeights(float[] weights)
        {
            if (weights == null || weights.Length == 0)
            {
                Log.Error("RandomByWeights: 权重数组为空");
                return -1;
            }

            float total = 0f;
            foreach (float weight in weights)
            {
                total += weight;
            }

            float random = Random(0f, total);
            float current = 0f;

            for (int i = 0; i < weights.Length; i++)
            {
                current += weights[i];
                if (random < current)
                {
                    return i;
                }
            }

            return weights.Length - 1;
        }

        /// <summary>
        /// 随机打乱数组元素顺序（Fisher-Yates洗牌算法）。
        /// </summary>
        /// <typeparam name="T">数组元素类型。</typeparam>
        /// <param name="array">要打乱顺序的数组。</param>
        public static void Shuffle<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Random(0, i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }
        }

        /// <summary>
        /// 随机打乱列表元素顺序（Fisher-Yates洗牌算法）。
        /// </summary>
        /// <typeparam name="T">列表元素类型。</typeparam>
        /// <param name="list">要打乱顺序的列表。</param>
        public static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random(0, i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }

        /// <summary>
        /// 生成随机三维方向（单位向量）。
        /// </summary>
        /// <returns>随机单位方向向量。</returns>
        public static Vector3 RandomDirection()
        {
            return UnityEngine.Random.onUnitSphere;
        }

        /// <summary>
        /// 生成随机颜色。
        /// </summary>
        /// <param name="includeAlpha">是否包含随机透明度。</param>
        /// <returns>随机颜色值。</returns>
        public static Color RandomColor(bool includeAlpha = false)
        {
            return new Color(
                UnityEngine.Random.value,
                UnityEngine.Random.value,
                UnityEngine.Random.value,
                includeAlpha ? UnityEngine.Random.value : 1f
            );
        }
    }
}