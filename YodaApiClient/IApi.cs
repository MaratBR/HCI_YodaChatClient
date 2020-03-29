using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient
{
    public class RegistrationRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public Gender? Gender { get; set; } = null;
    }

    public class AuthenticationRequest
    {
        public string Login { get; set; }

        public string Password { get; set; }
    }

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

        Task<bool> Ping();
    }

    public class CreateRoomRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public interface IApi
    {
        Task<IUser> GetUserAsync();

        Task<IUser> GetUserAsync(Guid id);

        Task<ICollection<Room>> GetRooms();

        Task<Room> CreateRoom(CreateRoomRequest createRoom);

        Task<IChatApiHandler> Connect();

        Task<FileModel> UploadFile(string fileName, Stream fileStream);

        SessionInfo GetSessionInfo();

        Guid GetGuid();

        string GetAccessToken();
    }
}
