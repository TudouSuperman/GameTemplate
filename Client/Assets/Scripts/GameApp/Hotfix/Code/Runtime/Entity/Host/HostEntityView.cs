using UnityEngine.InputSystem;
using CodeBind;
using GameApp.GFEntity;

namespace GameApp.Hotfix.GFEntity
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