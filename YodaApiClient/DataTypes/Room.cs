using System;

namespace YodaApiClient.DataTypes
{
    public class Room
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}