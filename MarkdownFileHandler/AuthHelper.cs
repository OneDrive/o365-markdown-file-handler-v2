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

namespace MarkdownFileHandler
{
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Utils;

    public static class AuthHelper
    {
        public const string ObjectIdentifierClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        /// <summary>
        /// Silently retrieve a new access token for the specified resource. If the request fails, null is returned.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static async Task<string> GetUserAccessTokenSilentAsync(string resource)
        {
            string signInUserId = GetUserId();
            if (!string.IsNullOrEmpty(signInUserId))
            {
                var authContext = new AuthenticationContext(SettingsHelper.Authority, false, new AzureTableTokenCache(signInUserId));

                var clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.AppKey);

                try
                {
                    var authResult = await authContext.AcquireTokenSilentAsync(
                        resource,
                        clientCredential,
                        new UserIdentifier(signInUserId, UserIdentifierType.UniqueId));
                    return authResult.AccessToken;
                }
                catch (AdalSilentTokenAcquisitionException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error retriving token silently: {ex.ToString()}");
                }
            }
            return null;
        }

        //public static string GetResourceFromUrl(string url)
        //{
        //    Uri uri = new Uri(url);
        //    return uri.GetLeftPart(UriPartial.Authority);
        //}

        public static string GetUserId()
        {
            var claim = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(ObjectIdentifierClaim);
            if (null != claim)
            {
                return claim.Value;
            }
                
            return null;
        }
    }
}