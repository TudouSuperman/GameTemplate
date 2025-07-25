namespace GameApp.Hot
{
    /// <summary>
    /// 热更组件优先级（数值大的优先加载）。
    /// </summary>
    public enum EHotComponentPriority : int
    {
        HotProcedureComponent = 1,
        ModelComponent = 2,
    }
}