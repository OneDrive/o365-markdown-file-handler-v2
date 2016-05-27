using System.Configuration;

namespace MarkdownFileHandler
{
    public class SettingsHelper
    {
        private static string _clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string _appKey = ConfigurationManager.AppSettings["ida:ClientSecret"] ?? ConfigurationManager.AppSettings["ida:AppKey"];

        private static string _graphResourceId = ConfigurationManager.AppSettings["ida:GraphResourceId"];
        private static string _authority = ConfigurationManager.AppSettings["ida:AADInstance"];

        private static string _consentUri = _authority + "oauth2/authorize?response_type=code&client_id={0}&resource={1}&redirect_uri={2}";
        private static string _adminConsentUri = _authority + "oauth2/authorize?response_type=code&client_id={0}&resource={1}&redirect_uri={2}&prompt={3}";

        public static string ClientId
        {
            get
            {
                return _clientId;
            }
        }

        public static string AppKey
        {
            get
            {
                return _appKey;
            }
        }

        public static string Authority
        {
            get
            {
                return _authority;
            }
        }

        public static string AADGraphResourceId
        {
            get
            {
                return _graphResourceId;
            }
        }
    }
}