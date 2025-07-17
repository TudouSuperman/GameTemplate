using System;
using System.Text.RegularExpressions;

namespace GameApp
{
    public static class WebUtility
    {
        private static readonly Regex s_Regex = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\*\+,;=.]+$");

        public static bool CheckUri(string uri)
        {
            return !string.IsNullOrEmpty(uri) && s_Regex.IsMatch(uri);
        }

        public static string EscapeString(string stringToEscape)
        {
            return Uri.EscapeDataString(stringToEscape);
        }

        public static string UnescapeString(string stringToUnescape)
        {
            return Uri.UnescapeDataString(stringToUnescape);
        }
    }
}