using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApp.Persistence
{
    public static class Store
    {
        private static IStore _instance;

        public static IStore Instance => _instance;

        public static void SetInstance(IStore store)
        {
            _instance = store ?? throw new ArgumentNullException(nameof(store));
        }
    }
}
