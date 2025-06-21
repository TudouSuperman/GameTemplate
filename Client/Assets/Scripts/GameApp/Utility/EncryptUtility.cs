using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GameApp
{
    /// <summary>
    /// 加密。
    /// </summary>
    public static class EncryptUtility
    {
        #region 加密/解密（二进制）

        private static readonly string Password = "b9ceceae146dda88"; // 加密密码 GameApp.JiaMi
        private static readonly byte[] Salt = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }; // 盐值
        private static readonly int Iterations = 10000; // PBKDF2 迭代次数
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256; // 哈希算法

        /// <summary>
        /// 加密。
        /// </summary>
        public static string Encrypt(string text)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(text);
            using (Aes aes = Aes.Create())
            {
                // 使用 PBKDF2 派生密钥
                var keyDeriver = new Rfc2898DeriveBytes(Password, Salt, Iterations, HashAlgorithm);
                aes.Key = keyDeriver.GetBytes(32); // AESAES-256密钥
                aes.GenerateIV(); // 生成随机 IV

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (MemoryStream ms = new MemoryStream())
                {
                    // 将 IV 写入密文开头
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(plainBytes, 0, plainBytes.Length);
                        cs.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// 解密。
        /// </summary>
        public static string Decrypt(string text)
        {
            byte[] cipherBytes = Convert.FromBase64String(text);
            using (Aes aes = Aes.Create())
            {
                // 提取 IV（前 16 字节）
                byte[] iv = new byte[16];
                Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                aes.IV = iv;

                // 派生密钥
                var keyDeriver = new Rfc2898DeriveBytes(Password, Salt, Iterations, HashAlgorithm);
                aes.Key = keyDeriver.GetBytes(32);

                // 提取密文数据（ IV 之后的部分）
                byte[] encryptedData = new byte[cipherBytes.Length - iv.Length];
                Array.Copy(cipherBytes, iv.Length, encryptedData, 0, encryptedData.Length);

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (MemoryStream ms = new MemoryStream(encryptedData))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取 MD5。
        /// </summary>
        public static string MD5(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytResult = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(value));
            string strResult = BitConverter.ToString(bytResult);
            strResult = strResult.Replace("-", "");
            return strResult;
        }

        /// <summary>
        /// 获取文件的 MD5。
        /// </summary>
        /// <param name="filePath">文件路径。</param>
        /// <returns></returns>
        public static string GetFileMD5(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] bytResult = md5.ComputeHash(file);
                string strResult = BitConverter.ToString(bytResult);
                strResult = strResult.Replace("-", "");
                return strResult;
            }
            catch
            {
                return null;
            }
        }
    }
}