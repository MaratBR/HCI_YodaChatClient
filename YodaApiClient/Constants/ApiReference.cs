using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.Constants
{
    public static class ApiReference
    {
        private static readonly string AUTHENTICATION_CONTROLLER_ROUTE = "authentication";
        private static readonly string ROOMS_CONTROLLER_ROUTE = "rooms";



        public static readonly string AUTHENTICATION_ROUTE = $"{AUTHENTICATION_CONTROLLER_ROUTE}/authenticate";
        public static readonly string REGISTRATION_ROUTE = $"{AUTHENTICATION_CONTROLLER_ROUTE}/register";
        public static readonly string CURRENT_USER_ROUTE = $"{AUTHENTICATION_CONTROLLER_ROUTE}/user";

        public static readonly string LIST_OF_ROOMS_ROUTE = ROOMS_CONTROLLER_ROUTE;
        public static readonly string CREATE_ROOM_ROUTE = ROOMS_CONTROLLER_ROUTE;

        public static readonly string SIGNALR_HUB_ROUTE = "chat";
    }
}
