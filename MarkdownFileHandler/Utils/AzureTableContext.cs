using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        private readonly CloudTable userTokenCacheTable;
        

        public AzureTableContext()
        {
            client = this.StorageAccount.CreateCloudTableClient();
            userTokenCacheTable = client.GetTableReference("tokenCache");
            userTokenCacheTable.CreateIfNotExists();
        }
    }


    public class UserTokenCacheAzure : TableEntity, IUserTokenCacheEntry
    {
        public UserTokenCacheAzure()
        {
            this.PartitionKey = "tokenCache";
        }
        public int UserTokenCacheId {
            get
            {
                return Int32.Parse(this.RowKey);
            }
            set
            {
                this.RowKey = value.ToString();
            }
        }
        public string WebUserUniqueId { get; set; }
        public byte[] CacheBits { get; set; }
        public DateTime LastWrite { get; set; }
    }
    
}