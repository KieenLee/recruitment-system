namespace Backend.DTOs;

public class CreateJobPostingDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public decimal SalaryMin { get; set; }
    public decimal SalaryMax { get; set; }
    public DateTime? ClosingDate { get; set; }
    public int CompanyId { get; set; }
    public int CreatedByUserId { get; set; }
}

public class UpdateJobPostingDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? Status { get; set; }
    public DateTime? ClosingDate { get; set; }
}

public class JobPostingResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public decimal SalaryMin { get; set; }
    public decimal SalaryMax { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime PostedDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int TotalCandidates { get; set; }
}