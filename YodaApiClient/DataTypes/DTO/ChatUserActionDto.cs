using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes.DTO
{
    public enum ChatUserActionType : byte
    {
        Joined,
        Left,
        QuacksterAscending
    }

    public class ChatUserActionDto
    {
        public ChatUserActionType ActionType { get; set; }

        public Guid UserId { get; set; }

        public Guid RoomId { get; set; }
    }
}
