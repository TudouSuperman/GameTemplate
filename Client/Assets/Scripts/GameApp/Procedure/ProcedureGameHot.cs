using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameApp.Procedure
{
    public sealed class ProcedureGameHot : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.CodeRunner.StartRun("GameApp.Hot.Init");
            Log.Info("Start run GameApp.Hot!");
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.CodeRunner.Shutdown();
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}