﻿using System;
using System.Collections.Generic;
using System.IO;
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

        #endregion Connection

        #region User(s)

        Task<User> GetUserAsync();

        Task<User> GetUserAsync(Guid id);

        #endregion User(s)

        #region Rooms and messages

        Task<List<Room>> GetRoomsAsync();

        Task<Room> GetRoomAsync(Guid id);

        Task<Room> CreateRoomAsync(CreateRoomRequest createRoom);

        Task<List<ChatMembershipDto>> GetRoomMembersAsync(Guid roomId);

        Task JoinRoomAsync(Guid roomId);

        Task LeaveRoomAsync(Guid roomId);

        Task<List<ChatMessageDto>> GetRoomMessages(Guid roomId, DateTime? before = null);

        #endregion Rooms and messages

        #region Files

        Task DownloadFileAsync(Guid id, Stream fileStream);

        Task<List<FileModel>> GetUserFiles(DateTime? after = null);

        #endregion Files

        #region Session info

        SessionInfo GetSessionInfo();

        Guid GetApiSessionGuid();

        string GetAccessToken();

        #endregion Session info
    }
}