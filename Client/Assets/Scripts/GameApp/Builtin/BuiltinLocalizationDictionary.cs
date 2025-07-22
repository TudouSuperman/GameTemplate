using System.Collections.Generic;
using GameFramework.Localization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameApp
{
    [CreateAssetMenu(menuName = "GameApp/Localization Dictionary", fileName = "UGFLocalizationDictionary", order = 0)]
    public class BuiltinLocalizationDictionary : SerializedScriptableObject
    {
        [SerializeField] private Language m_Language;
        [SerializeField] private Dictionary<string, string> m_Dictionary;

        public Language Language => m_Language;
        public Dictionary<string, string> Dictionary => m_Dictionary;
    }
}