using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using YodaApiClient.Constants;
using YodaApiClient.DataTypes;
using YodaApiClient.DataTypes.DTO;
using YodaApiClient.Helpers;

namespace YodaApiClient.Implementation
{
    internal class Api : IApi
    {
        private SessionInfo sessionInfo;
        private readonly ApiConfiguration configuration;
        private readonly HttpClient httpClient;

        public Api(SessionInfo sessionInfo, ApiConfiguration configuration)
        {
            this.sessionInfo = sessionInfo;
            this.configuration = configuration;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionInfo.Token}");
        }

        public async Task<IChatClient> ConnectAsync()
        {
            var handler = new ChatClient(this, configuration);
            await handler.Connect();

            return handler;
        }

        #region "Raw" data

        public async Task<Room> CreateRoomAsync(CreateRoomRequest request)
        {
            HttpResponseMessage response;

            try
            {
                response = await httpClient.PostJson(configuration.AppendPathToMainUrl(ApiReference.CREATE_ROOM_ROUTE), request);
            }
            catch(HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            await response.ThrowErrorIfNotSuccessful();

            var room = await response.GetJson<Room>();

            return room;
        }

        class RoomsResponse
        {
            public List<Room> Rooms { get; set; }
        }

        public async Task<List<Room>> GetRoomsAsync()
        {
            HttpResponseMessage response;

            try
            {
                response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(ApiReference.LIST_OF_ROOMS_ROUTE));
            }
            catch(HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }


            await response.ThrowErrorIfNotSuccessful();

            var roomsResponse = await response.GetJson<RoomsResponse>();
            return roomsResponse.Rooms;
        }
        public async Task<Room> GetRoomAsync(Guid id)
        {
            HttpResponseMessage response;

            try
            {
                response = await httpClient.GetAsync(
                    configuration.AppendPathToMainUrl(
                        string.Format(ApiReference.GET_ROOM_ROUTE, id))
                    );
            }
            catch (HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            await response.ThrowErrorIfNotSuccessful();

            return await response.GetJson<Room>();
        }

        public SessionInfo GetSessionInfo() => sessionInfo;
        public Guid GetApiSessionGuid() => GetSessionInfo().SessionId;
        public string GetAccessToken()
        {
            return sessionInfo.Token;
        }

        public async Task<User> GetUserAsync()
        {
            HttpResponseMessage response;

            try
            {
                response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(ApiReference.CURRENT_USER_ROUTE));
            }
            catch (HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            await response.ThrowErrorIfNotSuccessful();

            return await response.GetJson<User>();
        }
        public async Task<User> GetUserAsync(Guid id)
        {
            HttpResponseMessage response;

            try
            {
                response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(string.Format(ApiReference.GET_USER_ROUTE, id)));
            }
            catch (HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            await response.ThrowErrorIfNotSuccessful();

            return await response.GetJson<User>();
        }

        public async Task<FileModel> GetFileModelAsync(Guid id)
        {
            HttpResponseMessage response;
            try
            {
                response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(string.Format(ApiReference.GET_FILEMODEL_ROUTE, id)));
            }
            catch (HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            if (response.StatusCode == System.Net.HttpStatusCode.RequestEntityTooLarge)
            {
                throw new FileTooBigException();
            }
            await response.ThrowErrorIfNotSuccessful();

            return await response.GetJson<FileModel>();
        }
        public async Task<FileModel> UploadFileAsync(FileStream fileStream, string fileName)
        {
            var multipart = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.Add("Content-Length", fileStream.Length.ToString());
            multipart.Add(streamContent, "file", fileName);

            HttpResponseMessage response;

            try
            {
                response = await httpClient.PostAsync(configuration.AppendPathToMainUrl(ApiReference.UPLOAD_ROUTE), streamContent);
            }
            catch (HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }
            string resp = await response.Content.ReadAsStringAsync();

            await response.ThrowErrorIfNotSuccessful();

            return await response.GetJson<FileModel>();
        }
        public async Task DownloadFileAsync(Guid id, Stream fileStream)
        {
            var response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(string.Format(ApiReference.DOWNLOAD_FILE_ROUTE, id)));

            using(var stream = await response.Content.ReadAsStreamAsync())
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        private class RoomMessagesResponse
        {
            public List<ChatMessageDto> Messages { get; set; }
        }

        public async Task<List<ChatMessageDto>> GetRoomMessages(Guid roomId, DateTime? before = null)
        {
            HttpResponseMessage response;

            try
            {
                string url = string.Format(ApiReference.GET_ROOM_MESSAGES_ROUTE, roomId);
                if (before == null)
                    url += $"?=before={before}";
                response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(url));
            }
            catch (HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            await response.ThrowErrorIfNotSuccessful();

            var data = await response.GetJson<RoomMessagesResponse>();

            return data.Messages;

        }

        class MembersResponse
        {
            public List<ChatMembershipDto> Users { get; set; }
        }

        public async Task<List<ChatMembershipDto>> GetRoomMembersAsync(Guid roomId)
        {
            HttpResponseMessage response;

            try
            {
                string url = string.Format(ApiReference.GET_ROOM_MEMBERS_ROUTE, roomId);
                response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(url));
            }
            catch (HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            await response.ThrowErrorIfNotSuccessful();

            var data = await response.GetJson<MembersResponse>();

            return data.Users;
        }

        public async Task JoinRoomAsync(Guid roomId)
        {
            HttpResponseMessage response;

            try
            {
                string url = string.Format(ApiReference.ROOM_MEMBERSHIP_ROUTE, roomId);
                response = await httpClient.PostAsync(configuration.AppendPathToMainUrl(url), new StringContent(string.Empty));
            }
            catch (HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            await response.ThrowErrorIfNotSuccessful();
        }

        public async Task LeaveRoomAsync(Guid roomId)
        {
            HttpResponseMessage response;

            try
            {
                string url = string.Format(ApiReference.ROOM_MEMBERSHIP_ROUTE, roomId);
                response = await httpClient.DeleteAsync(configuration.AppendPathToMainUrl(url));
            }
            catch (HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            await response.ThrowErrorIfNotSuccessful();
        }

        #endregion
    }
}