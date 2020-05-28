using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace YodaApiClient.Helpers
{
    internal static class Json
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