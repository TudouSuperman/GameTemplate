using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 颜色工具类
    /// </summary>
    public static class ColorUtility
    {
        /// <summary>
        /// 颜色模式枚举
        /// </summary>
        public enum ColorMode
        {
            RGB,
            HSV
        }

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
        public static Color HSVToRGB(Vector3 hsv) => Color.HSVToRGB(hsv.x, hsv.y, hsv.z);

        /// <summary>
        /// 颜色插值（支持多种颜色模式）
        /// </summary>
        public static Color Lerp(Color a, Color b, float t, ColorMode mode = ColorMode.RGB)
        {
            switch (mode)
            {
                case ColorMode.RGB:
                    return Color.Lerp(a, b, t);
                case ColorMode.HSV:
                    Vector3 aHSV = RGBToHSV(a);
                    Vector3 bHSV = RGBToHSV(b);

                    // 处理色相环绕
                    float hueDiff = Mathf.Abs(aHSV.x - bHSV.x);
                    float h = hueDiff > 0.5f ? Mathf.Lerp(aHSV.x + 1f, bHSV.x, t) % 1f : Mathf.Lerp(aHSV.x, bHSV.x, t);

                    return HSVToRGB(new Vector3(
                        h,
                        Mathf.Lerp(aHSV.y, bHSV.y, t),
                        Mathf.Lerp(aHSV.z, bHSV.z, t)
                    ));
                default:
                    return Color.Lerp(a, b, t);
            }
        }

        /// <summary>
        /// 生成渐变色序列
        /// </summary>
        public static Color[] CreateGradient(Color start, Color end, int steps, ColorMode mode = ColorMode.RGB)
        {
            Color[] gradient = new Color[steps];
            for (int i = 0; i < steps; i++)
            {
                float t = i / (float)(steps - 1);
                gradient[i] = Lerp(start, end, t, mode);
            }

            return gradient;
        }

        /// <summary>
        /// 调整颜色亮度
        /// </summary>
        public static Color AdjustBrightness(Color color, float factor)
        {
            if (factor < 0) factor = 0;
            return new Color(color.r * factor, color.g * factor, color.b * factor, color.a);
        }

        /// <summary>
        /// 调整颜色饱和度
        /// </summary>
        public static Color AdjustSaturation(Color color, float factor)
        {
            Vector3 hsv = RGBToHSV(color);
            hsv.y = Mathf.Clamp01(hsv.y * factor);
            return HSVToRGB(hsv);
        }

        /// <summary>
        /// 调整颜色色相
        /// </summary>
        public static Color AdjustHue(Color color, float hueOffset)
        {
            Vector3 hsv = RGBToHSV(color);
            hsv.x = (hsv.x + hueOffset) % 1f;
            if (hsv.x < 0) hsv.x += 1f;
            return HSVToRGB(hsv);
        }

        /// <summary>
        /// 调整颜色透明度
        /// </summary>
        public static Color AdjustAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));
        }

        /// <summary>
        /// 生成互补色
        /// </summary>
        public static Color Complementary(Color color)
        {
            Vector3 hsv = RGBToHSV(color);
            hsv.x = (hsv.x + 0.5f) % 1f;
            return HSVToRGB(hsv);
        }

        /// <summary>
        /// 生成类似色（色相偏移）
        /// </summary>
        public static Color Analogous(Color color, float offset = 0.05f)
        {
            Vector3 hsv = RGBToHSV(color);
            hsv.x = (hsv.x + offset) % 1f;
            return HSVToRGB(hsv);
        }

        /// <summary>
        /// 生成三色组
        /// </summary>
        public static Color[] Triadic(Color color)
        {
            Vector3 hsv = RGBToHSV(color);
            return new Color[]
            {
                color,
                HSVToRGB(new Vector3((hsv.x + 0.333f) % 1f, hsv.y, hsv.z)),
                HSVToRGB(new Vector3((hsv.x + 0.667f) % 1f, hsv.y, hsv.z))
            };
        }

        /// <summary>
        /// 生成四色组
        /// </summary>
        public static Color[] Tetradic(Color color)
        {
            Vector3 hsv = RGBToHSV(color);
            return new Color[]
            {
                color,
                HSVToRGB(new Vector3((hsv.x + 0.25f) % 1f, hsv.y, hsv.z)),
                HSVToRGB(new Vector3((hsv.x + 0.5f) % 1f, hsv.y, hsv.z)),
                HSVToRGB(new Vector3((hsv.x + 0.75f) % 1f, hsv.y, hsv.z))
            };
        }

        /// <summary>
        /// 计算颜色亮度（感知亮度）
        /// </summary>
        public static float PerceivedBrightness(Color color)
        {
            return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        }

        /// <summary>
        /// 检查颜色是否偏暗
        /// </summary>
        public static bool IsDark(Color color, float threshold = 0.5f)
        {
            return PerceivedBrightness(color) < threshold;
        }

        /// <summary>
        /// 生成与背景对比度足够的文本颜色
        /// </summary>
        public static Color GetContrastColor(Color backgroundColor, float minContrast = 4.5f)
        {
            float bgBrightness = PerceivedBrightness(backgroundColor);
            return bgBrightness > 0.5f ? Color.black : Color.white;
        }

        /// <summary>
        /// 计算两个颜色之间的对比度
        /// </summary>
        public static float CalculateContrastRatio(Color color1, Color color2)
        {
            float brightness1 = PerceivedBrightness(color1) + 0.05f;
            float brightness2 = PerceivedBrightness(color2) + 0.05f;

            return brightness1 > brightness2 ? brightness1 / brightness2 : brightness2 / brightness1;
        }

        /// <summary>
        /// 检查两个颜色是否有足够的对比度
        /// </summary>
        public static bool HasSufficientContrast(Color color1, Color color2, float minContrast = 4.5f)
        {
            return CalculateContrastRatio(color1, color2) >= minContrast;
        }

        /// <summary>
        /// 生成随机颜色
        /// </summary>
        public static Color RandomColor(bool includeAlpha = false)
        {
            return new Color(
                Random.value,
                Random.value,
                Random.value,
                includeAlpha ? Random.value : 1f
            );
        }

        /// <summary>
        /// 生成随机颜色（指定色相范围）
        /// </summary>
        public static Color RandomColorInHueRange(float minHue, float maxHue, float saturation = 0.8f, float value = 0.8f)
        {
            float hue = Mathf.Lerp(minHue, maxHue, Random.value);
            return Color.HSVToRGB(hue, saturation, value);
        }

        /// <summary>
        /// 生成随机暖色调
        /// </summary>
        public static Color RandomWarmColor()
        {
            return RandomColorInHueRange(0f, 0.2f); // 红色到黄色范围
        }

        /// <summary>
        /// 生成随机冷色调
        /// </summary>
        public static Color RandomCoolColor()
        {
            return RandomColorInHueRange(0.5f, 0.7f); // 青色到蓝色范围
        }

        /// <summary>
        /// 生成随机柔和色调（低饱和度）
        /// </summary>
        public static Color RandomPastelColor()
        {
            return RandomColorInHueRange(0f, 1f, 0.3f, 0.9f);
        }

        /// <summary>
        /// 颜色转换为十六进制字符串
        /// </summary>
        public static string ToHex(Color color, bool includeAlpha = false)
        {
            if (includeAlpha)
            {
                return $"#{ToHexString(color)}";
            }
            else
            {
                return $"#{ToHexString((Color32)color).Substring(0, 6)}";
            }

            string ToHexString(Color color)
            {
                return
                    ((byte)(color.r * 255)).ToString("X2") +
                    ((byte)(color.g * 255)).ToString("X2") +
                    ((byte)(color.b * 255)).ToString("X2") +
                    ((byte)(color.a * 255)).ToString("X2");
            }
        }

        /// <summary>
        /// 十六进制字符串转换为颜色
        /// </summary>
        public static Color FromHex(string hex)
        {
            Color color;
            if (UnityEngine.ColorUtility.TryParseHtmlString(hex, out color))
            {
                return color;
            }

            return Color.white;
        }


        /// <summary>
        /// 颜色转换为灰度
        /// </summary>
        public static Color ToGrayscale(Color color)
        {
            float gray = PerceivedBrightness(color);
            return new Color(gray, gray, gray, color.a);
        }

        /// <summary>
        /// 颜色反转
        /// </summary>
        public static Color Invert(Color color)
        {
            return new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);
        }

        /// <summary>
        /// 颜色混合（多种混合模式）
        /// </summary>
        public static Color Blend(Color baseColor, Color blendColor, BlendMode mode)
        {
            switch (mode)
            {
                case BlendMode.Normal:
                    return blendColor;
                case BlendMode.Multiply:
                    return baseColor * blendColor;
                case BlendMode.Screen:
                    return Color.white - (Color.white - baseColor) * (Color.white - blendColor);
                case BlendMode.Overlay:
                    return baseColor.r < 0.5f ? 2f * baseColor * blendColor : Color.white - 2f * (Color.white - baseColor) * (Color.white - blendColor);
                case BlendMode.Add:
                    return baseColor + blendColor;
                case BlendMode.Subtract:
                    return baseColor - blendColor;
                default:
                    return blendColor;
            }
        }

        /// <summary>
        /// 颜色混合模式枚举
        /// </summary>
        public enum BlendMode
        {
            Normal,
            Multiply,
            Screen,
            Overlay,
            Add,
            Subtract
        }

        /// <summary>
        /// 生成颜色调色板（基于基础色）
        /// </summary>
        public static Color[] GeneratePalette(Color baseColor, int count = 5)
        {
            Color[] palette = new Color[count];
            Vector3 baseHSV = RGBToHSV(baseColor);

            for (int i = 0; i < count; i++)
            {
                float hue = (baseHSV.x + i * (1f / count)) % 1f;
                palette[i] = HSVToRGB(new Vector3(hue, baseHSV.y, baseHSV.z));
            }

            return palette;
        }

        /// <summary>
        /// 生成单色调色板（不同明度和饱和度）
        /// </summary>
        public static Color[] GenerateMonochromaticPalette(Color baseColor, int count = 5)
        {
            Color[] palette = new Color[count];
            Vector3 baseHSV = RGBToHSV(baseColor);

            for (int i = 0; i < count; i++)
            {
                float t = i / (float)(count - 1);
                float saturation = Mathf.Lerp(0.2f, baseHSV.y, t);
                float value = Mathf.Lerp(0.3f, 1f, t);
                palette[i] = HSVToRGB(new Vector3(baseHSV.x, saturation, value));
            }

            return palette;
        }
    }
}