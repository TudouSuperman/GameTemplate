using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 网格与坐标工具类
    /// </summary>
    public static class GridUtility
    {
        /// <summary>
        /// 获取左侧相邻网格坐标
        /// </summary>
        public static Vector3Int Left(this Vector3Int vector3Int) => vector3Int + Vector3Int.left;

        /// <summary>
        /// 获取右侧相邻网格坐标
        /// </summary>
        public static Vector3Int Right(this Vector3Int vector3Int) => vector3Int + Vector3Int.right;

        /// <summary>
        /// 获取上方相邻网格坐标
        /// </summary>
        public static Vector3Int Up(this Vector3Int vector3Int) => vector3Int + Vector3Int.up;

        /// <summary>
        /// 获取下方相邻网格坐标
        /// </summary>
        public static Vector3Int Down(this Vector3Int vector3Int) => vector3Int + Vector3Int.down;

        /// <summary>
        /// 获取四方向相邻坐标（左、上、右、下）
        /// </summary>
        public static Vector3Int[] GetAdjacent4(this Vector3Int vector3Int) => new Vector3Int[4]
        {
            vector3Int.Left(),
            vector3Int.Up(),
            vector3Int.Right(),
            vector3Int.Down()
        };

        /// <summary>
        /// 获取八方向相邻坐标（包括对角线）
        /// </summary>
        public static Vector3Int[] GetAdjacent8(this Vector3Int vector3Int)
        {
            return new Vector3Int[8]
            {
                vector3Int.Left(),
                vector3Int.Left() + Vector3Int.up,
                vector3Int.Up(),
                vector3Int.Right() + Vector3Int.up,
                vector3Int.Right(),
                vector3Int.Right() + Vector3Int.down,
                vector3Int.Down(),
                vector3Int.Left() + Vector3Int.down
            };
        }

        /// <summary>
        /// 获取螺旋扩展范围内的所有网格坐标
        /// </summary>
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
        /// </summary>
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
        /// </summary>
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
        /// </summary>
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
        /// </summary>
        public static Vector3Int[] Get2x2Area(this Vector3Int bottomLeft) => new Vector3Int[4]
        {
            bottomLeft, // 左下 (0,0)
            bottomLeft + Vector3Int.right, // 右下 (1,0)
            bottomLeft + Vector3Int.up, // 左上 (0,1)
            bottomLeft + Vector3Int.one // 右上 (1,1)
        };

        /// <summary>
        /// 检查目标坐标是否与当前坐标相邻（四方向）
        /// </summary>
        public static bool IsAdjacent(this Vector3Int self, Vector3Int other) => self.GetAdjacent4().Any(p => p == other);

        /// <summary>
        /// 检查目标坐标是否在矩形范围内
        /// </summary>
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
        public static bool IsInCircle(this Vector3Int self, Vector3Int target, float radius)
        {
            float distance = Vector3Int.Distance(self, target);
            return distance <= radius;
        }

        /// <summary>
        /// 计算网格坐标的曼哈顿距离
        /// </summary>
        public static int ManhattanDistance(this Vector3Int a, Vector3Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

        /// <summary>
        /// 计算网格坐标的切比雪夫距离
        /// </summary>
        public static int ChebyshevDistance(this Vector3Int a, Vector3Int b) => Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));

        /// <summary>
        /// 获取两个坐标之间的直线路径
        /// </summary>
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

        /// <summary>
        /// 获取两个坐标之间的 Bresenham 直线路径
        /// </summary>
        public static List<Vector3Int> GetBresenhamLine(this Vector3Int start, Vector3Int end)
        {
            var points = new List<Vector3Int>();
            int x0 = start.x, y0 = start.y;
            int x1 = end.x, y1 = end.y;

            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = (x0 < x1) ? 1 : -1;
            int sy = (y0 < y1) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                points.Add(new Vector3Int(x0, y0, 0));

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return points;
        }

        /// <summary>
        /// 屏幕坐标转 UI 坐标
        /// </summary>
        public static Vector2 ScreenToUIPosition(Vector2 screenPosition, Canvas canvas)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPosition,
                canvas.worldCamera,
                out Vector2 localPos
            );
            return localPos;
        }

        /// <summary>
        /// UI 坐标转屏幕坐标
        /// </summary>
        public static Vector2 UIToScreenPosition(Vector2 uiPosition, Canvas canvas)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                return screenCenter + uiPosition;
            }
            else
            {
                return RectTransformUtility.WorldToScreenPoint(
                    canvas.worldCamera,
                    canvas.transform.TransformPoint(uiPosition)
                );
            }
        }

        /// <summary>
        /// 世界坐标转画布坐标
        /// </summary>
        public static Vector2 WorldToCanvasPosition(Vector3 worldPosition, Camera worldCamera, Canvas canvas)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPosition);
            return ScreenToUIPosition(screenPoint, canvas);
        }

        /// <summary>
        /// 画布坐标转世界坐标
        /// </summary>
        public static Vector3 CanvasToWorldPosition(Vector2 canvasPosition, Camera worldCamera, Canvas canvas)
        {
            Vector2 screenPoint = UIToScreenPosition(canvasPosition, canvas);
            Ray ray = worldCamera.ScreenPointToRay(screenPoint);
            Plane plane = new Plane(Vector3.forward, 0);
            float distance;
            plane.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }

        /// <summary>
        /// 检查点是否在屏幕范围内
        /// </summary>
        public static bool IsInScreenView(Vector3 worldPosition, Camera camera, float margin = 0.1f)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(worldPosition);
            return viewportPoint.x >= -margin && viewportPoint.x <= 1 + margin &&
                   viewportPoint.y >= -margin && viewportPoint.y <= 1 + margin &&
                   viewportPoint.z > 0;
        }

        /// <summary>
        /// 获取屏幕边界的世界坐标
        /// </summary>
        public static Bounds GetScreenWorldBounds(Camera camera, float distance = 10f)
        {
            Vector3 bottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, distance));
            Vector3 topRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, distance));
            return new Bounds((bottomLeft + topRight) / 2f, topRight - bottomLeft);
        }
    }
}