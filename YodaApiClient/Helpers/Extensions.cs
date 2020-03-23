using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.Helpers
{
    public static class Extensions
    {
        public static Task<HttpResponseMessage> PostJson(this HttpClient client, string requestUri, object body)
            => client.PostAsync(
                requestUri,
                new StringContent(Json.Serialize(body), Encoding.UTF8, "application/json")
                );

        public static async Task<T> GetJson<T>(this HttpResponseMessage httpResponse)
            => Json.Deserialize<T>(await httpResponse.Content.ReadAsStringAsync());
    }
}
