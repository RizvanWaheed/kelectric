using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CRM.Common
{
    public class CodeGenerator
    {
        public static string ConfirmationString(int maxSize)
        {
            //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            //var random = new Random();
            //return new string(Enumerable.Repeat(chars, length)
            //  .Select(s => s[random.Next(s.Length)]).ToArray());
            //char[] chars = new char[62];
            var chars = String.Format("{0,1}", DateTime.Now.Date.ToString("ddMMMMyyyyHHmmss"), "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890").ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
}
