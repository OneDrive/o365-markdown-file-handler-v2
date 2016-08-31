using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace MarkdownFileHandler.Models
{
    public class FileHandlerActivationParameters
    {
        public FileHandlerActivationParameters(NameValueCollection collection)
        {
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
                        default:
                            this.OtherValues[key] = collection[key];
                            break;
                    }
                }
            }
        }

        public string ResourceId { get; set; }
        public string CultureName { get; set; }
        public string FileGet { get; set; }
        public string FilePut { get; set; }
        public string FileId { get; set; }
        public string Client { get; set; }
        public Dictionary<string, string> OtherValues { get; private set; }

        public bool CanRead
        {
            get { return !(string.IsNullOrWhiteSpace(this.ResourceId) || string.IsNullOrWhiteSpace(this.FileGet)); }
        }

        public bool CanWrite
        {
            get { return CanRead && !(string.IsNullOrWhiteSpace(this.FilePut)); }
        }

        public System.Web.Mvc.MvcHtmlString OtherPropertyValues
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach(var p in OtherValues.Keys)
                {
                    sb.Append($"<b>{p}:</b>&nbsp;\"{OtherValues[p]}\"<br />");
                }

                return new System.Web.Mvc.MvcHtmlString(sb.ToString());
            }

        }
    }
}