using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Microsoft.OneDrive.Sdk;

namespace FileHandlerActions
{
    public class PdfConversion
    {
        private const int MAX_UPLOAD_SIZE = 4 * 1024 * 1024;
        private HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Converts a document into a PDF file asynchronously and saves it next to the original file.
        /// </summary>
        /// <param name="sourceFileUrl"></param>
        /// <returns></returns>
        public async Task ConvertFileToPdfAsync(string oneDriveApiSourceUrl, string accessToken = null)
        {
            var baseUrl = ParseBaseUrl(oneDriveApiSourceUrl);

            // Make an HTTP GET request to oneDriveApiSourceUrl to get item metadata (Microsoft.OneDrive.Sdk.Item)
            Microsoft.OneDrive.Sdk.Item sourceItem = null;
            try
            {
                sourceItem = await GetMetadataForUrlAsync<Microsoft.OneDrive.Sdk.Item>(oneDriveApiSourceUrl);
            }
            catch (Exception ex)
            {
                throw new ConverterException(ConverterException.Reason.UnableToFetchOriginalItem, ex);
            }

            var originalFilename = sourceItem.Name;
            var containingFolder = sourceItem.ParentReference.Id;

            // Check to make sure the selected item was a file
            if (sourceItem.File == null)
            {
                throw new ConverterException(ConverterException.Reason.ItemNotAFile);
            }

            Stream pdfStream = null;
            // Fetch the PDF version of the item
            try
            {
                var pdfStreamUrl = baseUrl + $"/drives/{sourceItem.ParentReference.DriveId}/items/{sourceItem.Id}/content?format=pdf";
                pdfStream = await GetStreamContentForUrlAsync(pdfStreamUrl);
            }
            catch (Exception ex)
            {
                throw new ConverterException(ConverterException.Reason.SourceFormatNotAcceptable, ex);
            }


            // Save the PDF file back to the server in the same folder, but with a unique name
            try
            {
                await UploadFileFromStreamAsync(pdfStream, baseUrl, sourceItem.ParentReference, Path.GetFileNameWithoutExtension(originalFilename) + ".pdf");
            }
            catch (Exception ex)
            {
                throw new ConverterException(ConverterException.Reason.UploadError, ex);
            }
        }

        private async Task<Microsoft.OneDrive.Sdk.Item> UploadFileFromStreamAsync(Stream fileStream, string baseUrl, ItemReference folder, string filename, string accessToken = null)
        {
            if (fileStream.Length > MAX_UPLOAD_SIZE)
            {
                throw new Exception("File stream is longer than allowed for simple PUT upload action.");
            }

            var requestUrl = $"{baseUrl}/drives/{folder.DriveId}/items/{folder.Id}:/{filename}:/content";
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUrl);

            if (!string.IsNullOrEmpty(accessToken))
            {
                requestMessage.Headers.TryAddWithoutValidation("Authorization", "Bearer " + accessToken);
            }

            var content = new StreamContent(fileStream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            requestMessage.Content = content;

            var responseMessage = await httpClient.SendAsync(requestMessage);
            responseMessage.EnsureSuccessStatusCode();

            return await ParseJsonFromResponseAsync<Microsoft.OneDrive.Sdk.Item>(responseMessage);
        }

        /// <summary>
        /// Trims the API URL at /drives/ to return the base URL we can use to build other API calls
        /// </summary>
        /// <param name="oneDriveApiSourceUrl"></param>
        /// <returns></returns>
        private string ParseBaseUrl(string oneDriveApiSourceUrl)
        {
            var trimPoint = oneDriveApiSourceUrl.IndexOf("/drives/");
            return oneDriveApiSourceUrl.Substring(0, trimPoint);
        }

        private async Task<T> ParseJsonFromResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.Content.Headers.ContentType.MediaType.ToLower() != "application/json")
            {
                throw new InvalidOperationException($"MediaType for the response message was {response.Content.Headers.ContentType.MediaType} instead of \"application/json\".");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseData);
        }

        private async Task<T> GetMetadataForUrlAsync<T>(string requestUri, string accessToken = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            if (!string.IsNullOrEmpty(accessToken))
            {
                requestMessage.Headers.TryAddWithoutValidation("Authorization", "Bearer " + accessToken);
            }

            var responseMessage = await httpClient.SendAsync(requestMessage);

            responseMessage.EnsureSuccessStatusCode();
            return await ParseJsonFromResponseAsync<T>(responseMessage);
        }

        private async Task<MemoryStream> GetStreamContentForUrlAsync(string requestUri, string accessToken = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            if (!string.IsNullOrEmpty(accessToken))
            {
                requestMessage.Headers.TryAddWithoutValidation("Authorization", "Bearer " + accessToken);
            }

            var responseMessage = await httpClient.SendAsync(requestMessage);

            if (responseMessage.IsSuccessStatusCode)
            {
                MemoryStream ms = new MemoryStream();
                var responseStream = await responseMessage.Content.ReadAsStreamAsync();
                await responseStream.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }

            return null;

        }


    }
}