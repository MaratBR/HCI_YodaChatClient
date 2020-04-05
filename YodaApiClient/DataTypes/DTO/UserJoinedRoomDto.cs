using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes.DTO
{
    public class UserJoinedRoomDto
    {
        public ChatUserDto User { get; set; }

        public Guid RoomId { get; set; }
    }
}
