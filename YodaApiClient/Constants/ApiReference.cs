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
        private static readonly string USERS_CONTROLLER_ROUTE = "users";
        private static readonly string MESSAGES_CONTROLLER_ROUTE = "messages";



        public static readonly string AUTHENTICATION_ROUTE = $"{AUTHENTICATION_CONTROLLER_ROUTE}/authenticate";
        public static readonly string REGISTRATION_ROUTE = $"{AUTHENTICATION_CONTROLLER_ROUTE}/register";
        public static readonly string CURRENT_USER_ROUTE = $"{AUTHENTICATION_CONTROLLER_ROUTE}/user";

        public static readonly string GET_USER_ROUTE = USERS_CONTROLLER_ROUTE + "/{0}";

        public static readonly string LIST_OF_ROOMS_ROUTE = ROOMS_CONTROLLER_ROUTE;
        public static readonly string CREATE_ROOM_ROUTE = ROOMS_CONTROLLER_ROUTE;
        public static readonly string GET_ROOM_ROUTE = ROOMS_CONTROLLER_ROUTE + "/{0}";
        public static readonly string GET_ROOM_MESSAGES_ROUTE = ROOMS_CONTROLLER_ROUTE + "/{0}/messages";

        public static readonly string SIGNALR_HUB_ROUTE = "chat";
        public static readonly string UPLOAD_ROUTE = "upload";
        public static readonly string GET_FILEMODEL_ROUTE = UPLOAD_ROUTE + "/{0}";
        public static readonly string DOWNLOAD_FILE_ROUTE = UPLOAD_ROUTE + "/{0}/download";
        public static readonly string PING_ROUTE = "ping";
    }
}
