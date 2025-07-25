using HotProcedureOwner = GameFramework.Fsm.IFsm<GameApp.Hot.Procedure.HotProcedureComponent>;

namespace GameApp.Hot.Procedure
{
    public sealed class ProcedureLaunch : ProcedureBase
    {
        protected override void OnEnter(HotProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }

        protected override void OnUpdate(HotProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            ChangeState<ProcedurePreload>(procedureOwner);
        }
    }
}