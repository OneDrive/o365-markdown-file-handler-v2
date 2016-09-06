using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHandlerActions.Directory
{
    public class UserInfo
    {
        /// <summary>
        /// Converts a document into a PDF file asynchronously and saves it next to the original file.
        /// </summary>
        /// <param name="sourceFileUrl"></param>
        /// <returns></returns>
        public static async Task<UserInfo> GetUserInfoAsync(string graphUrl, string userObjectId, string accessToken)
        {
            if (!string.IsNullOrEmpty(userObjectId))
            {
                return await HttpHelper.Default.GetMetadataForUrlAsync<UserInfo>($"{graphUrl}/v1.0/users/{userObjectId}", accessToken);
            }

            return null;
        }


        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string JobTitle { get; set; }
        public string Mail { get; set; }
        public string MobilePhone { get; set; }
        public string OfficeLocation { get; set; }
        public string Surname { get; set; }
        public string UserPrincipalName { get; set; }
    }
}
