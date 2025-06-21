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
            ConfirmButton.onClick.AddListener(() => { OnConfirmClicked?.Invoke(); });
            CancelButton.onClick.AddListener(() => { OnCancelClicked?.Invoke(); });
            OtherButton.onClick.AddListener(() => { OnOtherClicked?.Invoke(); });
        }
    }
}