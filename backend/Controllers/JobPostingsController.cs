// filepath: d:\Workspace\Project_.Net\recruitment-system\backend\Controllers\JobPostingsController.cs
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobPostingsController : ControllerBase
{
    private readonly IJobPostingService _jobPostingService;
    private readonly ILogger<JobPostingsController> _logger;

    public JobPostingsController(IJobPostingService jobPostingService, ILogger<JobPostingsController> logger)
    {
        _jobPostingService = jobPostingService;
        _logger = logger;
    }

    /// <summary>
    /// Get all job postings
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<JobPostingResponseDto>>> GetAllJobPostings([FromQuery] string? status = null)
    {
        try
        {
            var jobPostings = await _jobPostingService.GetAllJobPostingsAsync(status);
            return Ok(jobPostings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all job postings");
            return StatusCode(500, new { message = "An error occurred while retrieving job postings" });
        }
    }

    /// <summary>
    /// Get a specific job posting by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<JobPostingResponseDto>> GetJobPosting(int id)
    {
        try
        {
            var jobPosting = await _jobPostingService.GetJobPostingByIdAsync(id);
            if (jobPosting == null)
            {
                return NotFound(new { message = $"Job posting with ID {id} not found" });
            }
            return Ok(jobPosting);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job posting with ID {JobId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the job posting" });
        }
    }

    /// <summary>
    /// Create a new job posting
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<JobPostingResponseDto>> CreateJobPosting([FromBody] CreateJobPostingDto dto)
    {
        try
        {
            var jobPosting = await _jobPostingService.CreateJobPostingAsync(dto);
            if (jobPosting == null)
            {
                return BadRequest(new { message = "Failed to create job posting" });
            }
            return CreatedAtAction(nameof(GetJobPosting), new { id = jobPosting.Id }, jobPosting);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job posting");
            return StatusCode(500, new { message = "An error occurred while creating the job posting" });
        }
    }

    /// <summary>
    /// Update an existing job posting
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<JobPostingResponseDto>> UpdateJobPosting(int id, [FromBody] UpdateJobPostingDto dto)
    {
        try
        {
            var jobPosting = await _jobPostingService.UpdateJobPostingAsync(id, dto);
            if (jobPosting == null)
            {
                return NotFound(new { message = $"Job posting with ID {id} not found" });
            }
            return Ok(jobPosting);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job posting with ID {JobId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the job posting" });
        }
    }

    /// <summary>
    /// Delete a job posting
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobPosting(int id)
    {
        try
        {
            var result = await _jobPostingService.DeleteJobPostingAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Job posting with ID {id} not found" });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job posting with ID {JobId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the job posting" });
        }
    }
}