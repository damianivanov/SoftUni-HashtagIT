namespace HashtagIT.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class Cipher
    {
        public static string Encrypt(string plainText, string salt)
        {
            if (plainText == null)
            {
                return null;
            }

            if (salt == null)
            {
                salt = string.Empty;
            }

            // Get the bytes of the string
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(plainText);
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            // Hash the password with SHA256
            saltBytes = SHA256.Create().ComputeHash(saltBytes);

            var bytesEncrypted = Cipher.Encrypt(bytesToBeEncrypted, saltBytes);

            return Convert.ToBase64String(bytesEncrypted);
        }

        public static string Decrypt(string encryptedText, string salt)
        {
            if (encryptedText == null)
            {
                return null;
            }

            if (salt == null)
            {
                salt = string.Empty;
            }

            // Get the bytes of the string
            var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            saltBytes = SHA256.Create().ComputeHash(saltBytes);

            var bytesDecrypted = Cipher.Decrypt(bytesToBeDecrypted, saltBytes);

            return Encoding.UTF8.GetString(bytesDecrypted);
        }

        private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    aES.KeySize = 256;
                    aES.BlockSize = 128;
                    aES.Key = key.GetBytes(aES.KeySize / 8);
                    aES.IV = key.GetBytes(aES.BlockSize / 8);

                    aES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    aES.KeySize = 256;
                    aES.BlockSize = 128;
                    aES.Key = key.GetBytes(aES.KeySize / 8);
                    aES.IV = key.GetBytes(aES.BlockSize / 8);
                    aES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}
