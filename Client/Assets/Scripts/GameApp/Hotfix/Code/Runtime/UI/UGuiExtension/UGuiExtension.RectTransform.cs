using UnityEngine;

namespace GameApp.Hotfix
{
    public static partial class UGuiExtension
    {
        /// <summary>
        /// 世界坐标转成屏幕位置区域来判断重叠。
        /// </summary>
        public static bool CheckCursorOverlapAreaByPoint(RectTransform cursorArea, RectTransform targetArea)
        {
            Rect RectTransToScreenPos(RectTransform self, Camera camera)
            {
                Vector3[] _corners = new Vector3[4];
                self.GetWorldCorners(_corners);
                Vector2 _v0 = RectTransformUtility.WorldToScreenPoint(camera, _corners[0]);
                Vector2 _v1 = RectTransformUtility.WorldToScreenPoint(camera, _corners[2]);
                return new Rect(_v0, _v1 - _v0);
            }

            Rect _cursorRect = RectTransToScreenPos(cursorArea, GameEntry.Camera.UICamera);
            Rect _targetRect = RectTransToScreenPos(targetArea, GameEntry.Camera.UICamera);
            return _cursorRect.Overlaps(_targetRect);
        }

        /// <summary>
        /// 获取 UI 世界坐标系下的四个角的坐标来判断它们之间的重叠。
        /// </summary>
        private static bool CheckCursorOverlapAreaByPosition(RectTransform cursorArea, RectTransform targetArea)
        {
            Vector3[] _cursorCorners = new Vector3[4];
            cursorArea.GetWorldCorners(_cursorCorners);
            Vector3[] _targetCorners = new Vector3[4];
            targetArea.GetWorldCorners(_targetCorners);
            return !(_cursorCorners[2].x < _targetCorners[0].x
                     || _cursorCorners[0].x > _targetCorners[2].x
                     || _cursorCorners[2].y < _targetCorners[0].y
                     || _cursorCorners[0].y > _targetCorners[2].y);
        }
    }
}