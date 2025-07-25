using GameApp.Hot.Procedure;
using GameFramework.Fsm;

namespace GameApp.Hot.Procedure
{
    public sealed class ProcedurePreload : ProcedureBase
    {
        protected override void OnEnter(IFsm<ProcedureComponent> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            ChangeState<ProcedureGame>(procedureOwner);
        }
    }
}