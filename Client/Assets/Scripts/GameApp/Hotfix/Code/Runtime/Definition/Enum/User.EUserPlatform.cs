namespace GameApp.Hotfix
{
    public enum EUserPlatform : byte
    {
        /// <summary>
        /// 游客。
        /// </summary>
        Visitor = byte.MinValue,

        /// <summary>
        /// 谷歌。
        /// </summary>
        Google,

        /// <summary>
        /// 脸书。
        /// </summary>
        FaceBook,

        /// <summary>
        /// QQ。
        /// </summary>
        QQ,

        /// <summary>
        /// 微信。
        /// </summary>
        WeChat,

        /// <summary>
        /// 抖音。
        /// </summary>
        TikTok,

        /// <summary>
        /// TapTap。
        /// </summary>
        TapTap,

        /// <summary>
        /// PC Steam。
        /// </summary>
        Steam,

        /// <summary>
        /// PC WeGame。
        /// </summary>
        WeGame,

        /// <summary>
        /// 测试。
        /// </summary>
        Test = byte.MaxValue,
    }
}