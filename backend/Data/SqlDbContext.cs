using Backend.Models.Sql;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class SqlDbContext : DbContext
{
    public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<JobPosting> JobPostings { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Company configuration
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.Description)
                .HasMaxLength(500);

            entity.Property(c => c.Industry)
                .HasMaxLength(200);

            entity.Property(c => c.Website)
                .HasMaxLength(200);

            entity.Property(c => c.Logo)
                .HasMaxLength(500);

            entity.Property(c => c.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(c => c.CreatedBy)
                .HasMaxLength(100);

            entity.Property(c => c.UpdatedBy)
                .HasMaxLength(100);

            // Relationship
            entity.HasMany(c => c.JobPostings)
                .WithOne(j => j.Company)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // JobPosting configuration
        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(j => j.Id);

            entity.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(j => j.Description)
                .IsRequired();

            entity.Property(j => j.Requirements)
                .IsRequired();

            entity.Property(j => j.Location)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(j => j.EmploymentType)
                .HasMaxLength(50)
                .HasDefaultValue("Full-time");

            entity.Property(j => j.SalaryMin)
                .HasColumnType("decimal(18,2)");

            entity.Property(j => j.SalaryMax)
                .HasColumnType("decimal(18,2)");

            entity.Property(j => j.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");

            entity.Property(j => j.PostedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            // ✅ Audit fields configuration
            entity.Property(j => j.CreatedBy)
                .HasMaxLength(100);

            entity.Property(j => j.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(j => j.UpdatedBy)
                .HasMaxLength(100);

            // Indexes for better query performance
            entity.HasIndex(j => j.Status);
            entity.HasIndex(j => j.PostedDate);
            entity.HasIndex(j => j.CompanyId);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Candidate");

            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Unique constraints
            entity.HasIndex(u => u.FullName).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Companies
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = 1,
                Name = "Tech Corp",
                Description = "Leading technology company specializing in software development",
                Industry = "Technology",
                Website = "https://techcorp.example.com",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Company
            {
                Id = 2,
                Name = "Finance Solutions Ltd",
                Description = "Premier financial services provider",
                Industry = "Finance",
                Website = "https://financesolutions.example.com",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Company
            {
                Id = 3,
                Name = "Health Plus",
                Description = "Healthcare technology innovator",
                Industry = "Healthcare",
                Website = "https://healthplus.example.com",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            }
        );

        // Seed Job Postings
        modelBuilder.Entity<JobPosting>().HasData(
            new JobPosting
            {
                Id = 1,
                Title = "Senior .NET Developer",
                Description = "We are seeking an experienced .NET developer to join our team...",
                Requirements = "5+ years of experience with .NET Core, C#, SQL Server, and Azure",
                Location = "Ho Chi Minh City",
                EmploymentType = "Full-time",
                SalaryMin = 2000,
                SalaryMax = 3500,
                Status = "Open",
                PostedDate = DateTime.UtcNow,
                CompanyId = 1,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            },
            new JobPosting
            {
                Id = 2,
                Title = "Frontend React Developer",
                Description = "Join our dynamic team as a Frontend Developer...",
                Requirements = "3+ years of experience with React, TypeScript, and modern frontend tools",
                Location = "Hanoi",
                EmploymentType = "Full-time",
                SalaryMin = 1500,
                SalaryMax = 2500,
                Status = "Open",
                PostedDate = DateTime.UtcNow,
                CompanyId = 1,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            },
            new JobPosting
            {
                Id = 3,
                Title = "Financial Analyst",
                Description = "Seeking a detail-oriented Financial Analyst...",
                Requirements = "Bachelor's degree in Finance, 2+ years experience in financial analysis",
                Location = "Da Nang",
                EmploymentType = "Full-time",
                SalaryMin = 1200,
                SalaryMax = 2000,
                Status = "Open",
                PostedDate = DateTime.UtcNow,
                CompanyId = 2,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            }
        );

        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "admin",
                Email = "admin@recruitment.com",
                PasswordHash = "AQAAAAEAACcQAAAAEHashed_Password_Here", // This should be properly hashed
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                FullName = "hr_manager",
                Email = "hr@recruitment.com",
                PasswordHash = "AQAAAAEAACcQAAAAEHashed_Password_Here",
                Role = "HR",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}