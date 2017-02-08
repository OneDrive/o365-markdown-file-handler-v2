using System;
using System.Web;

namespace FileHandlerActions
{
    public static class ActionHelpers
    {
        /// <summary>
        /// Returns an access token from a URL string, if one is available
        /// </summary>
        /// <param name="oneDriveApiSourceUrl"></param>
        /// <returns></returns>
        public static string ParseAccessToken(string oneDriveApiSourceUrl)
        {
            UriBuilder builder = new UriBuilder(oneDriveApiSourceUrl);
            var queryString = builder.Query;
            var values = HttpUtility.ParseQueryString(queryString);
            return values["access_token"];
        }

        /// <summary>
        /// Trims the API URL at /drive to return the base URL we can use to build other API calls
        /// </summary>
        /// <param name="oneDriveApiSourceUrl"></param>
        /// <returns></returns>
        public static string ParseBaseUrl(string oneDriveApiSourceUrl)
        {
            var trimPoint = oneDriveApiSourceUrl.IndexOf("/drive");
            return oneDriveApiSourceUrl.Substring(0, trimPoint);
        }


        public static string BuildApiUrl(string baseUrl, string driveId, string itemId, string extra = "")
        {
            return $"{baseUrl}/drives/{driveId}/items/{itemId}/{extra}";
        }
    }
}
