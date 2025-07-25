using HotProcedureOwner = GameFramework.Fsm.IFsm<GameApp.Hot.Procedure.HotProcedureComponent>;

namespace GameApp.Hot.Procedure
{
    public sealed class ProcedurePreload : ProcedureBase
    {
        protected override void OnEnter(HotProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            ChangeState<ProcedureGame>(procedureOwner);
        }
    }
}