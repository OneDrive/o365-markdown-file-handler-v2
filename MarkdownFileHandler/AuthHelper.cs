using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MarkdownFileHandler.Utils;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;

namespace MarkdownFileHandler
{
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

        public static string GetResourceFromUrl(string url)
        {
            Uri uri = new Uri(url);
            return uri.GetLeftPart(UriPartial.Authority);

        }

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