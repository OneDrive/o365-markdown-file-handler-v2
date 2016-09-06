using MarkdownFileHandler.Models;
using MarkdownFileHandler.Utils;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace MarkdownFileHandler.Controllers
{

    //[Authorize]
    public class FileHandlerController : Controller
    {

        /// <summary>
        /// Generate a read-only preview of the file
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Preview()
        {
            var input = LoadActivationParameters();

            if (!input.CanRead)
            {
                return View(new FileHandlerModel(input, null, new MvcHtmlString("Required parameters are missing.")));
            }

            return View(await GetFileHandlerModelAsync(input));
        }

        /// <summary>
        /// Generate a read-write opened version of the file
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Open()
        {
            var input = LoadActivationParameters();

            if (!input.CanWrite)
            {
                return View(new FileHandlerModel(input, null, new MvcHtmlString("Required parameters are missing.")));
            }

            return View(await GetFileHandlerModelAsync(input));
        }


        public async Task<ActionResult> ConvertToPDF()
        {
            var input = LoadActivationParameters();
            FileHandlerActions.PdfConverterJob job = new FileHandlerActions.PdfConverterJob();
            job.Status.OriginalParameters = input.ToDictionary();

            var resourceUrl = AuthHelper.GetResourceFromUrl(input.ItemUrl);
            var accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(resourceUrl);

            HostingEnvironment.QueueBackgroundWorkItem(ct => job.BeginConvertToPdf(input.ItemUrl, accessToken));
            return View(new AsyncActionModel { JobIdentifier = job.Id, Status = job.Status });
        }

        public ActionResult GetAsyncJobStatus(string identifier)
        {
            var job = FileHandlerActions.JobTracker.GetJob(identifier);
            return View("AsyncJobStatus", new AsyncActionModel { JobIdentifier = identifier, Status = job });
        }

        public async Task<ActionResult> CompressFiles()
        {
            var input = LoadActivationParameters();
            return View(new FileHandlerModel(input, null, null));
        }

        public async Task<ActionResult> NewFile()
        {
            var input = LoadActivationParameters();

            if (!input.CanWrite)
            {
                return View(new FileHandlerModel(input, null, new MvcHtmlString("Required parameters are missing.")));
            }

            return View(await GetFileHandlerModelAsync(input));
        }

        /// <summary>
        /// Parse either the POST data or stored cookie data to retrieve the file information from
        /// the request.
        /// </summary>
        /// <returns></returns>
        private FileHandlerActivationParameters LoadActivationParameters()
        {
            FileHandlerActivationParameters activationParameters = null;

            if (Request.Form != null && Request.Form.AllKeys.Count<string>() != 0)
            {
                // Get from current request's form data
                activationParameters = new FileHandlerActivationParameters(Request.Form);
            }
            else
            {
                // If form data does not exist, it must be because of the sign in redirection. 
                // Read the cookie we saved before the redirection in RedirectToIdentityProvider callback in Startup.Auth.cs 
                activationParameters = new FileHandlerActivationParameters(CookieStorage.Load());
                
                // Clear the cookie after using it
                CookieStorage.Clear();
            }

            return activationParameters;
        }


        private async Task<FileHandlerModel> GetFileHandlerModelAsync(FileHandlerActivationParameters input)
        {
            // Retrieve an access token so we can make API calls
            string accessToken = null;
            try
            {
                accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(input.ResourceId);
            }
            catch (Exception ex)
            {
                return new FileHandlerModel(input, null, new MvcHtmlString("Error acquiring access token. Exception: " + ex.ToString()));
            }

            // Get file content
            Stream fileContentStream = null;
            try
            {
                fileContentStream = await GetFileContentAsync(input, accessToken);
            }
            catch (Exception ex)
            {
                return new FileHandlerModel(input, null, new MvcHtmlString("Error downloading file contents. Exception: " + ex.ToString()));
            }

            // Convert the stream into text for rendering
            StreamReader reader = new StreamReader(fileContentStream);
            var fileContentString = await reader.ReadToEndAsync();

            var htmlContent = ConvertMarkdownToHtml(fileContentString);

            return new FileHandlerModel(input, htmlContent, null);
        }


        private string ConvertMarkdownToHtml(string markdown)
        {
            var file = new MarkdownFile(markdown);
            return file.TransformToHtml();
        }

        /// <summary>
        /// Download the contents of the file from the server and return as a stream.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private async Task<Stream> GetFileContentAsync(FileHandlerActivationParameters input, string accessToken)
        {
            // Use the input.FileGet URL to download the contents of the file
            var request = WebRequest.CreateHttp(input.FileGet);
            request.Headers.Add("Authorization", "Bearer " + accessToken);
            request.AllowAutoRedirect = false;

            HttpWebResponse httpResponse = null;

            try
            {
                var response = await request.GetResponseAsync();
                httpResponse = response as HttpWebResponse;
            }
            catch (WebException ex)
            {
                httpResponse = ex.Response as HttpWebResponse;
            }

            if (httpResponse == null)
            {
                throw new WebException("Request was unsuccessful.");
            }

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                MemoryStream responseStream = new MemoryStream();
                await httpResponse.GetResponseStream().CopyToAsync(responseStream);

                // Reset the memory stream
                responseStream.Seek(0, SeekOrigin.Begin);

                return responseStream;
            }
            else
            {
                throw new WebException("Http response had invalid status code: " + httpResponse.StatusCode);
            }
        }

        
    }
}