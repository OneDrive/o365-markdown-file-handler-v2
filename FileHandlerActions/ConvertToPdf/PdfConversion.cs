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

namespace FileHandlerActions
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;

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
            Microsoft.Graph.DriveItem sourceItem = null;
            try
            {
                sourceItem = await HttpHelper.Default.GetMetadataForUrlAsync<Microsoft.Graph.DriveItem>(oneDriveApiSourceUrl, accessToken);
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