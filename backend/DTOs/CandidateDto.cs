namespace Backend.DTOs;

// DTO for applying as a candidate
public class ApplyCandidateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

// Response DTO for candidate
public class CandidateResponseDto
{
    public string Id { get; set; } = string.Empty;
    public int JobId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CvFileName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public AiAnalysisDto? AiAnalysis { get; set; }
}

// AI Analysis DTO
public class AiAnalysisDto
{
    public double OverallScore { get; set; }
    public string Summary { get; set; } = string.Empty;
    public ExtractedInformationDto ExtractedInformation { get; set; } = new();
    public List<CriterionEvaluationDto> CriteriaEvaluation { get; set; } = new();
    public List<string> RedFlags { get; set; } = new();
    public DateTime AnalyzedAt { get; set; }
}

// Extracted Information DTO
public class ExtractedInformationDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public List<string> Education { get; set; } = new();
    public List<string> Experience { get; set; } = new();
    public int YearsOfExperience { get; set; }
}

// Criterion Evaluation DTO
public class CriterionEvaluationDto
{
    public string Criterion { get; set; } = string.Empty;
    public double Score { get; set; }
    public string IsMet { get; set; } = string.Empty;
    public string Evidence { get; set; } = string.Empty;
}

// Update status DTO
public class UpdateCandidateStatusDto
{
    public string Status { get; set; } = string.Empty; // "Pending", "Approved", "Rejected"
}