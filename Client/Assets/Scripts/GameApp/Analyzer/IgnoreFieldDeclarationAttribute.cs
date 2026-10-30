using System;

namespace GameApp
{
    /// <summary>
    /// 跳过当前字段的命名检查。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class IgnoreFieldDeclarationAttribute : Attribute
    {
    }
}
