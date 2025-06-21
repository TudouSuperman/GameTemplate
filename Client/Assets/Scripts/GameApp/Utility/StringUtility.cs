using System;
using System.Text.RegularExpressions;

namespace GameApp
{
    public static class StringUtility
    {
        /// <summary>
        /// 检查是否是邮件。
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(this string email)
        {
            Regex regex = new Regex("[a-zA-Z_0-9]+@[a-zA-Z_0-9]{2,6}(\\.[a-zA-Z_0-9]{2,3})+");
            return regex.IsMatch(email);
        }

        /// <summary>
        /// 检查是否是电话号码。
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(this string strInput)
        {
            Regex reg = new Regex(@"(^\d{11}$)");
            return reg.IsMatch(strInput);
        }

        /// <summary>
        /// 检查是否是后缀名。
        /// </summary>
        public static bool IsSuffix(this string str, string suffix, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            // 总长度减去后缀的索引等于后缀的长度。
            int indexOf = str.LastIndexOf(suffix, StringComparison.CurrentCultureIgnoreCase);
            return indexOf != -1 && indexOf == str.Length - suffix.Length;
        }

        /// <summary>
        /// 把 String 类型转换成 Byte。
        /// </summary>
        public static Int16 ToByte(this String str)
        {
            Byte.TryParse(str, out Byte temp);
            return temp;
        }

        /// <summary>
        /// 把 String 类型转换成 Int16。
        /// </summary>
        public static Int16 ToInt16(this String str)
        {
            Int16.TryParse(str, out Int16 temp);
            return temp;
        }

        /// <summary>
        /// 把 String 类型转换成 Int32。
        /// </summary>
        public static Int32 ToInt32(this String str)
        {
            Int32.TryParse(str, out Int32 temp);
            return temp;
        }

        /// <summary>
        /// 把 String 类型转换成 Int64。
        /// </summary>
        public static Int64 ToInt64(this String str)
        {
            Int64.TryParse(str, out Int64 temp);
            return temp;
        }

        /// <summary>
        /// 把 String 类型转换成 Single。
        /// </summary>
        public static Single ToSingle(this String str)
        {
            Single.TryParse(str, out Single temp);
            return temp;
        }

        /// <summary>
        /// 把 String 类型转换成 Boolean。
        /// </summary>
        public static Boolean ToBoolean(this String str)
        {
            Boolean.TryParse(str, out Boolean temp);
            return temp;
        }
    }
}