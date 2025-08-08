using HotfixProcedureOwner = GameFramework.Fsm.IFsm<GameApp.Hotfix.HotfixProcedureComponent>;

namespace GameApp.Hotfix
{
    public sealed class ProcedureLaunch : ProcedureBase
    {
        protected override void OnEnter(HotfixProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }

        protected override void OnUpdate(HotfixProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            ChangeState<ProcedurePreload>(procedureOwner);
        }
    }
}