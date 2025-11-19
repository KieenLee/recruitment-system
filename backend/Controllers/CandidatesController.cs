using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/jobs/{jobId}/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;
    private readonly IJobPostingService _jobPostingService;
    private readonly ICvParserService _cvParserService;
    private readonly ILlmAnalyzerService _llmAnalyzerService;
    private readonly ILogger<CandidatesController> _logger;

    public CandidatesController(
        ICandidateService candidateService,
        IJobPostingService jobPostingService,
        ICvParserService cvParserService,
        ILlmAnalyzerService llmAnalyzerService,
        ILogger<CandidatesController> logger)
    {
        _candidateService = candidateService;
        _jobPostingService = jobPostingService;
        _cvParserService = cvParserService;
        _llmAnalyzerService = llmAnalyzerService;
        _logger = logger;
    }

    /// <summary>
    /// Get all candidates for a specific job posting
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CandidateResponseDto>>> GetCandidates(int jobId, [FromQuery] string? status = null)
    {
        try
        {
            // Verify job exists
            var job = await _jobPostingService.GetJobPostingByIdAsync(jobId);
            if (job == null)
            {
                return NotFound(new { message = $"Job posting with ID {jobId} not found" });
            }

            var candidates = await _candidateService.GetCandidatesByJobIdAsync(jobId, status);
            return Ok(candidates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting candidates for job {JobId}", jobId);
            return StatusCode(500, new { message = "An error occurred while retrieving candidates" });
        }
    }

    /// <summary>
    /// Get a specific candidate by ID
    /// </summary>
    [HttpGet("{candidateId}")]
    public async Task<ActionResult<CandidateResponseDto>> GetCandidate(int jobId, string candidateId)
    {
        try
        {
            var candidate = await _candidateService.GetCandidateByIdAsync(candidateId);
            if (candidate == null)
            {
                return NotFound(new { message = $"Candidate with ID {candidateId} not found" });
            }

            if (candidate.JobId != jobId)
            {
                return BadRequest(new { message = "Candidate does not belong to this job posting" });
            }

            return Ok(candidate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting candidate {CandidateId} for job {JobId}", candidateId, jobId);
            return StatusCode(500, new { message = "An error occurred while retrieving the candidate" });
        }
    }

    /// <summary>
    /// Apply to a job posting - Upload CV (Endpoint quan trọng nhất - "Cầu nối")
    /// </summary>
    [HttpPost("apply")]
    public async Task<ActionResult<CandidateResponseDto>> ApplyToJob(
        int jobId,
        [FromForm] ApplyCandidateDto dto,
        [FromForm] IFormFile cvFile)
    {
        try
        {
            // Validate job exists
            var job = await _jobPostingService.GetJobPostingByIdAsync(jobId);
            if (job == null)
            {
                return NotFound(new { message = $"Job posting with ID {jobId} not found" });
            }

            // Validate file
            if (cvFile == null || cvFile.Length == 0)
            {
                return BadRequest(new { message = "CV file is required" });
            }

            if (!cvFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Only PDF files are accepted" });
            }

            // Save CV file
            var cvFilePath = await _cvParserService.SaveCvFileAsync(cvFile, jobId);
            var cvFileName = cvFile.FileName;

            // Create candidate record in MongoDB with JobId (CẦU NỐI)
            var candidateId = await _candidateService.CreateCandidateAsync(jobId, dto, cvFileName, cvFilePath);

            // Start background analysis (fire and forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    // Extract text from PDF
                    var cvText = await _cvParserService.ExtractTextFromPdfAsync(cvFilePath);

                    // Analyze with AI
                    var aiAnalysis = await _llmAnalyzerService.AnalyzeCvAsync(cvText, job.Requirements);

                    if (aiAnalysis != null)
                    {
                        // Update candidate with AI analysis
                        await _candidateService.UpdateCandidateAiAnalysisAsync(candidateId, aiAnalysis);
                        _logger.LogInformation("AI analysis completed for candidate {CandidateId}", candidateId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background AI analysis for candidate {CandidateId}", candidateId);
                }
            });

            // Return candidate info immediately
            var candidate = await _candidateService.GetCandidateByIdAsync(candidateId);
            return CreatedAtAction(nameof(GetCandidate), new { jobId, candidateId }, candidate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying to job {JobId}", jobId);
            return StatusCode(500, new { message = "An error occurred while processing your application" });
        }
    }

    /// <summary>
    /// Update candidate status (Approve/Reject)
    /// </summary>
    [HttpPatch("{candidateId}/status")]
    public async Task<IActionResult> UpdateCandidateStatus(
        int jobId,
        string candidateId,
        [FromBody] UpdateCandidateStatusDto dto)
    {
        try
        {
            var candidate = await _candidateService.GetCandidateByIdAsync(candidateId);
            if (candidate == null)
            {
                return NotFound(new { message = $"Candidate with ID {candidateId} not found" });
            }

            if (candidate.JobId != jobId)
            {
                return BadRequest(new { message = "Candidate does not belong to this job posting" });
            }

            var result = await _candidateService.UpdateCandidateStatusAsync(candidateId, dto.Status);
            if (!result)
            {
                return BadRequest(new { message = "Failed to update candidate status" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for candidate {CandidateId}", candidateId);
            return StatusCode(500, new { message = "An error occurred while updating candidate status" });
        }
    }
}

public class UpdateCandidateStatusDto
{
    public string Status { get; set; } = string.Empty; // "Approved", "Rejected", "Pending"
}