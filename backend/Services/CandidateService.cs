using Backend.Data;
using Backend.DTOs;
using Backend.Models.Mongo;
using MongoDB.Driver;

namespace Backend.Services;

public interface ICandidateService
{
    Task<string> CreateCandidateAsync(int jobId, ApplyCandidateDto dto, string cvFileName, string cvFilePath);
    Task<CandidateResponseDto?> GetCandidateByIdAsync(string id);
    Task<List<CandidateResponseDto>> GetCandidatesByJobIdAsync(int jobId, string? status = null);
    Task<int> GetCandidateCountByJobIdAsync(int jobId);
    Task<bool> UpdateCandidateStatusAsync(string id, string status);
    Task<bool> UpdateCandidateAiAnalysisAsync(string id, AiAnalysis aiAnalysis);
}

public class CandidateService : ICandidateService
{
    private readonly MongoDbContext _mongoContext;

    public CandidateService(MongoDbContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    public async Task<string> CreateCandidateAsync(int jobId, ApplyCandidateDto dto, string cvFileName, string cvFilePath)
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
        return candidate.Id;
    }

    public async Task<CandidateResponseDto?> GetCandidateByIdAsync(string id)
    {
        var candidate = await _mongoContext.Candidates
            .Find(c => c.Id == id)
            .FirstOrDefaultAsync();

        if (candidate == null) return null;

        return MapToDto(candidate);
    }

    public async Task<List<CandidateResponseDto>> GetCandidatesByJobIdAsync(int jobId, string? status = null)
    {
        var filterBuilder = Builders<Candidate>.Filter;
        var filter = filterBuilder.Eq(c => c.JobId, jobId);

        if (!string.IsNullOrEmpty(status))
        {
            filter &= filterBuilder.Eq(c => c.Status, status);
        }

        var candidates = await _mongoContext.Candidates
            .Find(filter)
            .SortByDescending(c => c.UploadedAt)
            .ToListAsync();

        return candidates.Select(MapToDto).ToList();
    }

    public async Task<int> GetCandidateCountByJobIdAsync(int jobId)
    {
        var filter = Builders<Candidate>.Filter.Eq(c => c.JobId, jobId);
        return (int)await _mongoContext.Candidates.CountDocumentsAsync(filter);
    }

    public async Task<bool> UpdateCandidateStatusAsync(string id, string status)
    {
        var filter = Builders<Candidate>.Filter.Eq(c => c.Id, id);
        var update = Builders<Candidate>.Update.Set(c => c.Status, status);

        var result = await _mongoContext.Candidates.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateCandidateAiAnalysisAsync(string id, AiAnalysis aiAnalysis)
    {
        var filter = Builders<Candidate>.Filter.Eq(c => c.Id, id);
        var update = Builders<Candidate>.Update.Set(c => c.AiAnalysis, aiAnalysis);

        var result = await _mongoContext.Candidates.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    private static CandidateResponseDto MapToDto(Candidate candidate)
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
                    Experience = candidate.AiAnalysis.ExtractedInformation.Experience
                },
                RedFlags = candidate.AiAnalysis.RedFlags,
                AnalyzedAt = candidate.AiAnalysis.AnalyzedAt
            } : null
        };
    }
}