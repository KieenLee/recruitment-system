using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.Mongo;

public class Candidate
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("jobId")]
    public int JobId { get; set; }

    [BsonElement("fullName")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("cvFilePath")]
    public string CvFilePath { get; set; } = string.Empty;

    [BsonElement("cvFileName")]
    public string CvFileName { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    [BsonElement("uploadedAt")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("aiAnalysis")]
    public CvAnalysis? AiAnalysis { get; set; }
}

// CV Analysis Result from AI
public class CvAnalysis
{
    [BsonElement("overallScore")]
    public double OverallScore { get; set; }

    [BsonElement("summary")]
    public string Summary { get; set; } = string.Empty;

    [BsonElement("extractedInformation")]
    public ExtractedInformation ExtractedInformation { get; set; } = new();

    [BsonElement("criteriaEvaluation")]
    public List<CriterionEvaluation> CriteriaEvaluation { get; set; } = new();

    [BsonElement("redFlags")]
    public List<string> RedFlags { get; set; } = new();

    [BsonElement("analyzedAt")]
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}

// ✅ Extracted Information from CV
public class ExtractedInformation
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("skills")]
    public List<string> Skills { get; set; } = new();

    [BsonElement("education")]
    public List<string> Education { get; set; } = new();

    [BsonElement("experience")]
    public List<string> Experience { get; set; } = new();

    [BsonElement("yearsOfExperience")]
    public int YearsOfExperience { get; set; }
}

// ✅ Evaluation of each job requirement criterion
public class CriterionEvaluation
{
    [BsonElement("criterion")]
    public string Criterion { get; set; } = string.Empty;

    [BsonElement("score")]
    public double Score { get; set; }

    [BsonElement("isMet")]
    public string IsMet { get; set; } = "false"; // "true", "false", "partially"

    [BsonElement("evidence")]
    public string Evidence { get; set; } = string.Empty;
}