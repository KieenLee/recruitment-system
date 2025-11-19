using Backend.Data;
using Backend.DTOs;
using Backend.Models.Mongo;
using MongoDB.Driver;

namespace Backend.Services;

public interface ICandidateService
{
    Task<List<Candidate>> GetCandidatesByJobIdAsync(int jobId);
    Task<Candidate?> GetCandidateByIdAsync(string candidateId);
    Task<Candidate> CreateCandidateAsync(Candidate candidate);
    Task<bool> UpdateCandidateStatusAsync(string candidateId, string status);
    Task<int> GetCandidateCountByJobIdAsync(int jobId);
    Task<string> CreateCandidateAsync(int jobId, ApplyCandidateDto dto, string cvFileName, string cvFilePath);
    Task<bool> UpdateCandidateAiAnalysisAsync(string candidateId, CvAnalysis aiAnalysis);
}

public class CandidateService : ICandidateService
{
    private readonly MongoDbContext _mongoContext;
    private readonly ILogger<CandidateService> _logger;

    public CandidateService(MongoDbContext mongoContext, ILogger<CandidateService> logger)
    {
        _mongoContext = mongoContext;
        _logger = logger;
    }

    public async Task<List<Candidate>> GetCandidatesByJobIdAsync(int jobId)
    {
        try
        {
            var filterBuilder = Builders<Candidate>.Filter;
            var filter = filterBuilder.Eq(c => c.JobId, jobId);

            var candidates = await _mongoContext.Candidates
                .Find(filter)
                .SortByDescending(c => c.UploadedAt)
                .ToListAsync();

            return candidates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting candidates for job {JobId}", jobId);
            return new List<Candidate>();
        }
    }

    public async Task<Candidate?> GetCandidateByIdAsync(string candidateId)
    {
        try
        {
            var candidate = await _mongoContext.Candidates
                .Find(c => c.Id == candidateId)
                .FirstOrDefaultAsync();

            return candidate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting candidate {CandidateId}", candidateId);
            return null;
        }
    }

    public async Task<Candidate> CreateCandidateAsync(Candidate candidate)
    {
        try
        {
            await _mongoContext.Candidates.InsertOneAsync(candidate);
            return candidate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating candidate");
            throw;
        }
    }

    // ✅ New overload method for creating candidate from DTO
    public async Task<string> CreateCandidateAsync(int jobId, ApplyCandidateDto dto, string cvFileName, string cvFilePath)
    {
        try
        {
            var candidate = new Candidate
            {
                JobId = jobId,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                CvFileName = cvFileName,
                CvFilePath = cvFilePath,
                Status = "Pending",
                UploadedAt = DateTime.UtcNow
            };

            await _mongoContext.Candidates.InsertOneAsync(candidate);

            _logger.LogInformation("Created candidate {CandidateId} for job {JobId}", candidate.Id, jobId);

            return candidate.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating candidate for job {JobId}", jobId);
            throw;
        }
    }

    public async Task<bool> UpdateCandidateStatusAsync(string candidateId, string status)
    {
        try
        {
            var filter = Builders<Candidate>.Filter.Eq(c => c.Id, candidateId);
            var update = Builders<Candidate>.Update.Set(c => c.Status, status);

            var result = await _mongoContext.Candidates.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating candidate {CandidateId} status", candidateId);
            return false;
        }
    }

    // ✅ New method to update AI analysis
    public async Task<bool> UpdateCandidateAiAnalysisAsync(string candidateId, CvAnalysis aiAnalysis)
    {
        try
        {
            var filter = Builders<Candidate>.Filter.Eq(c => c.Id, candidateId);
            var update = Builders<Candidate>.Update.Set(c => c.AiAnalysis, aiAnalysis);

            var result = await _mongoContext.Candidates.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                _logger.LogInformation("Updated AI analysis for candidate {CandidateId}", candidateId);
                return true;
            }

            _logger.LogWarning("No candidate found to update AI analysis for ID {CandidateId}", candidateId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating AI analysis for candidate {CandidateId}", candidateId);
            return false;
        }
    }

    public async Task<int> GetCandidateCountByJobIdAsync(int jobId)
    {
        try
        {
            var filter = Builders<Candidate>.Filter.Eq(c => c.JobId, jobId);
            var count = await _mongoContext.Candidates.CountDocumentsAsync(filter);
            return (int)count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting candidates for job {JobId}", jobId);
            return 0;
        }
    }

    // Helper method to map to DTO
    public static CandidateResponseDto MapToDto(Candidate candidate)
    {
        return new CandidateResponseDto
        {
            Id = candidate.Id,
            JobId = candidate.JobId,
            FullName = candidate.FullName,
            Email = candidate.Email,
            Phone = candidate.Phone,
            CvFileName = candidate.CvFileName,
            Status = candidate.Status,
            UploadedAt = candidate.UploadedAt,
            AiAnalysis = candidate.AiAnalysis != null ? new AiAnalysisDto
            {
                OverallScore = candidate.AiAnalysis.OverallScore,
                Summary = candidate.AiAnalysis.Summary,
                CriteriaEvaluation = candidate.AiAnalysis.CriteriaEvaluation.Select(c => new CriterionEvaluationDto
                {
                    Criterion = c.Criterion,
                    IsMet = c.IsMet,
                    Evidence = c.Evidence,
                    Score = c.Score
                }).ToList(),
                ExtractedInformation = new ExtractedInformationDto
                {
                    Name = candidate.AiAnalysis.ExtractedInformation.Name,
                    Email = candidate.AiAnalysis.ExtractedInformation.Email,
                    Phone = candidate.AiAnalysis.ExtractedInformation.Phone,
                    Skills = candidate.AiAnalysis.ExtractedInformation.Skills,
                    Education = candidate.AiAnalysis.ExtractedInformation.Education,
                    Experience = candidate.AiAnalysis.ExtractedInformation.Experience,
                    YearsOfExperience = candidate.AiAnalysis.ExtractedInformation.YearsOfExperience
                },
                RedFlags = candidate.AiAnalysis.RedFlags,
                AnalyzedAt = candidate.AiAnalysis.AnalyzedAt
            } : null
        };
    }
}