using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient;

namespace YodaApp.Persistence
{
    public interface IStore
    {
        void SetSession(SessionInfo info);

        SessionInfo GetSession();

        void SetConfiguration(ApiConfiguration configuration);

        ApiConfiguration GetConfiguration();
    }

    public static class Encryptions
    {
        public static string Encrypt(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            int newSize = (bytes.Length / 16) * 16 + (bytes.Length % 16 == 0 ? 0 : 16); // размер массива должен быть кратен 16
            Array.Resize(ref bytes, newSize);
            ProtectedMemory.Protect(bytes, MemoryProtectionScope.SameLogon);
            return Convert.ToBase64String(bytes);
        }

        public static string Decrypt(string value)
        {
            if (value == null)
                return null;
            byte[] data;
            try
            {
                data = Convert.FromBase64String(value);
            }
            catch(FormatException)
            {
                return null;
            }


            try
            {
                ProtectedMemory.Unprotect(data, MemoryProtectionScope.SameLogon);
            }
            catch(CryptographicException)
            {
                return null;
            }

            return Encoding.UTF8.GetString(data);
        }
    }
}
