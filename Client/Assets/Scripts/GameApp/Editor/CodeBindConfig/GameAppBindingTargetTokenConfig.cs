using System;
using System.Collections.Generic;
using CodeBind.Editor;

namespace GameApp.Editor
{
    public sealed class GameAppBindingTargetTokenConfig : IBindingTargetTokenConfig
    {
        public int Priority => 1;

        public IReadOnlyDictionary<string, Type> TargetTypesByToken { get; } = new Dictionary<string, Type>()
        {
            // Unity / UGUI
            { "Animation", typeof(UnityEngine.Animation) },
            { "Animator", typeof(UnityEngine.Animator) },
            { "Button", typeof(UnityEngine.UI.Button) },
            { "Canvas", typeof(UnityEngine.Canvas) },
            { "CanvasGroup", typeof(UnityEngine.CanvasGroup) },
            { "Dropdown", typeof(UnityEngine.UI.Dropdown) },
            { "GridLayoutGroup", typeof(UnityEngine.UI.GridLayoutGroup) },
            { "HorizontalLayoutGroup", typeof(UnityEngine.UI.HorizontalLayoutGroup) },
            { "Image", typeof(UnityEngine.UI.Image) },
            { "InputField", typeof(UnityEngine.UI.InputField) },
            { "Mask", typeof(UnityEngine.UI.Mask) },
            { "RawImage", typeof(UnityEngine.UI.RawImage) },
            { "RectMask2D", typeof(UnityEngine.UI.RectMask2D) },
            { "RectTransform", typeof(UnityEngine.RectTransform) },
            { "Scrollbar", typeof(UnityEngine.UI.Scrollbar) },
            { "ScrollRect", typeof(UnityEngine.UI.ScrollRect) },
            { "Slider", typeof(UnityEngine.UI.Slider) },
            { "SpriteRenderer", typeof(UnityEngine.SpriteRenderer) },
            { "Text", typeof(UnityEngine.UI.Text) },
            { "Toggle", typeof(UnityEngine.UI.Toggle) },
            { "ToggleGroup", typeof(UnityEngine.UI.ToggleGroup) },
            { "VerticalLayoutGroup", typeof(UnityEngine.UI.VerticalLayoutGroup) },
            // TMP
            { "TMPText", typeof(TMPro.TMP_Text) },
            { "TMPInputField", typeof(TMPro.TMP_InputField) },
            { "TextMeshProUGUI", typeof(TMPro.TextMeshProUGUI) },
            { "TextMeshPro", typeof(TMPro.TextMeshPro) },
            // New InputSystem
            { "PlayerInput", typeof(UnityEngine.InputSystem.PlayerInput) },
        };
    }
}
