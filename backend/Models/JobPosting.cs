using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Sql;

[Table("JobPostings")]
public class JobPosting
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Requirements { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(50)]
    public string EmploymentType { get; set; } = "Full-time";

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalaryMin { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalaryMax { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Open"; // Open, Closed, Draft

    public DateTime PostedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ClosingDate { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    // Foreign key
    public int CompanyId { get; set; }

    [ForeignKey("CompanyId")]
    public Company Company { get; set; } = null!;
}