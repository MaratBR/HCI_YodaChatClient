using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YodaApp.Persistence
{
    public interface IStore
    {
        string Get(string key);

        void Set(string key, string value);
    }

    public static class StoreExtension
    {
        public static void SetEncryted(this IStore store, string key, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            ProtectedMemory.Protect(bytes, MemoryProtectionScope.SameLogon);
            store.Set(key, Convert.ToBase64String(bytes));
        }

        public static string GetEncryted(this IStore store, string key)
        {
            byte[] data;
            string value = store.Get(key);
            if (value == null)
                return null;
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

        public static string GetAccessToken(this IStore store) => store.GetEncryted("ACCESS_TOKEN");

        public static void SetAccessToken(this IStore store, string token) => store.SetEncryted("ACCESS_TOKEN", token);
    }
}
