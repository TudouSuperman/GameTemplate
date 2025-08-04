using System;
using System.Collections.Generic;
using GameFramework;

namespace UnityGameFramework.Extension
{
    public sealed class UGFStack<T> : Stack<T>, IDisposable, IReference
    {
        public static UGFStack<T> Create()
        {
            return ReferencePool.Acquire<UGFStack<T>>();
        }

        public void Dispose()
        {
            this.Clear();
            this.TrimExcess();
            ReferencePool.Release(this);
        }
    }
}