using MarkdownFileHandler;
using MarkdownFileHandler.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MarkdownFileHandler.Controllers
{
    [Authorize]
    public class EditorController : ApiController
    {
        // POST api/<controller>
        [HttpPost]
        public async Task<IHttpActionResult> SaveContentChanges(string fileUrl, [FromBody]ItemContents contents)
        {
            if (string.IsNullOrEmpty(fileUrl)) return BadRequest();
            if (null == contents) return BadRequest();

            // Generate the resourceId for the MS Graph URL so we know where to request an access token
            Uri target = new Uri(fileUrl);
            string resourceId = target.GetLeftPart(UriPartial.Authority);

            // Retrieve an access token so we can make API calls
            string accessToken = null;
            try
            {
                accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(resourceId);
            }
            catch (Exception ex)
            {
                return Ok(new SaveResults { Success = false, Error = ex.Message });
            }

            // Upload the new file content
            try
            {
                var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(contents.MarkdownText));
                var result = await HttpHelper.Default.UploadFileContentsFromStreamAsync(stream, fileUrl, accessToken);
                return Ok(new SaveResults { Success = true });
            }
            catch (Exception ex)
            {
                return Ok(new SaveResults { Success = false, Error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IHttpActionResult> RenameFile(string fileUrl, string filename)
        {
            if (string.IsNullOrEmpty(fileUrl)) return BadRequest();
            if (string.IsNullOrEmpty(filename)) return BadRequest();

            // Generate the resourceId for the MS Graph URL so we know where to request an access token
            Uri target = new Uri(fileUrl);
            string resourceId = target.GetLeftPart(UriPartial.Authority);

            // Retrieve an access token so we can make API calls
            string accessToken = null;
            try
            {
                accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(resourceId);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Unable to retrieve access token for {resourceId}.", ex));
            }

            // Upload the new file content
            try
            {
                var driveItemMetadata = new { name = filename };
                var updatedItem = await HttpHelper.Default.PatchAsync<Microsoft.Graph.DriveItem>(driveItemMetadata, fileUrl, accessToken);
                return Ok(new SaveResults { Success = true, Filename = updatedItem.Name });
            }
            catch (Exception ex)
            {
                return Ok(new SaveResults { Success = false, Error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IHttpActionResult> CreateSharingLink(string fileUrl, string type, string scope)
        {
            if (string.IsNullOrEmpty(fileUrl)) return BadRequest();
            if (string.IsNullOrEmpty(type)) return BadRequest();
            if (string.IsNullOrEmpty(scope)) return BadRequest();

            // Generate the resourceId for the MS Graph URL so we know where to request an access token
            Uri target = new Uri(fileUrl);
            string resourceId = target.GetLeftPart(UriPartial.Authority);

            // Retrieve an access token so we can make API calls
            string accessToken = null;
            try
            {
                accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(resourceId);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Unable to retrieve access token for {resourceId}.", ex));
            }

            // Upload the new file content
            try
            {
                var createLinkParameters = new { type = type, scope = scope };
                UriBuilder builder = new UriBuilder(fileUrl);
                builder.Path += "/createLink";
                var permission = await HttpHelper.Default.PostAsync<Microsoft.Graph.Permission>(createLinkParameters, builder.ToString(), accessToken);
                return Ok(new ShareLinkResults { Success = true, SharingUrl = permission?.Link?.WebUrl });
            }
            catch (Exception ex)
            {
                return Ok(new SaveResults { Success = false, Error = ex.Message });
            }
        }
    }
}