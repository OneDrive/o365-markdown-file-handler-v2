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

namespace MarkdownFileHandler.Controllers
{
    using MarkdownFileHandler;
    using MarkdownFileHandler.Models;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;

    [Authorize]
    public class FileHandlerController : Controller
    {

        #region Methods registered in file handler manifest
        /// <summary>
        /// Generate a read-write editor experience for a new file
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> NewFile()
        {
            var input = GetActivationParameters(Request);
            return View("Open", await GetFileHandlerModelV2Async(input));
        }

        /// <summary>
        /// Generate a read-only preview of the file
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Preview()
        {
            var input = GetActivationParameters(Request);
            return View(await GetFileHandlerModelV2Async(input, false));
        }

        /// <summary>
        /// Generate a read-write opened version of the file
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Open()
        {
            var input = GetActivationParameters(Request);
            var view = View(await GetFileHandlerModelV2Async(input));
            return view;
        }
        
        /// <summary>
        /// Custom action implemented for this file handler. Converts selected files
        /// into archive.zip. 
        /// </summary>
        public async Task<ActionResult> CompressFiles()
        {
            var input = GetActivationParameters(Request);

            var addToZipFile = new MarkdownFileHandler.AddToZip.AddToZipAction();
            MarkdownFileHandler.AsyncJob job = new MarkdownFileHandler.AsyncJob(addToZipFile);
            job.Status.OriginalParameters = input.ToDictionary();

            var accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(input.ResourceId);

            HostingEnvironment.QueueBackgroundWorkItem(ct => job.Begin(input.ItemUrls, accessToken));
            return View("AsyncAction", new AsyncActionModel { JobIdentifier = job.Id, Status = job.Status, Title = "Add to ZIP" });
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            
            var input = GetActivationParameters(filterContext.HttpContext.Request);
            if (input != null)
            {
                // Ensure that the user context for a file handler matches the signed-in user context
                if (input.UserId != User.Identity.Name)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
        }

        #endregion
        
        public ActionResult GetAsyncJobStatus(string identifier)
        {
            var job = MarkdownFileHandler.JobTracker.GetJob(identifier);
            return View("AsyncJobStatus", new AsyncActionModel { JobIdentifier = identifier, Status = job });
        }

        /// <summary>
        /// Parse either the POST data or stored cookie data to retrieve the file information from
        /// the request.
        /// </summary>
        /// <returns></returns>
        public static FileHandlerActivationParameters GetActivationParameters(HttpRequestBase request)
        {
            FileHandlerActivationParameters activationParameters = null;
            if (IsFileHandlerActivationRequest(request, out activationParameters))
            {
                return activationParameters;
            }
            return null;
        }

        public static bool IsFileHandlerActivationRequest(HttpRequestBase request, out FileHandlerActivationParameters activationParameters)
        {
            activationParameters = null;
            if (request.Form != null && request.Form.AllKeys.Any())
            {
                // Get from current request's form data
                activationParameters = new FileHandlerActivationParameters(request.Form);
                return true;
            }
            else
            {
                // If form data does not exist, it must be because of the sign in redirection. 
                // Read the cookie we saved before the redirection in RedirectToIdentityProvider callback in Startup.Auth.cs 
                var persistedRequestData = CookieStorage.Load(request);
                if (null != persistedRequestData)
                {
                    activationParameters = new FileHandlerActivationParameters(persistedRequestData);
                    return true;
                }
            }
            return false;
        }

        public static bool IsFileHandlerActivationRequest(Microsoft.Owin.IOwinRequest request, out FileHandlerActivationParameters activationParameters)
        {
            activationParameters = null;

            var formTask = request.ReadFormAsync();
            formTask.RunSynchronously();
            var formData = formTask.Result;

            if (formData != null && formData.Any())
            {
                // Get from current request's form data
                activationParameters = new FileHandlerActivationParameters(null);
                return true;
            }
            else
            {
                // If form data does not exist, it must be because of the sign in redirection. 
                // Read the cookie we saved before the redirection in RedirectToIdentityProvider callback in Startup.Auth.cs 
                var persistedRequestData = CookieStorage.Load(request.Cookies);
                if (null != persistedRequestData)
                {
                    activationParameters = new FileHandlerActivationParameters(persistedRequestData);
                    return true;
                }
            }
            return false;
        }


        private async Task<MarkdownFileModel> GetFileHandlerModelV2Async(FileHandlerActivationParameters input, bool allowInteractiveLogin = true)
        {
            if (input == null)
            {
                return MarkdownFileModel.GetErrorModel(new FileHandlerActivationParameters(), "No activation parameters were provided.");
            }

            // Retrieve an access token so we can make API calls
            string accessToken = null;
            try
            {
                accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(input.ResourceId, allowInteractiveLogin);
            }
            catch (Exception ex)
            {
                return MarkdownFileModel.GetErrorModel(input, ex);
            }

            // Get file content
            FileData results = null;
            try
            {
                UriBuilder downloadUrlBuilder = new UriBuilder(input.ItemUrls.First());
                downloadUrlBuilder.Path += "/content";

                results = await HttpHelper.Default.DownloadFileAsync(downloadUrlBuilder.ToString(), accessToken);
            }
            catch (Exception ex)
            {
                return MarkdownFileModel.GetErrorModel(input, ex);
            }

            return MarkdownFileModel.GetWriteableModel(input, results.Filename, results.Content);
        }
    }
}