using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Common.Security.Cifrado
{
    public class AES
    {
        public AES()
        {
        }

        /// <summary>
        /// Encripta
        /// </summary>
        /// <param name="PlainText">Texto a cifrar</param>
        /// <param name="Password">Nuestra contraseña</param>
        /// <param name="SaltValue">Puede ser cualquier cadena</param>
        /// <param name="hashAlgorithm">El algoritmo para generar el hash, puede ser MD5 o SHA1
        /// <param name="PasswordIterations">Con uno o dos será suficiente</param>
        /// <param name="InitialVector">Debe ser una cadena de 16 bytes, es decir, 16 caracteres</param>
        /// <param name="KeySize">Puede ser cualquiera de estos tres valores: 128, 192 o 256</param>
        /// <returns></returns>
        public string Encrypt(string PlainText, string Password, string SaltValue, string hashAlgorithm, int PasswordIterations, string InitialVector, int KeySize)
        {
            try
            {
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(SaltValue);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(PlainText);

                PasswordDeriveBytes password = new PasswordDeriveBytes(Password, saltValueBytes, hashAlgorithm, PasswordIterations);

                byte[] keyBytes = password.GetBytes(KeySize / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged();

                symmetricKey.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, InitialVectorBytes);

                MemoryStream memoryStream = new MemoryStream();

                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

                cryptoStream.FlushFinalBlock();

                byte[] cipherTextBytes = memoryStream.ToArray();

                memoryStream.Close();
                cryptoStream.Close();

                string cipherText = Convert.ToBase64String(cipherTextBytes);

                return cipherText;
            }
            catch
            {
                //MessageBox.Show("The typed information is wrong. Please, check it.", "FoS TeaM", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return null;
            }
        }

        /// <summary>
        /// Desencripta 
        /// </summary>
        /// <param name="PlainText">Texto a cifrar</param>
        /// <param name="Password">Nuestra contraseña</param>
        /// <param name="SaltValue">Puede ser cualquier cadena</param>
        /// <param name="hashAlgorithm">El algoritmo para generar el hash, puede ser MD5 o SHA1
        /// <param name="PasswordIterations">Con uno o dos será suficiente</param>
        /// <param name="InitialVector">Debe ser una cadena de 16 bytes, es decir, 16 caracteres</param>
        /// <param name="KeySize">Puede ser cualquiera de estos tres valores: 128, 192 o 256</param>
        /// <returns></returns>
        public string Decrypt(string PlainText, string Password, string SaltValue, string HashAlgorithm, int PasswordIterations, string InitialVector, int KeySize)
        {
            try
            {
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(SaltValue);

                byte[] cipherTextBytes = Convert.FromBase64String(PlainText);

                PasswordDeriveBytes password = new PasswordDeriveBytes(Password, saltValueBytes, HashAlgorithm, PasswordIterations);

                byte[] keyBytes = password.GetBytes(KeySize / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged();

                symmetricKey.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, InitialVectorBytes);

                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                memoryStream.Close();
                cryptoStream.Close();

                string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                return plainText;
            }
            catch (Exception)
            {
                //MessageBox.Show("The typed information is wrong. Please, check it.", "FoS TeaM", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return null;
            }
        }
    }

}
