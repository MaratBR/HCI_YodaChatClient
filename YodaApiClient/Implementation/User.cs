using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient.Implementation
{
    class User : IUser
    {
        private readonly UserDto dto;
        public User(UserDto dto)
        {
            this.dto = dto;
        }

        public string UserName => dto.UserName;

        public string Email => dto.Email;

        public string Alias => dto.Alias;

        public Gender? Gender => dto.Gender;

        public Guid Id => dto.Id;
    }
}
