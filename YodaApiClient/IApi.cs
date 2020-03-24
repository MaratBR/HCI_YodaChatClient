using System;
using System.Collections.Generic;
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
        Task<IApi> CreateApi(AuthenticationRequest request);

        Task<IApi> RegisterUserAndCreateApi(RegistrationRequest request);
    }

    public class CreateRoomRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public interface IApi
    {
        Task<User> GetUserAsync();

        Task<ICollection<Room>> GetRooms();

        Task<Room> CreateRoom(CreateRoomRequest createRoom);

        Task<IChatApiHandler> Connect();
    }
}
