using System.Threading.Tasks;
using YodaApiClient.DataTypes.Requests;

namespace YodaApiClient.Abstract
{
    public interface IApiProvider
    {
        /// <summary>
        /// Authenticate a use and create instance of IApi
        /// </summary>
        /// <param name="request">request data sent to API</param>
        /// <returns>Instance of IApi</returns>
        Task<IApi> CreateApi(AuthenticationRequest request);


        /// <summary>
        /// Registers a new user and then call CreateApi
        /// </summary>
        /// <param name="request"></param>
        /// <returns>new instance of IApi</returns>
        Task<IApi> RegisterUserAndCreateApi(RegistrationRequest request);

        Task<IApi> CreateApi(SessionInfo session);

        Task<string> PingAsync();
    }
}
