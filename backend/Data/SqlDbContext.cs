using Microsoft.EntityFrameworkCore;
using Backend.Models.Sql;

namespace Backend.Data;

public class SqlDbContext : DbContext
{
    public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<JobPosting> JobPostings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Company entity
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Industry).HasMaxLength(100);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.HasIndex(e => e.Name);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            
            entity.HasIndex(e => e.Email).IsUnique();
            
            // Configure relationship with Company
            entity.HasOne(e => e.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure JobPosting entity
        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Requirements).IsRequired();
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.EmploymentType).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SalaryMin).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SalaryMax).HasColumnType("decimal(18,2)");

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PostedDate);

            // Configure relationship with Company
            entity.HasOne(e => e.Company)
                .WithMany(c => c.JobPostings)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with User (CreatedBy)
            entity.HasOne(e => e.CreatedBy)
                .WithMany(u => u.JobPostings)
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}