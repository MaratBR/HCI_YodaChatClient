﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes.DTO
{
    public class UserDepartedDto
    {
        public Guid RoomId { get; set; }

        public Guid UserId { get; set; }
    }
}
