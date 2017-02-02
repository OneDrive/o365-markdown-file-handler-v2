using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace MarkdownFileHandler.Models
{
    public class FileHandlerActivationParameters
    {
        private NameValueCollection sourceParameters;
        public FileHandlerActivationParameters(NameValueCollection collection)
        {
            sourceParameters = collection;
            this.OtherValues = new Dictionary<string, string>();
            if (collection != null)
            {
                foreach(var key in collection.AllKeys)
                {
                    switch(key)
                    {
                        case "cultureName":
                            this.CultureName = collection[key];
                            break;
                        case "client":
                            this.Client = collection[key];
                            break;
                        case "userId":
                            this.UserId = collection[key];
                            break;
                        case "filename":
                            this.Filename = collection[key];
                            break;
                        case "items":
                            this.Items = collection[key];
                            break;
                        case "content":
                            this.FileContent = collection[key];
                            break;
                        default:
                            this.OtherValues[key] = collection[key];
                            break;
                    }
                }
            }
        }

        internal Dictionary<string, string> ToDictionary()
        {
            var result = new Dictionary<string, string>();
            foreach(var key in sourceParameters.AllKeys)
            {
                result[key] = sourceParameters[key];
            }
            return result;
        }

        /// <summary>
        /// Resource ID for authentication purposes
        /// </summary>
        public string ResourceId { get { return "https://graph.microsoft.com"; } }

        /// <summary>
        /// Locale for the current user's language
        /// </summary>
        public string CultureName { get; set; }

        /// <summary>
        /// The source service that invoked the file handler
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// A collection of Microsoft Graph API URLs that can be used to access the items the file handler is being invoked with
        /// </summary>
        public string Items { get; set; }

        /// <summary>
        /// Helper property to retrieve item urls
        /// </summary>
        public string[] ItemUrls()
        {
            if (!string.IsNullOrEmpty(Items))
                return ConvertFromJsonArray<string>(this.Items).ToArray();
            else
                return new string[0];
        }
        

        /// <summary>
        /// Returns a single Microsoft Graph API URL that can be used to access an item in a single selection scenario.
        /// </summary>
        public string SingleItemUrl()
        {
            return ItemUrls().FirstOrDefault();
        }

        /// <summary>
        /// A unique identifer for the logged in user who invoked the file handler.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The content of a file, used when saving changes back from the client
        /// </summary>
        public string FileContent { get; set; }

        public string Filename { get; set; }

        public Dictionary<string, string> OtherValues { get; private set; }

        public System.Web.Mvc.MvcHtmlString OtherPropertyValues
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach(var p in OtherValues.Keys)
                {
                    sb.Append($"<b>{HttpUtility.HtmlEncode(p)}:</b>&nbsp;\"{HttpUtility.HtmlEncode(OtherValues[p])}\"<br />");
                }

                return new System.Web.Mvc.MvcHtmlString(sb.ToString());
            }

        }

        private static List<T> ConvertFromJsonArray<T>(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(input);
        }
    }
}