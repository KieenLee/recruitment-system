namespace Backend.DTOs;

public class ApplyCandidateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

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

public class AiAnalysisDto
{
    public int OverallScore { get; set; }
    public string Summary { get; set; } = string.Empty;
    public List<CriterionEvaluationDto> CriteriaEvaluation { get; set; } = new();
    public ExtractedInformationDto ExtractedInformation { get; set; } = new();
    public List<string> RedFlags { get; set; } = new();
    public DateTime AnalyzedAt { get; set; }
}

public class CriterionEvaluationDto
{
    public string Criterion { get; set; } = string.Empty;
    public string IsMet { get; set; } = string.Empty;
    public string Evidence { get; set; } = string.Empty;
    public int Score { get; set; }
}

public class ExtractedInformationDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public List<string> Education { get; set; } = new();
    public List<string> Experience { get; set; } = new();
}