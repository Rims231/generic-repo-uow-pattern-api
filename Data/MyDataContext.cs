using generic_repo_uow_pattern_api.Entity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;


namespace generic_repo_uow_pattern_api.Data
{
   

        public class MyDbContext : DbContext
        {
            public DbSet<Product> Products { get; set; }

            public DbSet<Order> Orders { get; set; }

            public DbSet<Blog> Blogs { get; set; }

            public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Product>()
                    .HasMany(p => p.Orders)
                    .WithOne(o => o.Product)
                    .HasForeignKey(o => o.ProductId);

                modelBuilder.Entity<Product>()
                    .Property(p => p.Price)
                    .HasPrecision(18, 2);
            }
        }
    }

