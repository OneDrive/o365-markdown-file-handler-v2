using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using Microsoft.Graph;

namespace FileHandlerActions.AddToZip
{
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
