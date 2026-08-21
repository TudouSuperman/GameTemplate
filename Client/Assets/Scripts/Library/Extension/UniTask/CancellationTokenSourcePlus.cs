namespace System.Threading
{
    /// <summary>
    /// 取消令牌源
    /// 最大限度地避免重复创建取消令牌源
    /// </summary>
    public class CancellationTokenSourcePlus
    {
        private CancellationTokenSource m_CTS;
        private Int32 m_RefCount = 0;

        /// <summary>
        /// 分配取消令牌
        /// </summary>
        public CancellationToken MallocToken()
        {
            if (this.m_CTS == null)
            {
                this.m_CTS = new CancellationTokenSource();
                this.m_RefCount = 0;
            }

            this.m_RefCount++;
            return this.m_CTS.Token;
        }

        /// <summary>
        /// 释放取消令牌
        /// </summary>
        public void FreeToken()
        {
            this.m_RefCount--;
            if (this.m_RefCount < 0)
            {
                throw new Exception($"CancellationTokenSourcePlus RefCount is less than 0! RefCount:'{this.m_RefCount}'.");
            }
        }

        /// <summary>
        /// 取消取消令牌源
        /// </summary>
        public void Cancel()
        {
            if (this.m_RefCount != 0 && this.m_CTS != null)
            {
                this.m_CTS.Cancel();
                this.m_CTS.Dispose();
                this.m_CTS = null;
            }
        }
    }
}