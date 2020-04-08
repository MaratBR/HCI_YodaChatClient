﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes
{
    public class User
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Alias { get; set; }

        public Gender? Gender { get; set; }

        public Guid Id { get; set; }
    }
}