using UnityEngine;

namespace GameApp
{
    public static class BezierUtility
    {
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
    }
}