namespace GameApp
{
    /// <summary>
    /// 关系类型。
    /// </summary>
    public enum ERelation : byte
    {
        /// <summary>
        /// 未知。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 自己。
        /// </summary>
        Self,

        /// <summary>
        /// 友好但是排除自己。
        /// </summary>
        FriendlyExceptSelf,

        /// <summary>
        /// 友好。
        /// </summary>
        Friendly,

        /// <summary>
        /// 中立。
        /// </summary>
        Neutral,

        /// <summary>
        /// 敌对。
        /// </summary>
        Hostile,

        /// <summary>
        /// 任何。
        /// </summary>
        Any,
    }
}