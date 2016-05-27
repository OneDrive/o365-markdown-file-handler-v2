using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Script.Serialization;

namespace MarkdownFileHandler
{
    public class CookieStorage
    {
        private const string CookieName = "FileHandlerActivationParameters";

        public static NameValueCollection Load()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];

            if (cookie != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new NameValueCollectionConverter() });
                return serializer.Deserialize<NameValueCollection>(cookie.Value);
            }
            else
            {
                return null;
            }
        }

        public static void Save(NameValueCollection collection)
        {
            HttpCookie cookie = new HttpCookie(CookieName);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new NameValueCollectionConverter() });

            cookie.Value = serializer.Serialize(collection);
            cookie.Expires = DateTime.Now.AddMinutes(10);

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void Clear()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        private class NameValueCollectionConverter : JavaScriptConverter
        {
            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    return new List<Type>
                    {
                        typeof(NameValueCollection)
                    };
                }
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                NameValueCollection collection = obj as NameValueCollection;
                Dictionary<string, object> result = new Dictionary<string, object>();

                if (collection != null)
                {
                    foreach (string key in collection.AllKeys)
                    {
                        result.Add(key, collection[key]);
                    }
                }

                return result;
            }

            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException("dictionary");
                }

                if (type == typeof(NameValueCollection))
                {
                    NameValueCollection collection = new NameValueCollection();

                    foreach (string key in dictionary.Keys)
                    {
                        collection.Add(key, dictionary[key] as string);
                    }

                    return collection;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}