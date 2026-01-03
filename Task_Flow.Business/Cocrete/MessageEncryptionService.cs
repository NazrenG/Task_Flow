using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Task_Flow.Business.Cocrete
{
    public class MessageEncryptionService
    {
        private readonly byte[] _key;

        public MessageEncryptionService(IConfiguration config)
        {
            var keyString = Environment.GetEnvironmentVariable("AES_KEY");
            if (string.IsNullOrEmpty(keyString))
                throw new Exception("AES_KEY not set in environment variables!");

            _key = Convert.FromBase64String(keyString);
        }

        public (string CipherText, string IV) Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            return (
                Convert.ToBase64String(encrypted),
                Convert.ToBase64String(aes.IV)
            );
        }

        public string Decrypt(string cipherText, string iv)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                return "";

            if (string.IsNullOrWhiteSpace(iv))
                return cipherText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = Convert.FromBase64String(iv);

            using var decryptor = aes.CreateDecryptor();
            var bytes = Convert.FromBase64String(cipherText);
            var decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
