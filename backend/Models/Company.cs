namespace Backend.Models.Sql;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
}
