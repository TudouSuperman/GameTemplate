using UnityEngine;
using UnityEngine.EventSystems;

namespace GameApp.Hotfix
{
    public sealed class WindowDragCom : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerClickHandler
    {
        private RectTransform m_TargetRect;
        private Vector3 m_Offset = Vector3.zero;
        private bool m_DragFlag;

        private void Awake()
        {
            m_TargetRect = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            SetIndex();
            m_DragFlag = false;
            SetDragPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_DragFlag = true;
            SetDragPosition(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SetIndex();
        }

        private void SetDragPosition(PointerEventData eventData)
        {
            //判断是否点到 UI 图片上的时候
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_TargetRect, eventData.position, eventData.pressEventCamera, out Vector3 _mouseWorldPosition))
            {
                if (m_DragFlag)
                {
                    m_TargetRect.position = _mouseWorldPosition + m_Offset;
                }
                else
                {
                    m_Offset = m_TargetRect.position - _mouseWorldPosition;
                }
            }
        }

        private void SetIndex() => gameObject.transform.SetAsLastSibling();
    }
}