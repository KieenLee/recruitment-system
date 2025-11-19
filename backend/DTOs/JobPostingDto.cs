namespace Backend.DTOs;

//DTO for creating a new job posting
public class CreateJobPostingDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = "Full-time";
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string Status { get; set; } = "Open";
    public DateTime? ClosingDate { get; set; }
    public int CompanyId { get; set; }
}

// DTO for updating existing job posting
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

// Request DTO for creating/updating job postings (legacy - can be removed if not used)
public class JobPostingRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string Status { get; set; } = "Open";
    public DateTime? ClosingDate { get; set; }
    public int CompanyId { get; set; }
}

// Response DTO with additional information
public class JobPostingResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime PostedDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int TotalCandidates { get; set; }
}

// List response DTO (simplified)
public class JobPostingListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PostedDate { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int TotalCandidates { get; set; }
}