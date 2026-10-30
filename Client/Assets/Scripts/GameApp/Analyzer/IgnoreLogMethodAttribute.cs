using System;

namespace GameApp
{
    /// <summary>
    /// 允许当前方法直接调用 UnityEngine.Debug.Log 系列方法。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class IgnoreLogMethodAttribute : Attribute
    {
    }
}
