using Microsoft.EntityFrameworkCore;
using Test_Ef_Relationship.Repository;

namespace Test_Ef_Relationship
{
    public class ComputerContext : DbContext
    {
        protected ComputerContext()
        {
        }

        public ComputerContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DbComputerEntity> ComputerEntities { get; set; }
        public DbSet<DbDiskEntity> DiskEntities { get; set; }
        public DbSet<DbNicEntity> NicEntities { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost;" +
                "Database=testdatabase;" +
                "User Id=sa;" +
                "Password=sup3rpassword<3<3"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbNicEntity>();
        }
    }
}