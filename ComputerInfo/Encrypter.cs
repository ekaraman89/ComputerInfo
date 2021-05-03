using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ComputerInfo
{
    public static class Encrypter
    {
        private const int Keysize = 128;

        private const int DerivationIterations = 1000;

        #region - Encrypt -

        /// <summary>
        /// Metin sifreleme -
        /// </summary>
        /// <param name="plainText">Sifrelenecek metin</param>
        /// <param name="passPhrase">Sifrelenen metnin sifresi</param>
        /// <param name="cipherMode">Sifre modu</param>
        /// <param name="paddingMode">Dolgulama modu</param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string passPhrase, CipherMode cipherMode, PaddingMode paddingMode)
        {
            byte[] saltStringBytes = Generate128BitsOfRandomEntropy();
            byte[] ivStringBytes = Generate128BitsOfRandomEntropy();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using Rfc2898DeriveBytes password = new(passPhrase, saltStringBytes, DerivationIterations);
            byte[] keyBytes = password.GetBytes(Keysize / 8);
            using RijndaelManaged symmetricKey = new();
            symmetricKey.BlockSize = 128;
            symmetricKey.Mode = cipherMode;
            symmetricKey.Padding = paddingMode;
            using ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes);
            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = saltStringBytes;
            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        #endregion - Encrypt -

        #region - Decrypt -

        /// <summary>
        /// Metin sifre cozucu -
        /// </summary>
        /// <param name="cipherText">Sifrelenmis metin</param>
        /// <param name="passPhrase">Sifrelenen metnin sifresi</param>
        /// <param name="cipherMode">Sifre modu</param>
        /// <param name="paddingMode">Dolgulama modu</param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, string passPhrase, CipherMode cipherMode, PaddingMode paddingMode)
        {
            byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using Rfc2898DeriveBytes password = new(passPhrase, saltStringBytes, DerivationIterations);
            byte[] keyBytes = password.GetBytes(Keysize / 8);
            using RijndaelManaged symmetricKey = new();
            symmetricKey.BlockSize = 128;
            symmetricKey.Mode = cipherMode;
            symmetricKey.Padding = paddingMode;
            using ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes);
            using MemoryStream memoryStream = new(cipherTextBytes);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        #endregion - Decrypt -

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}