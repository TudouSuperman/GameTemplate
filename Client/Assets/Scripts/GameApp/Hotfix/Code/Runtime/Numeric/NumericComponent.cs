using System.Collections.Generic;

namespace GameApp.Hotfix
{
    public static class NumericComponentEx
    {
        public static float GetAsFloat(this NumericComponent self, int numericType)
        {
            return (float)self.GetByKey(numericType) / 10000;
        }

        public static int GetAsInt(this NumericComponent self, int numericType)
        {
            return (int)self.GetByKey(numericType);
        }

        public static long GetAsLong(this NumericComponent self, int numericType)
        {
            return self.GetByKey(numericType);
        }

        public static void Set(this NumericComponent self, int nt, float value)
        {
            self[nt] = (long)(value * 10000);
        }

        public static void Set(this NumericComponent self, int nt, int value)
        {
            self[nt] = value;
        }

        public static void Set(this NumericComponent self, int nt, long value)
        {
            self[nt] = value;
        }

        public static void SetNoEvent(this NumericComponent self, int numericType, long value)
        {
            self.Insert(numericType, value, false);
        }

        public static void Insert(this NumericComponent self, int numericType, long value, bool isPublicEvent = true)
        {
            long oldValue = self.GetByKey(numericType);
            if (oldValue == value)
            {
                return;
            }

            self.NumericDic[numericType] = value;

            if (numericType >= NumericType.Max)
            {
                self.Update(numericType, isPublicEvent);
                return;
            }

            if (isPublicEvent)
            {
                self.Fire(new NumbericChange
                {
                    NumericType = numericType,
                    Old = oldValue,
                    New = value
                });
            }
        }

        public static long GetByKey(this NumericComponent self, int key)
        {
            long value = 0;
            self.NumericDic.TryGetValue(key, out value);
            return value;
        }

        public static void Update(this NumericComponent self, int numericType, bool isPublicEvent)
        {
            int final = (int)numericType / 10;
            int bas = final * 10 + 1;
            int add = final * 10 + 2;
            int pct = final * 10 + 3;
            int finalAdd = final * 10 + 4;
            int finalPct = final * 10 + 5;

            // 一个数值可能会多种情况影响，比如速度，加个 buff 可能增加速度绝对值 100，也有些 buff 增加 10% 速度，所以一个值可以由 5 个值进行控制其最终结果
            // 最终值 = （（（基础值 + 加成值） * 百分百） + 最终加成值） * 最终百分比
            // final = (((base + add) * (100 + pct) / 100) + finalAdd) * (100 + finalPct) / 100;
            long result = (long)(((self.GetByKey(bas) + self.GetByKey(add)) * (100 + self.GetAsFloat(pct)) / 100f + self.GetByKey(finalAdd)) * (100 + self.GetAsFloat(finalPct)) / 100f);
            self.Insert(final, result, isPublicEvent);
        }
    }

    public struct NumbericChange
    {
        public int NumericType;
        public long Old;
        public long New;
    }

    #region TODO 案例。

    // NumericComponent _numeric = new NumericComponent();
    // _numeric.Set(NumericType.SpeedBase, 100);
    // _numeric.Set(NumericType.SpeedAdd, 100);
    // _numeric.Set(NumericType.SpeedPct, 10f);
    // _numeric.Set(NumericType.SpeedFinalAdd, 100);
    // _numeric.Set(NumericType.SpeedFinalPct, 15f);
    // UnityEngine.Debug.Log($"Speed after update: {_numeric.GetAsInt(NumericType.Speed)} 结果为 368");

    #endregion

    public sealed class NumericComponent
    {
        public Dictionary<int, long> NumericDic = new Dictionary<int, long>();
        public event System.Action<NumbericChange> OnChange;

        public long this[int numericType]
        {
            get => this.GetByKey(numericType);
            set => this.Insert(numericType, value);
        }

        public void Fire(NumbericChange data) => OnChange?.Invoke(data);
    }
}