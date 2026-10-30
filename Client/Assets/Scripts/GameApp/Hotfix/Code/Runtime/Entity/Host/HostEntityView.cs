using UnityEngine.InputSystem;
using CodeBind;

namespace GameApp.Hotfix
{
    [MonoBehaviourBinding('-')]
    public sealed partial class HostEntityView : UGFEntityView
    {
        public override void OnInit()
        {
        }

        public PlayerInput GetControlCom() => m_ControlNodePlayerInput;
    }
}