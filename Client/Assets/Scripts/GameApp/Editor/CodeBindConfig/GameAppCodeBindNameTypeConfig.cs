using System;
using System.Collections.Generic;
using CodeBind;

namespace GameApp.Editor
{
    sealed class GameAppCodeBindNameTypeConfig
    {
        [CodeBindNameType]
        public static Dictionary<string, Type> BindNameTypeDict = new Dictionary<string, Type>()
        {
            // TMP
            { "TMPText", typeof(TMPro.TMP_Text) },
            { "TMPInputField", typeof(TMPro.TMP_InputField) },
            { "TextMeshProUGUI", typeof(TMPro.TextMeshProUGUI) },
            { "TextMeshPro", typeof(TMPro.TextMeshPro) },
        };
    }
}