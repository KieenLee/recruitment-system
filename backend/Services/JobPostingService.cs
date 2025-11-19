using Backend.Data;
using Backend.DTOs;
using Backend.Models.Sql;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public interface IJobPostingService
{
    Task<JobPostingResponseDto?> CreateJobPostingAsync(CreateJobPostingDto dto);
    Task<JobPostingResponseDto?> GetJobPostingByIdAsync(int id);
    Task<List<JobPostingResponseDto>> GetAllJobPostingsAsync(string? status = null);
    Task<JobPostingResponseDto?> UpdateJobPostingAsync(int id, UpdateJobPostingDto dto);
    Task<bool> DeleteJobPostingAsync(int id);
}

public class JobPostingService : IJobPostingService
{
    private readonly SqlDbContext _sqlContext;
    private readonly ICandidateService _candidateService;

    public JobPostingService(SqlDbContext sqlContext, ICandidateService candidateService)
    {
        _sqlContext = sqlContext;
        _candidateService = candidateService;
    }

    public async Task<JobPostingResponseDto?> CreateJobPostingAsync(CreateJobPostingDto dto)
    {
        var jobPosting = new JobPosting
        {
            Title = dto.Title,
            Description = dto.Description,
            Requirements = dto.Requirements,
            Location = dto.Location,
            EmploymentType = dto.EmploymentType,
            SalaryMin = dto.SalaryMin,
            SalaryMax = dto.SalaryMax,
            CompanyId = dto.CompanyId,
            CreatedByUserId = dto.CreatedByUserId,
            ClosingDate = dto.ClosingDate,
            Status = "Open",
            PostedDate = DateTime.UtcNow
        };

        _sqlContext.JobPostings.Add(jobPosting);
        await _sqlContext.SaveChangesAsync();

        return await GetJobPostingByIdAsync(jobPosting.Id);
    }

    public async Task<JobPostingResponseDto?> GetJobPostingByIdAsync(int id)
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

    public async Task<List<JobPostingResponseDto>> GetAllJobPostingsAsync(string? status = null)
    {
        var query = _sqlContext.JobPostings
            .Include(j => j.Company)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(j => j.Status == status);
        }

        var jobPostings = await query.OrderByDescending(j => j.PostedDate).ToListAsync();

        var result = new List<JobPostingResponseDto>();
        foreach (var job in jobPostings)
        {
            var candidateCount = await _candidateService.GetCandidateCountByJobIdAsync(job.Id);
            result.Add(new JobPostingResponseDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                Location = job.Location,
                EmploymentType = job.EmploymentType,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                Status = job.Status,
                PostedDate = job.PostedDate,
                ClosingDate = job.ClosingDate,
                CompanyId = job.CompanyId,
                CompanyName = job.Company.Name,
                TotalCandidates = candidateCount
            });
        }

        return result;
    }

    public async Task<JobPostingResponseDto?> UpdateJobPostingAsync(int id, UpdateJobPostingDto dto)
    {
        var jobPosting = await _sqlContext.JobPostings.FindAsync(id);
        if (jobPosting == null) return null;

        if (!string.IsNullOrEmpty(dto.Title)) jobPosting.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Description)) jobPosting.Description = dto.Description;
        if (!string.IsNullOrEmpty(dto.Requirements)) jobPosting.Requirements = dto.Requirements;
        if (!string.IsNullOrEmpty(dto.Location)) jobPosting.Location = dto.Location;
        if (!string.IsNullOrEmpty(dto.EmploymentType)) jobPosting.EmploymentType = dto.EmploymentType;
        if (!string.IsNullOrEmpty(dto.Status)) jobPosting.Status = dto.Status;
        if (dto.SalaryMin.HasValue) jobPosting.SalaryMin = dto.SalaryMin.Value;
        if (dto.SalaryMax.HasValue) jobPosting.SalaryMax = dto.SalaryMax.Value;
        if (dto.ClosingDate.HasValue) jobPosting.ClosingDate = dto.ClosingDate;

        await _sqlContext.SaveChangesAsync();
        return await GetJobPostingByIdAsync(id);
    }

    public async Task<bool> DeleteJobPostingAsync(int id)
    {
        var jobPosting = await _sqlContext.JobPostings.FindAsync(id);
        if (jobPosting == null) return false;

        _sqlContext.JobPostings.Remove(jobPosting);
        await _sqlContext.SaveChangesAsync();
        return true;
    }
}