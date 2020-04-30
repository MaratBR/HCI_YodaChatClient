using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes.DTO
{
    public class ChatUserDepartedDto
    {
        public Guid RoomId { get; set; }

        public int UserId { get; set; }
    }
}
