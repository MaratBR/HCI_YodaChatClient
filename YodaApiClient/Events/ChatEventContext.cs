namespace YodaApiClient.Events
{
    public class ChatEventContext
    {
        public IChatClient Client { get; }
        public IApi Api => Client.Api;

        internal ChatEventContext(IChatClient client)
        {
            Client = client;
        }
    }
}