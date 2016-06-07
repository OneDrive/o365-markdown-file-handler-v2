using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage.Table;

namespace MarkdownFileHandler.Utils
{
    public class AzureTableTokenCache : TokenCache
    {

        private AzureTableContext tables = new AzureTableContext();
        string User;
        TokenCacheEntity CachedEntity;

        public AzureTableTokenCache(string user)
        {
            this.User = user;
            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification;
            this.BeforeWrite = BeforeWriteNotification;

            CachedEntity = LoadPersistedCacheEntry();
            this.Deserialize((CachedEntity == null) ? null : CachedEntity.CacheBits);
        }

        public override void Clear()
        {
            base.Clear();

            var entry = LoadPersistedCacheEntry();
            if (null != entry)
            {
                TableOperation delete = TableOperation.Delete(entry);
                tables.UserTokenCacheTable.Execute(delete);
            }
            CachedEntity = null;
        }

        private TokenCacheEntity LoadPersistedCacheEntry()
        {
            TableOperation retrieve = TableOperation.Retrieve<TokenCacheEntity>(TokenCacheEntity.PartitionKeyValue, User);
            TableResult results = tables.UserTokenCacheTable.Execute(retrieve);
            var persistedEntry = (TokenCacheEntity)results.Result;
            return persistedEntry;
        }

        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            // Look up the persisted entry
            var persistedEntry = LoadPersistedCacheEntry();

            if (CachedEntity == null)
            {
                // first time access
                CachedEntity = persistedEntry;
            }
            else {
                // if the in-memory copy is older than the persistent copy
                if (persistedEntry != null && persistedEntry.LastWrite > CachedEntity.LastWrite)
                {
                //// read from from storage, update in-memory copy
                CachedEntity = persistedEntry;
                }
            }

            this.Deserialize((null != CachedEntity) ? CachedEntity.CacheBits : null);
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (this.HasStateChanged)
            {
                if (CachedEntity == null)
                {
                    CachedEntity = new TokenCacheEntity();
                }
                CachedEntity.RowKey = User;
                CachedEntity.CacheBits = this.Serialize();
                CachedEntity.LastWrite = DateTime.Now;

                TableOperation insert = TableOperation.InsertOrReplace(CachedEntity);
                tables.UserTokenCacheTable.Execute(insert);
                this.HasStateChanged = false;
            }
        }

        private void BeforeWriteNotification(TokenCacheNotificationArgs args)
        {

        }

    }
}