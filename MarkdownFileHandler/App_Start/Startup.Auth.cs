using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MarkdownFileHandler.Utils;

namespace MarkdownFileHandler
{
    public partial class Startup
    {
       
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions { });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = SettingsHelper.ClientId,
                    Authority = SettingsHelper.Authority,
                    ClientSecret = SettingsHelper.AppKey,
                    ResponseType = "code id_token",
                    Resource = "https://graph.microsoft.com",
                    PostLogoutRedirectUri = "/",
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // instead of using the default validation (validating against a single issuer value, as we do in line of business apps), 
                        // we inject our own multitenant validation logic
                        ValidateIssuer = false,
                        // If the app needs access to the entire organization, then add the logic
                        // of validating the Issuer here.
                        // IssuerValidator
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        SecurityTokenValidated = (context) =>
                        {
                            // If your authentication logic is based on users then add your logic here
                            return Task.FromResult(0);
                        },
                        AuthenticationFailed = (context) =>
                        {
                            // Pass in the context back to the app
                            string message = Uri.EscapeDataString(context.Exception.Message);
                            context.OwinContext.Response.Redirect("/Home/Error?msg=" + message);
                            context.HandleResponse(); // Suppress the exception
                            return Task.FromResult(0);
                        },
                        AuthorizationCodeReceived = async (context) =>
                        {
                            var code = context.Code;
                            ClientCredential credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.AppKey);

                            string tenantID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                            string signInUserId = context.AuthenticationTicket.Identity.FindFirst(AuthHelper.ObjectIdentifierClaim).Value;

                            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(SettingsHelper.Authority, 
                                new AzureTableTokenCache(signInUserId));

                            // Get the access token for AAD Graph. Doing this will also initialize the token cache associated with the authentication context
                            // In theory, you could acquire token for any service your application has access to here so that you can initialize the token cache
                            AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(code,
                                new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)),
                                credential,
                                SettingsHelper.AADGraphResourceId);
                        },
                        RedirectToIdentityProvider = (context) =>
                        {
                            // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                            // this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings
                            // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                            context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;

                            // Save the form in the cookie to prevent it from getting lost in the login redirect
                            CookieStorage.Save(HttpContext.Current.Request.Form);

                            return Task.FromResult(0);
                        }
                    }
                });
        }
        
    }
}
