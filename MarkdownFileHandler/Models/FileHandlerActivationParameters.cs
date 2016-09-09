using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                        case "resourceId":
                            this.ResourceId = collection[key];
                            break;
                        case "cultureName":
                            this.CultureName = collection[key];
                            break;
                        case "fileGet":
                            this.FileGet = collection[key];
                            break;
                        case "filePut":
                            this.FilePut = collection[key];
                            break;
                        case "fileId":
                            this.FileId = collection[key];
                            break;
                        case "client":
                            this.Client = collection[key];
                            break;
                        case "item":
                            this.ItemUrl = collection[key];
                            break;
                        case "userId":
                            this.UserId = collection[key];
                            break;
                        case "filename":
                            this.Filename = collection[key];
                            break;
                        case "items":
                            if (!string.IsNullOrEmpty(collection[key]))
                                this.ItemUrls = ConvertFromJsonArray<string>(collection[key]).ToArray();
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

        public string ResourceId { get; set; }
        public string CultureName { get; set; }
        public string FileGet { get; set; }
        public string FilePut { get; set; }
        public string FileId { get; set; }

        /// <summary>
        /// The source service that invoked the file handler
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// For a single item action, the OneDrive API URL that can be used to access the item.
        /// </summary>
        public string ItemUrl { get; set; }
        
        /// <summary>
        /// A collection of OneDrive API URLs that can be used to access the items the file handler is being invoked with
        /// </summary>
        public string[] ItemUrls { get; set; }

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

        public bool CanRead
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ItemUrl))
                    return true;

                return !(string.IsNullOrWhiteSpace(this.ResourceId) || string.IsNullOrWhiteSpace(this.FileGet));
            }
        }

        public bool CanWrite
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ItemUrl))
                    return true;

                return CanRead && !string.IsNullOrWhiteSpace(this.FilePut);
            }
        }

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