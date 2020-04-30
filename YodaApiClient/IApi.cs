using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;
using YodaApiClient.DataTypes.DTO;

namespace YodaApiClient
{
    public class RegistrationRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public byte? Gender { get; set; } = null;
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

        Task<string> PingAsync();
    }

    public class CreateRoomRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public interface IApi : IFileUploader
    {
        Task<IChatClient> ConnectAsync();

        Task<User> GetUserAsync();
        Task<User> GetUserAsync(Guid id);

        Task<List<Room>> GetRoomsAsync();
        Task<Room> GetRoomAsync(Guid id);
        Task<Room> CreateRoomAsync(CreateRoomRequest createRoom);

        Task<List<ChatMembershipDto>> GetRoomMembersAsync(Guid roomId);
        Task JoinRoomAsync(Guid roomId);
        Task LeaveRoomAsync(Guid roomId);
        Task<List<ChatMessageDto>> GetRoomMessages(Guid roomId, DateTime? before = null);

        Task DownloadFileAsync(Guid id, Stream fileStream);
        SessionInfo GetSessionInfo();
        Guid GetApiSessionGuid();
        string GetAccessToken();
    }
}
