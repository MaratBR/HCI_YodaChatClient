using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;
using YodaApiClient.DataTypes.DTO;
using YodaApiClient.DataTypes.Requests;

namespace YodaApiClient
{

    public interface IApi : IFileUploader
    {

        #region Connection

        Task<IChatClient> ConnectAsync();

        #endregion

        #region User(s)

        Task<User> GetUserAsync();
        Task<User> GetUserAsync(Guid id);

        #endregion

        #region Rooms and messages

        Task<List<Room>> GetRoomsAsync();
        Task<Room> GetRoomAsync(Guid id);
        Task<Room> CreateRoomAsync(CreateRoomRequest createRoom);
        Task<List<ChatMembershipDto>> GetRoomMembersAsync(Guid roomId);
        Task JoinRoomAsync(Guid roomId);
        Task LeaveRoomAsync(Guid roomId);
        Task<List<ChatMessageDto>> GetRoomMessages(Guid roomId, DateTime? before = null);


        #endregion

        #region Files

        Task DownloadFileAsync(Guid id, Stream fileStream);

        Task<List<FileModel>> GetUserFiles(DateTime? after = null);

        #endregion

        #region Session info

        SessionInfo GetSessionInfo();
        Guid GetApiSessionGuid();
        string GetAccessToken();

        #endregion
    }
}
