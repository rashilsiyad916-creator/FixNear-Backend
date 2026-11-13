using FixNear.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FixNear.Infrastructure.Data
{
    public class FixNearDbContext : DbContext
    {
        public FixNearDbContext(DbContextOptions<FixNearDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Provider> Providers => Set<Provider>();
        public DbSet<ProviderService> ProviderServices => Set<ProviderService>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<CategoryRequest> CategoryRequests => Set<CategoryRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.UserId);
                e.Property(x => x.FullName).HasMaxLength(150).IsRequired();
                e.Property(x => x.Email).HasMaxLength(200).IsRequired();
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.PasswordHash).IsRequired();
                e.Property(x => x.Role).HasMaxLength(20).IsRequired();
            });

            modelBuilder.Entity<Category>(e =>
            {
                e.ToTable("Categories");
                e.HasKey(x => x.CategoryId);
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Provider>(e =>
            {
                e.ToTable("Providers");
                e.HasKey(x => x.ProviderId);
                e.Property(x => x.BusinessName).IsRequired();
                e.Property(x => x.ServiceArea).IsRequired();
                e.Property(x => x.Address).IsRequired();
                e.HasOne(x => x.User).WithMany(u => u.Providers)
                    .HasForeignKey(x => x.UserId).HasConstraintName("FK_Providers_Users_UserId");
            });

            modelBuilder.Entity<ProviderService>(e =>
            {
                e.ToTable("ProviderService");
                e.HasKey(x => x.ProviderServiceId);
                e.Property(x => x.ServiceName).IsRequired();
                e.Property(x => x.ImageUrl).HasMaxLength(500);
                e.Property(x => x.BasePrice).HasColumnType("decimal(18,2)");
                e.HasOne(x => x.Provider).WithMany(p => p.Services)
                    .HasForeignKey(x => x.ProviderId).HasConstraintName("FK_ProviderService_Providers_ProviderId");
                e.HasOne(x => x.Category).WithMany(c => c.Services)
                    .HasForeignKey(x => x.CategoryId).HasConstraintName("FK_ProviderService_Categories_CategoryId");
            });

            modelBuilder.Entity<Booking>(e =>
            {
                e.ToTable("Bookings");
                e.HasKey(x => x.BookingId);
                e.Property(x => x.Address).IsRequired();
                e.Property(x => x.ProblemDescription).IsRequired();
                e.Property(x => x.Status).IsRequired();
                e.HasOne(x => x.Customer).WithMany(u => u.Bookings)
                    .HasForeignKey(x => x.CustomerId).HasConstraintName("FK_Bookings_Users_CustomerId");
                e.HasOne(x => x.ProviderService).WithMany(s => s.Bookings)
                    .HasForeignKey(x => x.ProviderServiceId).HasConstraintName("FK_Bookings_ProviderService_ProviderServiceId");
            });

            modelBuilder.Entity<Review>(e =>
            {
                e.ToTable("Reviews");
                e.HasKey(x => x.ReviewId);
                e.HasOne(x => x.Booking).WithMany(b => b.Reviews)
                    .HasForeignKey(x => x.BookingId).HasConstraintName("FK_Reviews_Bookings_BookingId");
                e.HasOne(x => x.Customer).WithMany(u => u.Reviews)
                    .HasForeignKey(x => x.CustomerId).HasConstraintName("FK_Reviews_Users_CustomerId");
                e.HasOne(x => x.Provider).WithMany(p => p.Reviews)
                    .HasForeignKey(x => x.ProviderId).HasConstraintName("FK_Reviews_Providers_ProviderId");
            });

            modelBuilder.Entity<Favorite>(e =>
            {
                e.ToTable("Favorites");
                e.HasKey(x => new { x.UserId, x.ProviderId });
                e.HasOne(x => x.User).WithMany()
                    .HasForeignKey(x => x.UserId).HasConstraintName("FK_Favorites_Users_UserId");
                e.HasOne(x => x.Provider).WithMany()
                    .HasForeignKey(x => x.ProviderId).HasConstraintName("FK_Favorites_Providers_ProviderId");
            });

            modelBuilder.Entity<CategoryRequest>(e =>
            {
                e.ToTable("CategoryRequests");
                e.HasKey(x => x.CategoryRequestId);
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.Status).HasMaxLength(20).IsRequired();
                e.Property(x => x.AdminNote).HasMaxLength(500);
                e.HasIndex(x => new { x.ProviderId, x.Status });
                e.HasOne(x => x.Provider).WithMany()
                    .HasForeignKey(x => x.ProviderId)
                    .HasConstraintName("FK_CategoryRequests_Providers_ProviderId");
                e.HasOne(x => x.ReviewedBy).WithMany()
                    .HasForeignKey(x => x.ReviewedByUserId)
                    .HasConstraintName("FK_CategoryRequests_Users_ReviewedByUserId");
            });

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                .SelectMany(entityType => entityType.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}

