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
    public class PdfConversion : IAsyncJob
    {

        /// <summary>
        /// Converts a document into a PDF file asynchronously and saves it next to the original file.
        /// </summary>
        /// <param name="sourceFileUrl"></param>
        /// <returns></returns>
        public async Task<string> DoWorkAsync(string[] sourceUrls, string accessToken = null)
        {
            var oneDriveApiSourceUrl = sourceUrls.Single();
            var baseUrl = ActionHelpers.ParseBaseUrl(oneDriveApiSourceUrl);

            // Make an HTTP GET request to oneDriveApiSourceUrl to get item metadata (Microsoft.OneDrive.Sdk.Item)
            Microsoft.OneDrive.Sdk.Item sourceItem = null;
            try
            {
                sourceItem = await HttpHelper.Default.GetMetadataForUrlAsync<Microsoft.OneDrive.Sdk.Item>(oneDriveApiSourceUrl, accessToken);
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

            // If we don't have an accessToken yet, we need one from here on out. Try to reuse one from the first URL.
            if (string.IsNullOrEmpty(accessToken))
            {
                // Parse out the access_token value from the URL, if one exists
                accessToken = ActionHelpers.ParseAccessToken(oneDriveApiSourceUrl);
            }

            Stream pdfStream = null;
            // Fetch the PDF version of the item
            try
            {
                var pdfStreamUrl = ActionHelpers.BuildApiUrl(baseUrl, sourceItem.ParentReference.DriveId, sourceItem.Id, "content?format=pdf");
                pdfStream = await HttpHelper.Default.GetStreamContentForUrlAsync(pdfStreamUrl, accessToken);
            }
            catch (Exception ex)
            {
                throw new ConverterException(ConverterException.Reason.SourceFormatNotAcceptable, ex);
            }


            // Save the PDF file back to the server in the same folder, but with a unique name
            try
            {
                var uploadedItem = await HttpHelper.Default.UploadFileFromStreamAsync(pdfStream, baseUrl, sourceItem.ParentReference, Path.GetFileNameWithoutExtension(originalFilename) + ".pdf", accessToken);
                return uploadedItem.WebUrl;
            }
            catch (Exception ex)
            {
                throw new ConverterException(ConverterException.Reason.UploadError, ex);
            }
        }




        



    }
}