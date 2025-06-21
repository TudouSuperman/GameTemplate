using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityGameFramework.Runtime;

namespace GameApp.Procedure
{
    public sealed class ProcedureInitGame : ProcedureBase
    {
        public override bool UseNativeDialog => false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            InitPool();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.GameScene"));
            ChangeState<ProcedureChangeScene>(procedureOwner);
        }

        private void InitPool()
        {
            GameEntry.ObjectPool.CreateSingleSpawnObjectPool<CommonCacheObject>(nameof(CommonCacheObject), 30, 5, 30, 1);
        }
    }
}