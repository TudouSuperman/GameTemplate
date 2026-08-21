using System;
using System.Diagnostics;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// 自动重置的 UniTask 完成源。
    /// 扩展 完成、取消、异常 回调。
    /// </summary>
    public class AutoResetUniTaskCompletionSourcePlus : IUniTaskSource, ITaskPoolNode<AutoResetUniTaskCompletionSourcePlus>, IPromise
    {
        public ref AutoResetUniTaskCompletionSourcePlus NextNode => ref m_NextNode;

        private static TaskPool<AutoResetUniTaskCompletionSourcePlus> s_Pool;
        private AutoResetUniTaskCompletionSourcePlus m_NextNode;
        private event Action m_OnExceptionAction;
        private event Action m_OnCancelAction;
        private event Action m_OnResultAction;
        private UniTaskCompletionSourceCore<AsyncUnit> m_Core;
        private Int16 m_Version;

        static AutoResetUniTaskCompletionSourcePlus()
        {
            TaskPool.RegisterSizeGetter(typeof(AutoResetUniTaskCompletionSourcePlus), () => s_Pool.Size);
        }

        AutoResetUniTaskCompletionSourcePlus()
        {
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus Create()
        {
            if (!s_Pool.TryPop(out AutoResetUniTaskCompletionSourcePlus result))
            {
                result = new AutoResetUniTaskCompletionSourcePlus();
            }

            result.m_Version = result.m_Core.Version;
            TaskTracker.TrackActiveTask(result, 2);
            return result;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus CreateFromCanceled(CancellationToken cancellationToken, out Int16 token)
        {
            AutoResetUniTaskCompletionSourcePlus source = Create();
            source.TrySetCanceled(cancellationToken);
            token = source.m_Core.Version;
            return source;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus CreateFromException(Exception exception, out Int16 token)
        {
            AutoResetUniTaskCompletionSourcePlus source = Create();
            source.TrySetException(exception);
            token = source.m_Core.Version;
            return source;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus CreateCompleted(out Int16 token)
        {
            AutoResetUniTaskCompletionSourcePlus source = Create();
            source.TrySetResult();
            token = source.m_Core.Version;
            return source;
        }

        public void AddOnCancelAction(Action action)
        {
            m_OnCancelAction += action;
        }

        public void AddOnExceptionAction(Action action)
        {
            m_OnExceptionAction += action;
        }

        public void AddOnResultAction(Action action)
        {
            m_OnResultAction += action;
        }

        public void RemoveOnCancelAction(Action action)
        {
            m_OnCancelAction -= action;
        }

        public void RemoveOnExceptionAction(Action action)
        {
            m_OnExceptionAction -= action;
        }

        public void RemoveOnResultAction(Action action)
        {
            m_OnResultAction -= action;
        }

        public UniTask Task
        {
            [DebuggerHidden]
            get
            {
                return new UniTask(this, m_Core.Version);
            }
        }

        [DebuggerHidden]
        public Boolean TrySetResult()
        {
            if (m_Version == m_Core.Version && m_Core.TrySetResult(AsyncUnit.Default))
            {
                if (m_OnResultAction != null)
                {
                    m_OnResultAction.Invoke();
                    m_OnResultAction = null;
                }

                return true;
            }

            return false;
        }

        [DebuggerHidden]
        public Boolean TrySetCanceled(CancellationToken cancellationToken = default)
        {
            if (m_Version == m_Core.Version && m_Core.TrySetCanceled(cancellationToken))
            {
                if (m_OnCancelAction != null)
                {
                    m_OnCancelAction.Invoke();
                    m_OnCancelAction = null;
                }

                return true;
            }

            return false;
        }

        [DebuggerHidden]
        public Boolean TrySetException(Exception exception)
        {
            if (m_Version == m_Core.Version && m_Core.TrySetException(exception))
            {
                if (m_OnExceptionAction != null)
                {
                    m_OnExceptionAction.Invoke();
                    m_OnExceptionAction = null;
                }

                return true;
            }

            return false;
        }

        [DebuggerHidden]
        public void GetResult(Int16 token)
        {
            try
            {
                m_Core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        [DebuggerHidden]
        public UniTaskStatus GetStatus(Int16 token)
        {
            return m_Core.GetStatus(token);
        }

        [DebuggerHidden]
        public UniTaskStatus UnsafeGetStatus()
        {
            return m_Core.UnsafeGetStatus();
        }

        [DebuggerHidden]
        public void OnCompleted(Action<object> continuation, object state, Int16 token)
        {
            m_Core.OnCompleted(continuation, state, token);
        }

        [DebuggerHidden]
        Boolean TryReturn()
        {
            m_OnExceptionAction = null;
            m_OnCancelAction = null;
            m_OnResultAction = null;
            TaskTracker.RemoveTracking(this);
            m_Core.Reset();
            return s_Pool.TryPush(this);
        }
    }

    public class AutoResetUniTaskCompletionSourcePlus<T> : IUniTaskSource<T>, ITaskPoolNode<AutoResetUniTaskCompletionSourcePlus<T>>, IPromise<T>
    {
        public ref AutoResetUniTaskCompletionSourcePlus<T> NextNode => ref m_NextNode;
        
        private static TaskPool<AutoResetUniTaskCompletionSourcePlus<T>> s_Pool;
        private AutoResetUniTaskCompletionSourcePlus<T> m_NextNode;
        private event Action m_OnExceptionAction;
        private event Action m_OnCancelAction;
        private event Action m_OnResultAction;
        private UniTaskCompletionSourceCore<T> m_Core;
        private Int16 m_Version;

        static AutoResetUniTaskCompletionSourcePlus()
        {
            TaskPool.RegisterSizeGetter(typeof(AutoResetUniTaskCompletionSourcePlus<T>), () => s_Pool.Size);
        }
        
        AutoResetUniTaskCompletionSourcePlus()
        {
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus<T> Create()
        {
            if (!s_Pool.TryPop(out AutoResetUniTaskCompletionSourcePlus<T> result))
            {
                result = new AutoResetUniTaskCompletionSourcePlus<T>();
            }

            result.m_Version = result.m_Core.Version;
            TaskTracker.TrackActiveTask(result, 2);
            return result;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus<T> CreateFromCanceled(CancellationToken cancellationToken, out Int16 token)
        {
            AutoResetUniTaskCompletionSourcePlus<T> source = Create();
            source.TrySetCanceled(cancellationToken);
            token = source.m_Core.Version;
            return source;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus<T> CreateFromException(Exception exception, out Int16 token)
        {
            AutoResetUniTaskCompletionSourcePlus<T> source = Create();
            source.TrySetException(exception);
            token = source.m_Core.Version;
            return source;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus<T> CreateFromResult(T result, out Int16 token)
        {
            AutoResetUniTaskCompletionSourcePlus<T> source = Create();
            source.TrySetResult(result);
            token = source.m_Core.Version;
            return source;
        }

        public void AddOnCancelAction(Action action)
        {
            m_OnCancelAction += action;
        }

        public void AddOnExceptionAction(Action action)
        {
            m_OnExceptionAction += action;
        }

        public void AddOnResultAction(Action action)
        {
            m_OnResultAction += action;
        }

        public void RemoveOnCancelAction(Action action)
        {
            m_OnCancelAction -= action;
        }

        public void RemoveOnExceptionAction(Action action)
        {
            m_OnExceptionAction -= action;
        }

        public void RemoveOnResultAction(Action action)
        {
            m_OnResultAction -= action;
        }

        public UniTask<T> Task
        {
            [DebuggerHidden]
            get
            {
                return new UniTask<T>(this, m_Core.Version);
            }
        }

        [DebuggerHidden]
        public Boolean TrySetResult(T result)
        {
            if (m_Version == m_Core.Version && m_Core.TrySetResult(result))
            {
                if (m_OnResultAction != null)
                {
                    m_OnResultAction.Invoke();
                    m_OnResultAction = null;
                }

                return true;
            }

            return false;
        }

        [DebuggerHidden]
        public Boolean TrySetCanceled(CancellationToken cancellationToken = default)
        {
            if (m_Version == m_Core.Version && m_Core.TrySetCanceled(cancellationToken))
            {
                if (m_OnCancelAction != null)
                {
                    m_OnCancelAction.Invoke();
                    m_OnCancelAction = null;
                }

                return true;
            }

            return false;
        }

        [DebuggerHidden]
        public Boolean TrySetException(Exception exception)
        {
            if (m_Version == m_Core.Version && m_Core.TrySetException(exception))
            {
                if (m_OnExceptionAction != null)
                {
                    m_OnExceptionAction.Invoke();
                    m_OnExceptionAction = null;
                }

                return true;
            }

            return false;
        }

        [DebuggerHidden]
        public T GetResult(Int16 token)
        {
            try
            {
                return m_Core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        [DebuggerHidden]
        void IUniTaskSource.GetResult(Int16 token)
        {
            GetResult(token);
        }

        [DebuggerHidden]
        public UniTaskStatus GetStatus(Int16 token)
        {
            return m_Core.GetStatus(token);
        }

        [DebuggerHidden]
        public UniTaskStatus UnsafeGetStatus()
        {
            return m_Core.UnsafeGetStatus();
        }

        [DebuggerHidden]
        public void OnCompleted(Action<object> continuation, object state, Int16 token)
        {
            m_Core.OnCompleted(continuation, state, token);
        }

        [DebuggerHidden]
        Boolean TryReturn()
        {
            m_OnExceptionAction = null;
            m_OnCancelAction = null;
            m_OnResultAction = null;
            TaskTracker.RemoveTracking(this);
            m_Core.Reset();
            return s_Pool.TryPush(this);
        }
    }
}