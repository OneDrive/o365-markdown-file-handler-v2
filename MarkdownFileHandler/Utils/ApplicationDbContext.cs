using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MarkdownFileHandler
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserTokenCache> UserTokenCacheList { get; set; }
    }


    public interface IUserTokenCacheEntry
    {
        int UserTokenCacheId { get; set; }
        string WebUserUniqueId { get; set; }
        byte[] CacheBits { get; set; }
        DateTime LastWrite { get; set; }
    }

    public class UserTokenCache : IUserTokenCacheEntry
    {
        [Key]
        public int UserTokenCacheId { get; set; }
        public string WebUserUniqueId { get; set; }
        public byte[] CacheBits { get; set; }
        public DateTime LastWrite { get; set; }
    }
}