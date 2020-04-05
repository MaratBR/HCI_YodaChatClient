using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using YodaApiClient.Constants;
using YodaApiClient.DataTypes;
using YodaApiClient.Helpers;

namespace YodaApiClient.Implementation
{
    class RoomsResponse
    {
        public List<Room> Rooms { get; set; }
    }

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
        public async Task<FileModel> UploadFileAsync(Stream fileStream, string fileName)
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(fileStream), "file", fileName);

                HttpResponseMessage response;

                try
                {
                    response = await httpClient.PostAsync(configuration.AppendPathToMainUrl(ApiReference.UPLOAD_ROUTE), content);
                }
                catch (HttpRequestException exc)
                {
                    throw new ServiceUnavailableException(exc.Message);
                }

                await response.ThrowErrorIfNotSuccessful();

                return await response.GetJson<FileModel>();
            }
        }
        public async Task DownloadFileAsync(Guid id, Stream fileStream)
        {
            var response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(string.Format(ApiReference.DOWNLOAD_FILE_ROUTE, id)));

            using(var stream = await response.Content.ReadAsStreamAsync())
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        #endregion
    }
}