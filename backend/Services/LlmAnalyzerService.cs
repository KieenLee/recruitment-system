using System.Text;
using System.Text.Json;
using Backend.Models.Mongo;

namespace Backend.Services;

public interface ILlmAnalyzerService
{
    Task<AiAnalysis?> AnalyzeCvAsync(string cvText, string jobDescription);
}

public class LlmAnalyzerService : ILlmAnalyzerService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LlmAnalyzerService> _logger;

    public LlmAnalyzerService(HttpClient httpClient, IConfiguration configuration, ILogger<LlmAnalyzerService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AiAnalysis?> AnalyzeCvAsync(string cvText, string jobDescription)
    {
        try
        {
            var prompt = CreateLlmPrompt(cvText, jobDescription);
            var apiKey = _configuration["OpenAI:ApiKey"] ?? "";
            var apiUrl = _configuration["OpenAI:ApiUrl"] ?? "https://api.openai.com/v1/chat/completions";

            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = "You are a senior HR specialist analyzing CVs. Always respond with valid JSON only." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 2000
            };

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseContent);
            var aiResponseText = jsonResponse.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";

            // Parse the AI response to AiAnalysis object
            var aiAnalysis = JsonSerializer.Deserialize<AiAnalysis>(aiResponseText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (aiAnalysis != null)
            {
                aiAnalysis.AnalyzedAt = DateTime.UtcNow;
            }

            return aiAnalysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing CV with LLM");
            return null;
        }
    }

    private static string CreateLlmPrompt(string cvText, string jobDescription)
    {
        return $@"
As a senior HR specialist, your task is to analyze the following CV based on the provided Job Description (JD).
Analyze objectively and return the result ONLY in a valid JSON format.

**Job Description (JD):**
{jobDescription}

**Candidate's CV Text:**
{cvText}

**JSON Output Schema:**
{{
  ""overallScore"": <A score from 1 to 10 on how well the candidate fits the JD>,
  ""summary"": ""<A 3-4 sentence summary of the candidate's key strengths and weaknesses regarding the JD>"",
  ""criteriaEvaluation"": [
    {{
      ""criterion"": ""<Name of the criterion from the JD>"",
      ""isMet"": ""<true/false/partially>"",
      ""evidence"": ""<Quote the exact phrase from the CV that supports this evaluation>"",
      ""score"": <A score from 1 to 10 for this specific criterion>
    }}
  ],
  ""extractedInformation"": {{
    ""name"": ""<Candidate's Full Name>"",
    ""email"": ""<Candidate's Email>"",
    ""phone"": ""<Candidate's Phone Number>"",
    ""skills"": [""<List of detected skills>""],
    ""education"": [""<List of education background>""],
    ""experience"": [""<List of work experience>""]
  }},
  ""redFlags"": [""<List any potential red flags, e.g., frequent job hopping, unexplained career gaps>""]
}}

Please respond with ONLY the JSON, no additional text or explanation.";
    }
}