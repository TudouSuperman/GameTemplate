using System;

namespace GameApp.Editor
{
    public sealed partial class DataTableProcessor
    {
        public interface ICollectionProcessor
        {
            Type ItemType { get; }

            string ItemLanguageKeyword { get; }
        }
    }
}