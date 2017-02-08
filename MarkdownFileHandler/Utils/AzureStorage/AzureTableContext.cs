using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace MarkdownFileHandler.Utils
{

    public class AzureTableContext
    {
        private CloudStorageAccount StorageAccount
        {
            get { return CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString")); }
        }

        private readonly CloudTableClient client;

        public readonly CloudTable UserTokenCacheTable;
        

        public AzureTableContext()
        {
            client = this.StorageAccount.CreateCloudTableClient();
            UserTokenCacheTable = client.GetTableReference("tokenCache");
            UserTokenCacheTable.CreateIfNotExists();
        }
    }


    public class TokenCacheEntity : TableEntity
    {
        public const string PartitionKeyValue = "tokenCache";
        public TokenCacheEntity()
        {
            this.PartitionKey = PartitionKeyValue;
        }

        public byte[] CacheBits { get; set; }
        public DateTime LastWrite { get; set; }
    }
    
}