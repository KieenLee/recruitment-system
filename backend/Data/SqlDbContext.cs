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

        // ✅ User configuration
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
                .HasDefaultValue("HR");

            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);

            // CompanyId is nullable for Admin users
            entity.Property(u => u.CompanyId)
                .IsRequired(false);

            // Unique constraints
            entity.HasIndex(u => u.Email)
                .IsUnique();

            // Relationship with Company (optional)
            entity.HasOne(u => u.Company)
                .WithMany()
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Companies FIRST (được reference bởi JobPostings và Users)
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = 1,
                Name = "Tech Corp",
                Description = "Leading technology company specializing in software development",
                Industry = "Technology",
                Website = "https://techcorp.example.com",
                CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Company
            {
                Id = 2,
                Name = "Finance Solutions Ltd",
                Description = "Premier financial services provider",
                Industry = "Finance",
                Website = "https://financesolutions.example.com",
                CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Company
            {
                Id = 3,
                Name = "Health Plus",
                Description = "Healthcare technology innovator",
                Industry = "Healthcare",
                Website = "https://healthplus.example.com",
                CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            }
        );

        // Seed Users (Admin không cần CompanyId, HR users thuộc Company)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "System Administrator",
                Email = "admin@recruitment.com",
                PasswordHash = "AQAAAAEAACcQAAAAEHashed_Password_Here", // TODO: Hash properly
                Role = "Admin",
                CompanyId = null, //Admin không thuộc company cụ thể
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 2,
                FullName = "HR Manager",
                Email = "hr@techcorp.com",
                PasswordHash = "AQAAAAEAACcQAAAAEHashed_Password_Here", // TODO: Hash properly
                Role = "HR",
                CompanyId = 1, // Thuộc Tech Corp
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 3,
                FullName = "Finance HR",
                Email = "hr@financesolutions.com",
                PasswordHash = "AQAAAAEAACcQAAAAEHashed_Password_Here", // TODO: Hash properly
                Role = "HR",
                CompanyId = 2, //Thuộc Finance Solutions
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed Job Postings (phải sau Companies)
        modelBuilder.Entity<JobPosting>().HasData(
            new JobPosting
            {
                Id = 1,
                Title = "Senior .NET Developer",
                Description = "We are seeking an experienced .NET developer to join our team. You will work on cutting-edge projects using .NET Core, Azure, and microservices architecture.",
                Requirements = "5+ years of experience with .NET Core, C#, SQL Server, and Azure. Strong understanding of design patterns and SOLID principles.",
                Location = "Ho Chi Minh City",
                EmploymentType = "Full-time",
                SalaryMin = 2000,
                SalaryMax = 3500,
                Status = "Open",
                PostedDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                CompanyId = 1,
                CreatedBy = "System",
                CreatedDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new JobPosting
            {
                Id = 2,
                Title = "Frontend React Developer",
                Description = "Join our dynamic team as a Frontend Developer. Work with modern technologies and build responsive, user-friendly web applications.",
                Requirements = "3+ years of experience with React, TypeScript, and modern frontend tools. Experience with Redux, React Query, and testing frameworks.",
                Location = "Hanoi",
                EmploymentType = "Full-time",
                SalaryMin = 1500,
                SalaryMax = 2500,
                Status = "Open",
                PostedDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                CompanyId = 1,
                CreatedBy = "System",
                CreatedDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new JobPosting
            {
                Id = 3,
                Title = "Financial Analyst",
                Description = "Seeking a detail-oriented Financial Analyst to join our team. Analyze financial data, create reports, and provide insights to management.",
                Requirements = "Bachelor's degree in Finance or related field. 2+ years experience in financial analysis. Strong Excel and data visualization skills.",
                Location = "Da Nang",
                EmploymentType = "Full-time",
                SalaryMin = 1200,
                SalaryMax = 2000,
                Status = "Open",
                PostedDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                CompanyId = 2,
                CreatedBy = "System",
                CreatedDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new JobPosting
            {
                Id = 4,
                Title = "DevOps Engineer",
                Description = "Looking for a DevOps Engineer to manage our cloud infrastructure and CI/CD pipelines.",
                Requirements = "Experience with Docker, Kubernetes, Azure/AWS, and CI/CD tools. Strong scripting skills in Bash or PowerShell.",
                Location = "Remote",
                EmploymentType = "Full-time",
                SalaryMin = 2200,
                SalaryMax = 3800,
                Status = "Open",
                PostedDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                CompanyId = 1,
                CreatedBy = "System",
                CreatedDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}