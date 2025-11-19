using System.Text;
using System.Text.Json;
using Backend.Models.Mongo; // ✅ Import namespace chứa CvAnalysis

namespace Backend.Services;

public interface ILlmAnalyzerService
{
    Task<CvAnalysis> AnalyzeCvAsync(string cvText, string jobDescription, string jobRequirements);
}

public class LlmAnalyzerService : ILlmAnalyzerService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LlmAnalyzerService> _logger;
    private readonly string _apiUrl;
    private readonly string _apiKey;

    public LlmAnalyzerService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<LlmAnalyzerService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiUrl = configuration["OpenAI:ApiUrl"] ?? "https://api.openai.com/v1/chat/completions";
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI:ApiKey is required");
    }

    public async Task<CvAnalysis> AnalyzeCvAsync(string cvText, string jobDescription, string jobRequirements)
    {
        try
        {
            var prompt = BuildAnalysisPrompt(cvText, jobDescription, jobRequirements);

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are an expert HR analyst specializing in CV evaluation." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                response_format = new { type = "json_object" }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(_apiUrl, jsonContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);

            var analysisJson = result?.Choices?.FirstOrDefault()?.Message?.Content;

            if (string.IsNullOrEmpty(analysisJson))
            {
                throw new Exception("Empty response from AI");
            }

            var analysis = JsonSerializer.Deserialize<CvAnalysisDto>(analysisJson);

            return MapToCvAnalysis(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing CV with AI");
            return CreateFallbackAnalysis();
        }
    }

    private string BuildAnalysisPrompt(string cvText, string jobDescription, string jobRequirements)
    {
        return $@"
Analyze the following CV against the job requirements and provide a detailed assessment in JSON format.

**Job Description:**
{jobDescription}

**Job Requirements:**
{jobRequirements}

**Candidate CV:**
{cvText}

Please provide your analysis in the following JSON structure:
{{
    ""overallScore"": <number 0-10>,
    ""summary"": ""<brief summary of candidate fit>"",
    ""extractedInformation"": {{
        ""name"": ""candidate name"",
        ""email"": ""email"",
        ""phone"": ""phone"",
        ""skills"": [""skill1"", ""skill2""],
        ""education"": [""degree and institution""],
        ""experience"": [""job title and company""],
        ""yearsOfExperience"": <number>
    }},
    ""criteriaEvaluation"": [
        {{
            ""criterion"": ""<requirement>"",
            ""score"": <number 0-10>,
            ""isMet"": ""true/false/partially"",
            ""evidence"": ""<specific evidence from CV>""
        }}
    ],
    ""redFlags"": [""<any concerns>""]
}}";
    }

    private CvAnalysis MapToCvAnalysis(CvAnalysisDto? dto)
    {
        if (dto == null) return CreateFallbackAnalysis();

        return new CvAnalysis
        {
            OverallScore = dto.OverallScore,
            Summary = dto.Summary,
            ExtractedInformation = new ExtractedInformation
            {
                Name = dto.ExtractedInformation?.Name ?? "",
                Email = dto.ExtractedInformation?.Email ?? "",
                Phone = dto.ExtractedInformation?.Phone ?? "",
                Skills = dto.ExtractedInformation?.Skills ?? new List<string>(),
                Education = dto.ExtractedInformation?.Education ?? new List<string>(),
                Experience = dto.ExtractedInformation?.Experience ?? new List<string>(),
                YearsOfExperience = dto.ExtractedInformation?.YearsOfExperience ?? 0
            },
            CriteriaEvaluation = dto.CriteriaEvaluation?.Select(c => new CriterionEvaluation
            {
                Criterion = c.Criterion,
                Score = c.Score,
                IsMet = c.IsMet,
                Evidence = c.Evidence
            }).ToList() ?? new List<CriterionEvaluation>(),
            RedFlags = dto.RedFlags ?? new List<string>(),
            AnalyzedAt = DateTime.UtcNow
        };
    }

    private CvAnalysis CreateFallbackAnalysis()
    {
        return new CvAnalysis
        {
            OverallScore = 5,
            Summary = "Analysis temporarily unavailable. Manual review required.",
            ExtractedInformation = new ExtractedInformation
            {
                Name = "",
                Email = "",
                Phone = "",
                Skills = new List<string>(),
                Education = new List<string>(),
                Experience = new List<string>(),
                YearsOfExperience = 0
            },
            CriteriaEvaluation = new List<CriterionEvaluation>(),
            RedFlags = new List<string> { "AI analysis unavailable" },
            AnalyzedAt = DateTime.UtcNow
        };
    }
}

// DTOs for OpenAI Response
public class OpenAIResponse
{
    public Choice[]? Choices { get; set; }
}

public class Choice
{
    public Message? Message { get; set; }
}

public class Message
{
    public string? Content { get; set; }
}

// ✅ DTO for JSON deserialization from OpenAI
public class CvAnalysisDto
{
    public double OverallScore { get; set; }
    public string Summary { get; set; } = string.Empty;
    public ExtractedInfoDto? ExtractedInformation { get; set; }
    public List<CriterionDto>? CriteriaEvaluation { get; set; }
    public List<string>? RedFlags { get; set; }
}

public class ExtractedInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<string>? Skills { get; set; }
    public List<string>? Education { get; set; }
    public List<string>? Experience { get; set; }
    public int YearsOfExperience { get; set; } // ✅ Thêm property
}

public class CriterionDto
{
    public string Criterion { get; set; } = string.Empty;
    public double Score { get; set; }
    public string IsMet { get; set; } = string.Empty;
    public string Evidence { get; set; } = string.Empty;
}