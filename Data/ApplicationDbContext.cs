using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<ActionHistory> ActionsHistory { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestHistory> RequestHistories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserRole many-to-many relationship
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId);
            });

            // Configure Products table
            modelBuilder.Entity<Product>()
                .Property(p => p.DeliveryPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Row)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.Section)
                .IsRequired();

            // Configure Request relationship
            modelBuilder.Entity<Request>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserID);

            // Configure RequestHistory relationship
            modelBuilder.Entity<RequestHistory>()
                .HasOne(rh => rh.Request)
                .WithMany(r => r.RequestHistories)
                .HasForeignKey(rh => rh.RequestId);

            modelBuilder.Entity<RequestHistory>()
                .HasOne(rh => rh.UpdatedBy)
                .WithMany()
                .HasForeignKey(rh => rh.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure AuditLog relationship
            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserID);
        }
    }
}