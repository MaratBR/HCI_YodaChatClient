using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.Helpers
{
    static class Json
    {
        private static JsonSerializerSettings settings =
            new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

        public static T Deserialize<T>(string text) => JsonConvert.DeserializeObject<T>(text, settings);

        public static string Serialize(object val) => JsonConvert.SerializeObject(val, settings);
    }
}
