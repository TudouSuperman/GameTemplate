using System;

namespace GameApp
{
    /// <summary>
    /// 跳过当前属性的命名检查。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnorePropertyDeclarationAttribute : Attribute
    {
    }
}
