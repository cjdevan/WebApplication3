using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication3.Model
{
    public class EncryptDecrypt
    {
        public class KeyGenerator
        {
            public static string GenerateRandomKey(int keyLengthInBytes)
            {
                byte[] keyBytes = new byte[keyLengthInBytes];

                using (var rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetBytes(keyBytes);
                }

                return Convert.ToBase64String(keyBytes);
            }
        }

        public static byte[] EncryptData(string data, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV(); // Generate a random IV

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                            csEncrypt.Write(dataBytes, 0, dataBytes.Length);
                       
                            // Concatenate the IV and the encrypted data
                            byte[] result = new byte[aesAlg.IV.Length + msEncrypt.Length];
                            Buffer.BlockCopy(aesAlg.IV, 0, result, 0, aesAlg.IV.Length);
                            Buffer.BlockCopy(msEncrypt.ToArray(), 0, result, aesAlg.IV.Length, (int)msEncrypt.Length);

                            return result;
                        }
                    }
                }
            }
        }

        public static string DecryptData(string encryptedData, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Extract the IV from the encrypted data
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                byte[] iv = encryptedBytes.Take(16).ToArray();

                aesAlg.IV = iv;

                // Extract the encrypted data without the IV
                byte[] dataWithoutIV = encryptedBytes.Skip(16).ToArray();

                // Decrypt the data
                using (MemoryStream msDecrypt = new MemoryStream(dataWithoutIV))
                {
                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}
