/*USAGE:
Simply place the script on the ScrollRect that contains the selectable children you will be scrolling
*/

using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("UI/Extensions/UIScrollToSelection")]
    public class UIScrollToSelectionCom : MonoBehaviour
    {
        #region MEMBERS

        [Header("[ References ]")]
        [SerializeField, Tooltip("View (boundaries/mask) rect transform. Used to check if automatic scroll to selection is required.")]
        private RectTransform viewportRectTransform;

        [SerializeField, Tooltip("Scroll rect used to reach selected element.")]
        private ScrollRect targetScrollRect;

        [Header("[ Scrolling ]")]
        [SerializeField, Tooltip("Allow automatic scrolling only on these axes.")]
        private Axis scrollAxes = Axis.ANY;

        [SerializeField, Tooltip("MOVE_TOWARDS: stiff movement, LERP: smoothed out movement")]
        private ScrollMethod usedScrollMethod = ScrollMethod.MOVE_TOWARDS;

        [SerializeField]
        private float scrollSpeed = 50;

        [Space(5)]
        [SerializeField, Tooltip("Scroll speed used when element to select is out of \"JumpOffsetThreshold\" range")]
        private float endOfListJumpScrollSpeed = 150;

        [SerializeField, Range(0, 1), Tooltip("If next element to scroll to is located over this screen percentage, use \"EndOfListJumpScrollSpeed\" to reach this element faster.")]
        private float jumpOffsetThreshold = 1;

        [Header("[ Input ]")]
        [SerializeField]
        private MouseButton cancelScrollMouseButtons = MouseButton.ANY;

        [SerializeField]
        private KeyCode[] cancelScrollKeys = new KeyCode[0];

        // INTERNAL - MEMBERS ONLY
        private Vector3[] viewRectCorners = new Vector3[4];
        private Vector3[] selectedElementCorners = new Vector3[4];
        private Camera canvasCamera; // 新增：存储Canvas使用的摄像机
        private CanvasScaler canvasScaler; // 新增：处理缩放因子

        #endregion

        #region PROPERTIES

        // REFERENCES
        public RectTransform ViewRectTransform
        {
            get { return viewportRectTransform; }
            set { viewportRectTransform = value; }
        }

        public ScrollRect TargetScrollRect
        {
            get { return targetScrollRect; }
            set { targetScrollRect = value; }
        }

        // SCROLLING
        public Axis ScrollAxes => scrollAxes;
        public ScrollMethod UsedScrollMethod => usedScrollMethod;
        public float ScrollSpeed => scrollSpeed;
        public float EndOfListJumpScrollSpeed => endOfListJumpScrollSpeed;
        public float JumpOffsetThreshold => jumpOffsetThreshold;

        // INPUT
        public MouseButton CancelScrollMouseButtons => cancelScrollMouseButtons;
        public KeyCode[] CancelScrollKeys => cancelScrollKeys;

        // VARIABLES
        private RectTransform scrollRectContentTransform;
        private GameObject lastCheckedSelection;

        // COROUTINES
        private Coroutine animationCoroutine;

        #endregion

        #region FUNCTIONS

        protected void Awake()
        {
            ValidateReferences();
            // 新增：获取Canvas的摄像机和缩放组件
            Canvas canvas = GetComponentInParent<Canvas>();
            canvasCamera = canvas.worldCamera;
            canvasScaler = canvas.GetComponent<CanvasScaler>();
        }

        protected void LateUpdate()
        {
            TryToScrollToSelection();
        }

        protected void Reset()
        {
            TargetScrollRect = gameObject.GetComponentInParent<ScrollRect>() ?? gameObject.GetComponentInChildren<ScrollRect>();
            ViewRectTransform = gameObject.GetComponent<RectTransform>();
        }

        private void ValidateReferences()
        {
            if (!targetScrollRect)
            {
                targetScrollRect = GetComponent<ScrollRect>();
            }

            if (!targetScrollRect)
            {
                Debug.LogError("[UIScrollToSelection] No ScrollRect found. Either attach this script to a ScrollRect or assign on in the 'Target Scroll Rect' property");
                gameObject.SetActive(false);
                return;
            }

            if (ViewRectTransform == null)
            {
                ViewRectTransform = TargetScrollRect.GetComponent<RectTransform>();
            }

            if (TargetScrollRect != null)
            {
                scrollRectContentTransform = TargetScrollRect.content;
            }

            if (EventSystem.current == null)
            {
                Debug.LogError("[UIScrollToSelection] Unity UI EventSystem not found. It is required to check current selected object.");
                gameObject.SetActive(false);
                return;
            }
        }

        private void TryToScrollToSelection()
        {
            // 更新选中对象
            GameObject selection = EventSystem.current.currentSelectedGameObject;

            if (selection == null || selection.activeInHierarchy == false || selection == lastCheckedSelection ||
                selection.transform.IsChildOf(transform) == false)
            {
                return;
            }

            RectTransform selectionRect = selection.GetComponent<RectTransform>();

            // 获取世界坐标并转换为屏幕坐标
            ViewRectTransform.GetWorldCorners(viewRectCorners);
            selectionRect.GetWorldCorners(selectedElementCorners);

            // ==== 关键修改1：坐标转换 ====
            for (int i = 0; i < 4; i++)
            {
                viewRectCorners[i] = RectTransformUtility.WorldToScreenPoint(canvasCamera, viewRectCorners[i]);
                selectedElementCorners[i] = RectTransformUtility.WorldToScreenPoint(canvasCamera, selectedElementCorners[i]);
            }

            ScrollToSelection(selection);

            lastCheckedSelection = selection;
        }

        private void ScrollToSelection(GameObject selection)
        {
            if (selection == null) return;

            Vector3[] viewCorners = new Vector3[4];
            ViewRectTransform.GetWorldCorners(viewCorners);

            Vector3[] selectionCorners = new Vector3[4];
            selection.GetComponent<RectTransform>().GetWorldCorners(selectionCorners);

            // 把 world corners 转换到 content 的 local space
            for (int i = 0; i < 4; i++)
            {
                viewCorners[i] = scrollRectContentTransform.InverseTransformPoint(viewCorners[i]);
                selectionCorners[i] = scrollRectContentTransform.InverseTransformPoint(selectionCorners[i]);
            }

            // 计算偏移
            Vector2 offsetToSelection = Vector2.zero;
            offsetToSelection.x =
                (selectionCorners[0].x < viewCorners[0].x ? selectionCorners[0].x - viewCorners[0].x : 0) +
                (selectionCorners[2].x > viewCorners[2].x ? selectionCorners[2].x - viewCorners[2].x : 0);
            offsetToSelection.y =
                (selectionCorners[0].y < viewCorners[0].y ? selectionCorners[0].y - viewCorners[0].y : 0) +
                (selectionCorners[1].y > viewCorners[1].y ? selectionCorners[1].y - viewCorners[1].y : 0);

            // scaleFactor 在这里其实不再需要除
            // offsetToSelection /= scaleFactor; // 可删

            float finalScrollSpeed = scrollSpeed;
            if (Mathf.Abs(offsetToSelection.x) / ViewRectTransform.rect.width >= jumpOffsetThreshold ||
                Mathf.Abs(offsetToSelection.y) / ViewRectTransform.rect.height >= jumpOffsetThreshold)
            {
                finalScrollSpeed = endOfListJumpScrollSpeed;
            }

            Vector2 targetPosition = (Vector2)scrollRectContentTransform.localPosition - offsetToSelection;

            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(ScrollToPosition(targetPosition, finalScrollSpeed));
        }


        private IEnumerator ScrollToPosition(Vector2 targetPosition, float speed)
        {
            Vector3 startPosition = scrollRectContentTransform.localPosition;

            // 限制滚动轴
            targetPosition.x = ((scrollAxes | Axis.HORIZONTAL) == scrollAxes) ? targetPosition.x : startPosition.x;
            targetPosition.y = ((scrollAxes | Axis.VERTICAL) == scrollAxes) ? targetPosition.y : startPosition.y;

            // ==== 关键修改3：基于Viewport尺寸计算速度 ====
            Rect viewRect = ViewRectTransform.rect;
            float horizontalSpeed = (viewRect.width / Screen.dpi) * speed;
            float verticalSpeed = (viewRect.height / Screen.dpi) * speed;
            Vector2 currentPosition2D = startPosition;
            while (currentPosition2D != targetPosition && !CheckIfScrollInterrupted())
            {
                currentPosition2D.x = MoveTowardsValue(currentPosition2D.x, targetPosition.x, horizontalSpeed, usedScrollMethod);
                currentPosition2D.y = MoveTowardsValue(currentPosition2D.y, targetPosition.y, verticalSpeed, usedScrollMethod);

                scrollRectContentTransform.localPosition = currentPosition2D;
                yield return null;
            }

            scrollRectContentTransform.localPosition = currentPosition2D;
        }

        private bool CheckIfScrollInterrupted()
        {
            bool mouseButtonClicked = false;

            // check mouse buttons
            switch (CancelScrollMouseButtons)
            {
                case MouseButton.LEFT:
                    mouseButtonClicked |= Input.GetMouseButtonDown(0);
                    break;
                case MouseButton.RIGHT:
                    mouseButtonClicked |= Input.GetMouseButtonDown(1);
                    break;
                case MouseButton.MIDDLE:
                    mouseButtonClicked |= Input.GetMouseButtonDown(2);
                    break;
            }

            if (mouseButtonClicked == true)
            {
                return true;
            }

            // check keyboard buttons
            for (int i = 0; i < CancelScrollKeys.Length; i++)
            {
                if (Input.GetKeyDown(CancelScrollKeys[i]) == true)
                {
                    return true;
                }
            }

            return false;
        }

        private float MoveTowardsValue(float from, float to, float delta, ScrollMethod method)
        {
            switch (method)
            {
                case ScrollMethod.MOVE_TOWARDS:
                    return Mathf.MoveTowards(from, to, delta * Time.unscaledDeltaTime);
                case ScrollMethod.LERP:
                    return Mathf.Lerp(from, to, delta * Time.unscaledDeltaTime);
                default:
                    return from;
            }
        }

        #endregion

        #region CLASS_ENUMS

        [Flags]
        public enum Axis
        {
            NONE = 0x00000000,
            HORIZONTAL = 0x00000001,
            VERTICAL = 0x00000010,
            ANY = 0x00000011
        }

        [Flags]
        public enum MouseButton
        {
            NONE = 0x00000000,
            LEFT = 0x00000001,
            RIGHT = 0x00000010,
            MIDDLE = 0x00000100,
            ANY = 0x00000111
        }

        public enum ScrollMethod
        {
            MOVE_TOWARDS,
            LERP
        }

        #endregion
    }
}