using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes.DTO
{
    public class UserStatusDto
    {
        public Guid UserId { get; set; }

        public bool IsOnline { get; set; }
    }
}
