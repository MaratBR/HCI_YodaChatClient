using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes.DTO
{
    public enum UserActionType : byte
    {
        Joined,
        Left,
        QuacksterAscending
    }

    public class UserActionDto
    {
        public UserActionType ActionType { get; set; }

        public Guid UserId { get; set; }

        public Guid RoomId { get; set; }
    }
}
