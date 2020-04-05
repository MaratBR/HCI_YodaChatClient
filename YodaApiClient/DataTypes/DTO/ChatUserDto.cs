using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes.DTO
{
    public class ChatUserDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Gender? Gender { get; set; }
    }
}
