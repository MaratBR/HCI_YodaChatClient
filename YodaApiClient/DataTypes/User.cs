﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes
{
    // не спрашивайте
    public enum Gender : byte
    {
        Respect = 0, // aka male
        Smooch = 1 // aka female
    }

    public class User
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Alias { get; set; }

        public Gender? Gender { get; set; }

        public int Id { get; set; }
    }
}