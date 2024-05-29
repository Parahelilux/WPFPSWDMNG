using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WPFPSWDMNG
{
    public class PasswordProtector : IPasswordProtector
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("your-encryption-key-here");

        public string EncryptPassword(string password)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new System.IO.MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cs))
                    {
                        writer.Write(password);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string DecryptPassword(string encryptedPassword)
        {
            var buffer = Convert.FromBase64String(encryptedPassword);
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                var iv = new byte[aes.BlockSize / 8];
                Buffer.BlockCopy(buffer, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new System.IO.MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
