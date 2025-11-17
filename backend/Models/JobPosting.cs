namespace Backend.Models.Sql;

public class JobPosting
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty; // Full-time, Part-time, Contract
    public decimal SalaryMin { get; set; }
    public decimal SalaryMax { get; set; }
    public string Status { get; set; } = "Open"; // Open, Closed, Draft
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ClosingDate { get; set; }
    
    // Foreign Keys
    public int CompanyId { get; set; }
    public int CreatedByUserId { get; set; }
    
    // Navigation Properties
    public Company Company { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
}