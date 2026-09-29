using System.Security.Cryptography;
using System.Text;

namespace Chat.Helper
{
    public class EncryptionHelper
    {
        private readonly string _key;
        private readonly string _iv;

        public EncryptionHelper(IConfiguration configuration)
        {
            _key = configuration["EncryptionSettings:Key"];
            _iv = configuration["EncryptionSettings:IV"];
        }

        public string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_key);
                aes.IV = Encoding.UTF8.GetBytes(_iv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(cipherBytes);
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_key);
                aes.IV = Encoding.UTF8.GetBytes(_iv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }
    }
}