using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileHandlerActions
{
    internal class HttpHelper
    {
        private const int MAX_UPLOAD_SIZE = 4 * 1024 * 1024;
        private HttpClient httpClient = new HttpClient();

        public static readonly HttpHelper Default = new HttpHelper();

        public async Task<Microsoft.OneDrive.Sdk.Item> UploadFileFromStreamAsync(Stream fileStream, string baseUrl, Microsoft.OneDrive.Sdk.ItemReference folder, string filename, string accessToken = null)
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

        private async Task<T> ParseJsonFromResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.Content.Headers.ContentType.MediaType.ToLower() != "application/json")
            {
                throw new InvalidOperationException($"MediaType for the response message was {response.Content.Headers.ContentType.MediaType} instead of \"application/json\".");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseData);
        }

        public async Task<T> GetMetadataForUrlAsync<T>(string requestUri, string accessToken = null)
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

        public async Task<MemoryStream> GetStreamContentForUrlAsync(string requestUri, string accessToken = null)
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
