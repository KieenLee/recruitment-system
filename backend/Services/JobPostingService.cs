using Backend.Data;
using Backend.DTOs;
using Backend.Models.Sql;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public interface IJobPostingService
{
    Task<List<JobPostingListDto>> GetAllJobPostingsAsync();
    Task<JobPostingResponseDto?> GetJobPostingByIdAsync(int id);
    Task<JobPostingResponseDto?> CreateJobPostingAsync(CreateJobPostingDto dto);
    Task<JobPostingResponseDto?> UpdateJobPostingAsync(int id, UpdateJobPostingDto dto);
    Task<bool> DeleteJobPostingAsync(int id);
}

public class JobPostingService : IJobPostingService
{
    private readonly SqlDbContext _sqlContext;
    private readonly ICandidateService _candidateService;
    private readonly ILogger<JobPostingService> _logger;

    public JobPostingService(
        SqlDbContext sqlContext,
        ICandidateService candidateService,
        ILogger<JobPostingService> logger)
    {
        _sqlContext = sqlContext;
        _candidateService = candidateService;
        _logger = logger;
    }

    public async Task<List<JobPostingListDto>> GetAllJobPostingsAsync()
    {
        try
        {
            var jobPostings = await _sqlContext.JobPostings
                .Include(j => j.Company)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();

            var result = new List<JobPostingListDto>();

            foreach (var job in jobPostings)
            {
                var candidateCount = await _candidateService.GetCandidateCountByJobIdAsync(job.Id);

                result.Add(new JobPostingListDto
                {
                    Id = job.Id,
                    Title = job.Title,
                    Location = job.Location,
                    EmploymentType = job.EmploymentType,
                    Status = job.Status,
                    PostedDate = job.PostedDate,
                    CompanyName = job.Company.Name,
                    TotalCandidates = candidateCount
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all job postings");
            return new List<JobPostingListDto>();
        }
    }

    public async Task<JobPostingResponseDto?> GetJobPostingByIdAsync(int id)
    {
        try
        {
            var jobPosting = await _sqlContext.JobPostings
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobPosting == null) return null;

            var candidateCount = await _candidateService.GetCandidateCountByJobIdAsync(id);

            return new JobPostingResponseDto
            {
                Id = jobPosting.Id,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                Requirements = jobPosting.Requirements,
                Location = jobPosting.Location,
                EmploymentType = jobPosting.EmploymentType,
                SalaryMin = jobPosting.SalaryMin,
                SalaryMax = jobPosting.SalaryMax,
                Status = jobPosting.Status,
                PostedDate = jobPosting.PostedDate,
                ClosingDate = jobPosting.ClosingDate,
                CompanyId = jobPosting.CompanyId,
                CompanyName = jobPosting.Company.Name,
                TotalCandidates = candidateCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job posting {JobId}", id);
            return null;
        }
    }

    // ✅ Updated method to use CreateJobPostingDto
    public async Task<JobPostingResponseDto?> CreateJobPostingAsync(CreateJobPostingDto dto)
    {
        try
        {
            // Validate company exists
            var company = await _sqlContext.Companies.FindAsync(dto.CompanyId);
            if (company == null)
            {
                _logger.LogWarning("Company with ID {CompanyId} not found", dto.CompanyId);
                return null;
            }

            var jobPosting = new JobPosting
            {
                Title = dto.Title,
                Description = dto.Description,
                Requirements = dto.Requirements,
                Location = dto.Location,
                EmploymentType = dto.EmploymentType,
                SalaryMin = dto.SalaryMin,
                SalaryMax = dto.SalaryMax,
                Status = dto.Status,
                ClosingDate = dto.ClosingDate,
                CompanyId = dto.CompanyId,
                PostedDate = DateTime.UtcNow
            };

            _sqlContext.JobPostings.Add(jobPosting);
            await _sqlContext.SaveChangesAsync();

            // Return response DTO
            return new JobPostingResponseDto
            {
                Id = jobPosting.Id,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                Requirements = jobPosting.Requirements,
                Location = jobPosting.Location,
                EmploymentType = jobPosting.EmploymentType,
                SalaryMin = jobPosting.SalaryMin,
                SalaryMax = jobPosting.SalaryMax,
                Status = jobPosting.Status,
                PostedDate = jobPosting.PostedDate,
                ClosingDate = jobPosting.ClosingDate,
                CompanyId = jobPosting.CompanyId,
                CompanyName = company.Name,
                TotalCandidates = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job posting");
            return null;
        }
    }

    // ✅ Updated method to use UpdateJobPostingDto
    public async Task<JobPostingResponseDto?> UpdateJobPostingAsync(int id, UpdateJobPostingDto dto)
    {
        try
        {
            var jobPosting = await _sqlContext.JobPostings
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobPosting == null)
            {
                _logger.LogWarning("Job posting with ID {JobId} not found", id);
                return null;
            }

            // Update only provided fields (partial update)
            if (!string.IsNullOrEmpty(dto.Title))
                jobPosting.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                jobPosting.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Requirements))
                jobPosting.Requirements = dto.Requirements;

            if (!string.IsNullOrEmpty(dto.Location))
                jobPosting.Location = dto.Location;

            if (!string.IsNullOrEmpty(dto.EmploymentType))
                jobPosting.EmploymentType = dto.EmploymentType;

            if (!string.IsNullOrEmpty(dto.Status))
                jobPosting.Status = dto.Status;

            if (dto.SalaryMin.HasValue)
                jobPosting.SalaryMin = dto.SalaryMin;

            if (dto.SalaryMax.HasValue)
                jobPosting.SalaryMax = dto.SalaryMax;

            if (dto.ClosingDate.HasValue)
                jobPosting.ClosingDate = dto.ClosingDate;

            await _sqlContext.SaveChangesAsync();

            var candidateCount = await _candidateService.GetCandidateCountByJobIdAsync(id);

            return new JobPostingResponseDto
            {
                Id = jobPosting.Id,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                Requirements = jobPosting.Requirements,
                Location = jobPosting.Location,
                EmploymentType = jobPosting.EmploymentType,
                SalaryMin = jobPosting.SalaryMin,
                SalaryMax = jobPosting.SalaryMax,
                Status = jobPosting.Status,
                PostedDate = jobPosting.PostedDate,
                ClosingDate = jobPosting.ClosingDate,
                CompanyId = jobPosting.CompanyId,
                CompanyName = jobPosting.Company.Name,
                TotalCandidates = candidateCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job posting {JobId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteJobPostingAsync(int id)
    {
        try
        {
            var jobPosting = await _sqlContext.JobPostings.FindAsync(id);
            if (jobPosting == null)
            {
                _logger.LogWarning("Job posting with ID {JobId} not found", id);
                return false;
            }

            _sqlContext.JobPostings.Remove(jobPosting);
            await _sqlContext.SaveChangesAsync();

            _logger.LogInformation("Deleted job posting with ID {JobId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job posting {JobId}", id);
            return false;
        }
    }
}