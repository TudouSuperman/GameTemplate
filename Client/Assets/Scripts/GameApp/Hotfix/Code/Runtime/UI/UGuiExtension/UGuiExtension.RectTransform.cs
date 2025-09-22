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

        /// <summary>
        /// TODO 作者：使用案例场景头顶血条。
        /// 根据世界空间位置设置 UI 在 Canvas 上的位置。
        /// </summary>
        /// <param name="worldCamera">世界空间相机（3D）。</param>
        /// <param name="uiCanvas">目标 UI 所在的 Canvas。</param>
        /// <param name="uiTransform">目标 UI 的 RectTransform。</param>
        /// <param name="worldPosition">世界空间位置。</param>
        /// <param name="uiOffset">UI 位置的偏移量。</param>
        /// <param name="failedUIPosition">当世界空间位置在世界相机背面时的 UI 位置。</param>
        /// <returns>如果世界空间位置在世界相机正面，返回 true，否则返回 false 。</returns>
        public static bool SetUIScreenPositionByWorldPosition
        (
            Camera worldCamera,
            Canvas uiCanvas,
            RectTransform uiTransform,
            Vector3 worldPosition,
            Vector2 uiOffset = default,
            Vector2? failedUIPosition = null
        )
        {
            // 当世界坐标在相机背面时，也能将坐标映射到 Canvas 上，这是不对的，所以要剔除相机背面的位置。
            Vector3 _camToWorldPos = worldPosition - worldCamera.transform.position;
            if (Vector3.Angle(_camToWorldPos, worldCamera.transform.forward) > 90)
            {
                if (failedUIPosition != null)
                {
                    uiTransform.anchoredPosition = failedUIPosition.Value;
                }

                return false;
            }

            // 获取屏幕和 Canvas 的尺寸。
            Vector2 _canvasSize = ((RectTransform)uiCanvas.transform).sizeDelta;
            Vector2Int _screenSize = new Vector2Int(worldCamera.pixelWidth, worldCamera.pixelHeight);
            // 计算锚点的最大最小最坐标。
            float _anchorMinPosX = _canvasSize.x * uiTransform.anchorMin.x;
            float _anchorMaxPosX = _canvasSize.x * uiTransform.anchorMax.x;
            float _anchorMinPosY = _canvasSize.y * uiTransform.anchorMin.y;
            float _anchorMaxPosY = _canvasSize.y * uiTransform.anchorMax.y;
            // 计算世界空间位置映射到屏幕空间的位置。
            Vector3 _worldCamScreenPos = worldCamera.WorldToScreenPoint(worldPosition);
            // 计算屏幕空间位置映射到 UICanvas 空间的位置。
            _worldCamScreenPos.x *= _canvasSize.x / _screenSize.x;
            _worldCamScreenPos.y *= _canvasSize.y / _screenSize.y;
            // 计算 UI 在 Canvas 上的位置。
            Vector2 _uiAnchoredPos = new Vector2()
            {
                x = (_worldCamScreenPos.x - _anchorMinPosX + _worldCamScreenPos.x - _anchorMaxPosX) / 2,
                y = (_worldCamScreenPos.y - _anchorMinPosY + _worldCamScreenPos.y - _anchorMaxPosY) / 2
            };
            // 应用偏移量。
            _uiAnchoredPos += uiOffset;
            uiTransform.anchoredPosition = _uiAnchoredPos;
            return true;
        }
    }
}