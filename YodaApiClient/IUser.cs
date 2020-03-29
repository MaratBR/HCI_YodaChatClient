using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient
{
    public interface IUser
    {
        string UserName { get; }

        string Email { get; }

        string Alias { get; }

        Gender? Gender { get; }

        Guid Id { get; }
    }
}
