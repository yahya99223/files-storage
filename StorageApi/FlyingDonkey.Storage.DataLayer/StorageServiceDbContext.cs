using Microsoft.EntityFrameworkCore;

namespace FlyingDonkey.Storage.DataLayer
{
   public class StorageServiceDbContext:DbContext
    {
        public StorageServiceDbContext(DbContextOptions<StorageServiceDbContext> options) : base(options)
        {
        }

        public virtual DbSet<FileInfoRecord> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder x)
        {
        }
    }
}
