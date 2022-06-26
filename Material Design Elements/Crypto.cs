using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;


namespace Steganography
{
    public class Crypto
    {
        private static byte[] _salt = Encoding.ASCII.GetBytes("jasdh7834y8hfeur73rsharks214");

        /// <summary>
        /// Зашифруйте заданную строку с помощью AES.  Строка может быть расшифрована с помощью DecryptStringAES().
        ///  Параметры sharedSecret должны совпадать.
        /// </summary>
        /// <param name="plainText">Текст для шифрования.</param>
        /// <param name="sharedSecret">Пароль, используемый для генерации ключа для шифрования.</param>
        public static string EncryptStringAES(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr = null;                       // Зашифрованная строка для возврата
            RijndaelManaged aesAlg = null;              // Объект RijndaelManaged, используемый для шифрования данных.

            try
            {
                // генерация ключа из the shared secret и  _salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Создание объекта RijndaelManaged
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Создаём дешифратор для выполнения преобразования потока.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Создаём потоки, используемые для шифрования.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // добавим IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Запишем все данные в поток.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Очистим объект RijndaelManaged.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            //Возвращаем зашифрованные байты из потока памяти.
            return outStr;
        }

        /// <summary>
        /// Расшифровываем заданную строку.  Предполагаем, что строка была зашифрована с помощью EncryptStringAES()
        ///  используя идентичный sharedSecret.
        /// </summary>
        /// <param name="cipherText">Текст для расшифрования.</param>
        /// <param name="sharedSecret">Пароль, используемый для генерации ключа для дешифровки.</param>
        public static string DecryptStringAES(string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            // Объявление объекта RijndaelManaged
            // используется для расшифровки данных.
            RijndaelManaged aesAlg = null;

            // Объявим строку, используемую для хранения расшифрованного текста
            string plaintext = null;

            try
            {
                // генерируем ключ из the shared secret и _salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Создаём потоки, используемые для расшифровки.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    // Создание объекта RijndaelManaged
                    // с указанным ключом и IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Получение вектора инициализации из зашифрованного потока
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Создаём дешифратор для выполнения преобразования потока.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Чтение расшифрованных байтов из потока расшифровки
                            // и помещаем их в строку.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Очищаем объект RijndaelManaged.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}
