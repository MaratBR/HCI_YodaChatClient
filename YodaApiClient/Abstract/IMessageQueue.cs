using System.Threading.Tasks;
using YodaApiClient.DataTypes.DTO;

namespace YodaApiClient
{
    public struct MessageQueueStatus
    {
        public bool Sent;

        public string Error;

        public long? Id;
    }

    public interface IMessageQueue
    {
        Task<MessageQueueStatus> PutToQueue(ChatMessageRequestDto message);
    }
}