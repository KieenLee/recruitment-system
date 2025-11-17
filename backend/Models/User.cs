namespace Backend.Models.Sql;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "HR"; // HR, Admin
    public int CompanyId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public Company Company { get; set; } = null!;
    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
}