using Microsoft.EntityFrameworkCore;

namespace Assessment.ORM
{
    public class EFModel : DbContext
    {
        public EFModel(DbContextOptions options) : base (options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataItem>().ToTable("DateItems").HasMany(c => c.quotes).WithOne().HasForeignKey(c => c.DataItemId);
            modelBuilder.Entity<Fiat>().ToTable("Fiat");
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<DataItem> DataItems { get; set; }
        public DbSet<Fiat> Fitats { get; set; }


    }
}
