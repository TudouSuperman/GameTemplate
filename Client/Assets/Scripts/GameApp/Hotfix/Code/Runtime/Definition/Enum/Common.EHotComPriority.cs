namespace GameApp.Hotfix
{
    /// <summary>
    /// 热更组件优先级（数值大的优先加载）。
    /// </summary>
    public enum EHotfixComponentPriority : int
    {
        HotfixProcedureComponent = 1,
        ModelComponent = 2,
    }
}