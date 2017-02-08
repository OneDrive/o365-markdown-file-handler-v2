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

namespace FileHandlerActions.AddToZip
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO.Compression;
    using System.IO;
    using Microsoft.Graph;

    public class AddToZipAction : IAsyncJob
    {
        public async Task<string> DoWorkAsync(string[] sourceItemUrls, string accessToken)
        {
            string baseUrl = ActionHelpers.ParseBaseUrl(sourceItemUrls.First());
            DriveItem firstItem = null;
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach(var itemUrl in sourceItemUrls)
                    {
                        // Fetch the metadata and content for this item
                        var sourceItem = await HttpHelper.Default.GetMetadataForUrlAsync<Microsoft.Graph.DriveItem>(itemUrl, accessToken);
                        if (firstItem == null)
                            firstItem = sourceItem;
                        var result = await TryAddItemToArchiveAsync(archive, sourceItem, baseUrl, accessToken);
                        if (!result)
                            System.Diagnostics.Debug.WriteLine($"Error adding item {sourceItem.Name} to the zip archive.");
                    }
                }

                try
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var uploadedItem = await HttpHelper.Default.UploadFileFromStreamAsync(memoryStream, baseUrl, firstItem.ParentReference, "archive.zip", accessToken);
                    return uploadedItem.WebUrl;
                }
                catch (Exception ex)
                {
                    throw new ConverterException(ConverterException.Reason.UploadError, ex);
                }
            }
        }

        private async Task<bool> TryAddItemToArchiveAsync(ZipArchive archive, Microsoft.Graph.DriveItem item, string baseUrl, string accessToken)
        {
            bool failure = false;

            var zipEntry = archive.CreateEntry(item.Name);
            zipEntry.LastWriteTime = item.LastModifiedDateTime.Value;
            using (var entryStream = zipEntry.Open())
            {
                try
                {
                    var itemContentUrl = ActionHelpers.BuildApiUrl(baseUrl, item.ParentReference.DriveId, item.Id, "content");
                    var contentStream = await HttpHelper.Default.GetStreamContentForUrlAsync(itemContentUrl, accessToken);
                    await contentStream.CopyToAsync(entryStream);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error copying data into file stream: " + ex.ToString());
                    failure = true;
                }
            }

            if (failure)
            {
                zipEntry.Delete();
            }

            return !failure;
        }
    }
}
