using GameFramework;
using CodeBind;

namespace GameApp.UI
{
    [MonoCodeBind('-')]
    public sealed partial class DialogFormView : UGuiFormView
    {
        public event GameFrameworkAction OnConfirmClicked;
        public event GameFrameworkAction OnCancelClicked;
        public event GameFrameworkAction OnOtherClicked;

        public override void OnInit()
        {
            m_ConfirmButton.onClick.AddListener(() => { OnConfirmClicked?.Invoke(); });
            m_CancelButton.onClick.AddListener(() => { OnCancelClicked?.Invoke(); });
            m_OtherButton.onClick.AddListener(() => { OnOtherClicked?.Invoke(); });
        }

        public void ShowConfirmNode() => m_ConfirmNodeRectTransform.gameObject.SetActive(true);
        public void HideConfirmNode() => m_ConfirmNodeRectTransform.gameObject.SetActive(false);
        public void ShowCancelNode() => m_CancelNodeRectTransform.gameObject.SetActive(true);
        public void HideCancelNode() => m_CancelNodeRectTransform.gameObject.SetActive(false);
        public void ShowOtherNode() => m_OtherNodeRectTransform.gameObject.SetActive(true);
        public void HideOtherNode() => m_OtherNodeRectTransform.gameObject.SetActive(false);

        public void RefreshTitleText(string text) => m_TitleTextMeshProUGUI.text = text;
        public void RefreshMessageText(string text) => m_MessageTextMeshProUGUI.text = text;
        public void RefreshConfirmText(string text) => m_ConfirmDescribeTextMeshProUGUI.text = text;
        public void RefreshCancelText(string text) => m_CancelDescribeTextMeshProUGUI.text = text;
        public void RefreshOtherText(string text) => m_OtherDescribeTextMeshProUGUI.text = text;
    }
}