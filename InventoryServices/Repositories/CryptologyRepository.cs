using CommonLibrary.Dtos;
using InventoryServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Repositories
{
    public class CryptologyRepository : ICryptologyRepository
    {
        private static readonly string encryptionKey = "!)($^$@";

        public string EncryptString(CryptographyDtos cryptographyDtos)
        {
            if (string.IsNullOrWhiteSpace(cryptographyDtos.ToEncryptString)) return "";

            byte[] keyArray;

            var toEncryptArray = UTF8Encoding.UTF8.GetBytes(cryptographyDtos.ToEncryptString);

            // If hashing use get hashcode regards to your key
            if (cryptographyDtos.UseHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();

                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(encryptionKey));

                hashmd5.Clear();
            }
            else keyArray = UTF8Encoding.UTF8.GetBytes(encryptionKey);

            // Set the secret key for the tripleDES algorithm
            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            // Transform the specified region of bytes array to resultArray
            var cTransform = tdes.CreateEncryptor();

            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();

            // Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string DecryptString(CryptographyDtos cryptographyDtos)
        {
            if (string.IsNullOrWhiteSpace(cryptographyDtos.CipherString)) return "";
            byte[] keyArray;
            var toEncryptArray = Convert.FromBase64String(cryptographyDtos.CipherString.Replace(' ', '+'));

            if (cryptographyDtos.UseHashing)
            {
                // If hashing was used get the hash code with regards to your key
                var hashmd5 = new MD5CryptoServiceProvider();

                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(encryptionKey));

                hashmd5.Clear();
            }
            else
            {
                // If hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(encryptionKey);
            }

            // Set the secret key for the tripleDES algorithm
            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = tdes.CreateDecryptor();

            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();

            // Return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
