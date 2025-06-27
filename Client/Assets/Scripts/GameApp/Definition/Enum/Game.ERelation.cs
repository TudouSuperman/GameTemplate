namespace GameApp
{
    /// <summary>
    /// 关系类型。
    /// </summary>
    public enum ERelation : byte
    {
        /// <summary>
        /// 未知的。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 友好的。
        /// </summary>
        Friendly,

        /// <summary>
        /// 中立的。
        /// </summary>
        Neutral,

        /// <summary>
        /// 敌对的。
        /// </summary>
        Hostile,
    }
}