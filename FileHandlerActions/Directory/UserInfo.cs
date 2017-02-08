/*
 * Markdown File Handler - Sample Code
 * Copyright (c) Microsoft Corporation
 * All rights reserved. 
 * 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the ""Software""), to deal in 
 * the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace FileHandlerActions.Directory
{
    using System.Threading.Tasks;

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
