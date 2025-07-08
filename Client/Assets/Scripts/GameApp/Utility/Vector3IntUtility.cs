using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// Vector3Int 扩展工具。
    /// 提供网格坐标操作和空间关系计算功能。
    /// </summary>
    public static class Vector3IntUtility
    {
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