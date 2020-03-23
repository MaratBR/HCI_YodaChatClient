using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.Constants
{
    public static class ApiReference
    {
        public static readonly string AUTHENTICATION_CONTROLLER_ROUTE = "authentication";
        public static readonly string AUTHENTICATION_ROUTE = $"{AUTHENTICATION_CONTROLLER_ROUTE}/authenticate";
        public static readonly string REGISTRATION_ROUTE = $"{AUTHENTICATION_CONTROLLER_ROUTE}/register";
        public static readonly string CURRENT_USER_ROUTE = $"{AUTHENTICATION_CONTROLLER_ROUTE}/user";
    }
}
