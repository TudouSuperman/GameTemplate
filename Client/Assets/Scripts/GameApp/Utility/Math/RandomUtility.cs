using UnityEngine;
using System.Collections.Generic;

namespace GameApp
{
    /// <summary>
    /// 随机数生成工具类
    /// </summary>
    public static class RandomUtility
    {
        /// <summary>
        /// 生成浮点数 0 - 1 随机数
        /// </summary>
        public static float Random01() => UnityEngine.Random.value;

        /// <summary>
        /// 生成整数随机数（包含最小值，不包含最大值）
        /// </summary>
        public static int Random(int min, int max) => UnityEngine.Random.Range(min, max);

        /// <summary>
        /// 生成浮点数随机数（包含最小值和最大值）
        /// </summary>
        public static float Random(float min, float max) => UnityEngine.Random.Range(min, max);

        /// <summary>
        /// 百分比概率检测（整数版）
        /// </summary>
        public static bool Random100(int percent) => UnityEngine.Random.Range(0, 101) < percent;

        /// <summary>
        /// 百分比概率检测（浮点数版）
        /// </summary>
        public static bool Random100(float percent) => UnityEngine.Random.Range(0f, 100f) < percent;

        /// <summary>
        /// 从数组中随机选择一个元素
        /// </summary>
        public static T RandomArray<T>(T[] array)
        {
            if (array == null || array.Length == 0)
            {
                Debug.LogError("RandomArray: 数组为空或长度为零");
                return default;
            }

            return array[Random(0, array.Length)];
        }

        /// <summary>
        /// 从列表中随机选择一个元素
        /// </summary>
        public static T RandomList<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                Debug.LogError("RandomList: 列表为空或长度为零");
                return default;
            }

            return list[Random(0, list.Count)];
        }

        /// <summary>
        /// 从数组中随机选择指定数量的不重复元素
        /// </summary>
        public static T[] RandomMultiple<T>(T[] array, int count)
        {
            if (array == null || array.Length == 0)
            {
                Debug.LogError("RandomMultiple: 数组为空或长度为零");
                return new T[0];
            }

            count = Mathf.Min(count, array.Length);
            var result = new T[count];
            var indices = new List<int>();

            for (int i = 0; i < array.Length; i++)
                indices.Add(i);

            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random(0, indices.Count);
                result[i] = array[indices[randomIndex]];
                indices.RemoveAt(randomIndex);
            }

            return result;
        }

        /// <summary>
        /// 根据权重数组随机选择索引
        /// </summary>
        public static int RandomByWeights(float[] weights)
        {
            if (weights == null || weights.Length == 0)
            {
                Debug.LogError("RandomByWeights: 权重数组为空");
                return -1;
            }

            float total = 0f;
            foreach (float weight in weights)
                total += weight;

            float random = Random(0f, total);
            float current = 0f;

            for (int i = 0; i < weights.Length; i++)
            {
                current += weights[i];
                if (random < current)
                    return i;
            }

            return weights.Length - 1;
        }

        /// <summary>
        /// 随机打乱数组元素顺序（Fisher-Yates洗牌算法）
        /// </summary>
        public static void Shuffle<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Random(0, i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }
        }

        /// <summary>
        /// 随机打乱列表元素顺序（Fisher-Yates洗牌算法）
        /// </summary>
        public static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random(0, i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }

        /// <summary>
        /// 生成随机三维方向（单位向量）
        /// </summary>
        public static Vector3 RandomDirection() => UnityEngine.Random.onUnitSphere;

        /// <summary>
        /// 生成随机颜色
        /// </summary>
        public static Color RandomColor(bool includeAlpha = false) => new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, includeAlpha ? UnityEngine.Random.value : 1f);

        /// <summary>
        /// 生成指定范围内的随机二维向量
        /// </summary>
        public static Vector2 RandomVector2(float min, float max) => new Vector2(Random(min, max), Random(min, max));

        /// <summary>
        /// 生成指定范围内的随机三维向量
        /// </summary>
        public static Vector3 RandomVector3(float min, float max) => new Vector3(Random(min, max), Random(min, max), Random(min, max));

        /// <summary>
        /// 在球体内生成随机点
        /// </summary>
        public static Vector3 RandomPointInSphere(Vector3 center, float radius) => center + UnityEngine.Random.insideUnitSphere * radius;

        /// <summary>
        /// 在圆盘内生成随机点
        /// </summary>
        public static Vector3 RandomPointInCircle(Vector3 center, float radius)
        {
            Vector2 circlePoint = UnityEngine.Random.insideUnitCircle * radius;
            return center + new Vector3(circlePoint.x, 0, circlePoint.y);
        }

        /// <summary>
        /// 随机符号（返回-1或1）
        /// </summary>
        public static int RandomSign() => UnityEngine.Random.value < 0.5f ? -1 : 1;

        /// <summary>
        /// 生成高斯分布随机数（均值为0，标准差为1）
        /// </summary>
        public static float GaussianRandom()
        {
            float u1 = 1.0f - UnityEngine.Random.value;
            float u2 = 1.0f - UnityEngine.Random.value;
            return Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        }

        /// <summary>
        /// 生成泊松分布随机数
        /// </summary>
        public static int PoissonRandom(float lambda)
        {
            float L = Mathf.Exp(-lambda);
            float p = 1.0f;
            int k = 0;

            do
            {
                k++;
                p *= UnityEngine.Random.value;
            } while (p > L);

            return k - 1;
        }
    }
}