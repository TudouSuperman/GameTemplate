using System;

namespace GameApp.Hotfix
{
    public sealed class UserModel : IModel
    {
        private int m_Coin;
        public event Action<int> OnCoinChanged;

        public void Clear()
        {
            m_Coin = 0;
            OnCoinChanged = null;
        }

        public int FetchCoin() => m_Coin;

        public void CacheCoin(int value)
        {
            m_Coin = value;
            OnCoinChanged?.Invoke(value);
        }
    }
}