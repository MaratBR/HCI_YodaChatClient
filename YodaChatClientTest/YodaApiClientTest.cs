using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YodaApiClient;

namespace YodaChatClientTest
{
    [TestClass]
    public class UnitTestYodaApiClient
    {
        private IApiProvider apiProvider;

        public UnitTestYodaApiClient()
        {
            ApiConfiguration configuration = new ApiConfiguration();
            apiProvider = new ApiProvider(configuration);
        }

        [TestMethod]
        public async Task TestRegistration()
        {
            var api = await CreateApi();
            var user = await api.GetUserAsync();
            Assert.AreEqual(user.Email, "TotallyNotFakeEmailTrustMe@LegitEMailService.com");
        }

        [TestMethod]
        public async Task TestMainApiMethods()
        {
            var api = await CreateApi();

            Assert.IsNotNull(await api.GetUserAsync());
            Assert.IsNotNull(await api.GetRooms());


            var roomName = $"TestRoom-{Guid.NewGuid()}";
            var roomRequest = new CreateRoomRequest
            {
                Name = roomName,
                Description = "some description"
            };

            var room = await api.CreateRoom(roomRequest);
            Assert.IsNotNull(room);
            Assert.AreEqual(room.Name, roomName);
        }

        [TestMethod]
        public async Task TestChatHandler()
        {
            var api = await CreateApi();
            var handler = await api.Connect();

            string messageText = "cake is a lie", receivedText = null;
            Guid? senderId = null,
                userJoinedId = null,
                userLeftId = null,
                roomId = null;
            var user = await api.GetUserAsync();

            handler.MessageReceived +=
                (object sender, ChatMessageEventArgs e) =>
                {
                    senderId = e.SenderId;
                    receivedText = e.Text;
                };

            handler.UserJoined +=
                (object sender, ChatUserJoinedEventArgs e) =>
                {
                    userJoinedId = e.UserId;
                    roomId = e.RoomId;
                };

            var room = await api.CreateRoom(new CreateRoomRequest
            {
                Name = $"TestRoom-{Guid.NewGuid()}",
                Description = "some description"
            });

            await handler.JoinRoom(room.Id);
            await Task.Delay(500);
            Assert.AreEqual(user.Id, userJoinedId);
            Assert.AreEqual(room.Id, roomId);
            await handler.SendToRoom(messageText, room.Id);
            await Task.Delay(500);
            Assert.AreEqual(messageText, receivedText);
            Assert.AreEqual(user.Id, senderId);
        }

        public Task<IApi> CreateApi()
        {
            var guid = Guid.NewGuid();

            return apiProvider.RegisterUserAndCreateApi(new RegistrationRequest
            {
                UserName = $"TestUser-{guid}",
                Password = "12345678qwerty@UPPERCASE",
                Gender = YodaApiClient.DataTypes.Gender.Respect,
                PhoneNumber = "+7 401 801 5148",
                Email = "TotallyNotFakeEmailTrustMe@LegitEMailService.com"
            });
        }
    }
}
