using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameApp
{
    public sealed class ProcedureGameHotfix : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.CodeRunner.StartRun("GameApp.Hotfix.Init");
            Log.Info("Start run GameApp.Hotfix!");
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.CodeRunner.StopRun();
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}