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

        /// <summary>
        ///  Throws error is response has status code other than 2XX
        /// </summary>
        /// <exception cref="InvalidCredentialsException"></exception>
        /// <exception cref="UnexpectedHttpStatusCodeException"></exception>
        /// <param name="httpResponse"></param>
        public static void ThrowErrorIfNotSuccessful(this HttpResponseMessage httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new InvalidCredentialsException();
                else
                    throw new UnexpectedHttpStatusCodeException(httpResponse.StatusCode);
            }
        }
    }
}
