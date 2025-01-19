using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplicationForWarehouseManagementOfOfficeSupplies.Models;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Data;
public class ApplicationDbContext : IdentityDbContext<User, Role, String>
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
    public DbSet<Company> Companies { get; set; }

    // Include UserCompany DbSet
    public DbSet<UserCompany> UserCompanies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure UserCompany Many-to-Many Relationship
        modelBuilder.Entity<UserCompany>(entity =>
        {
            entity.HasKey(uc => new { uc.UserId, uc.CompanyId });

            entity.HasOne(uc => uc.User)
                  .WithMany(u => u.UserCompanies)
                  .HasForeignKey(uc => uc.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(uc => uc.Company)
                  .WithMany(c => c.UserCompanies)
                  .HasForeignKey(uc => uc.CompanyId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Product Configuration
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

        // Request Configuration
        modelBuilder.Entity<Request>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserID);

        modelBuilder.Entity<RequestHistory>()
            .HasOne(rh => rh.Request)
            .WithMany(r => r.RequestHistories)
            .HasForeignKey(rh => rh.RequestId);

        modelBuilder.Entity<RequestHistory>()
            .HasOne(rh => rh.UpdatedBy)
            .WithMany()
            .HasForeignKey(rh => rh.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserID);

        modelBuilder.Entity<Category>().HasData(
       new Category { CategoryID = 1, Name = "Electronics" },
       new Category { CategoryID = 2, Name = "Furniture" },
       new Category { CategoryID = 3, Name = "Stationery" },
       new Category { CategoryID = 4, Name = "Office Supplies" }
       );

    }

    
}
