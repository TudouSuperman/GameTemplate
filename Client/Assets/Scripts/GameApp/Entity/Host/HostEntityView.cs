using UnityEngine.InputSystem;
using CodeBind;

namespace GameApp.Entity
{
    [MonoCodeBind('-')]
    public sealed partial class HostEntityView : BaseEntityView
    {
        public override void OnInit()
        {
        }

        public PlayerInput GetControlCom() => m_ControlNodePlayerInput;
    }
}