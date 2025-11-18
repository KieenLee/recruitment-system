using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.Mongo;

public class Candidate
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// JobId from SQL Server - This is the "bridge" between SQL and MongoDB
    /// </summary>
    [BsonElement("jobId")]
    public int JobId { get; set; }

    [BsonElement("fullName")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("cvFileName")]
    public string CvFileName { get; set; } = string.Empty;

    [BsonElement("cvFilePath")]
    public string CvFilePath { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    [BsonElement("uploadedAt")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("aiAnalysis")]
    public AiAnalysis? AiAnalysis { get; set; }
}

public class AiAnalysis
{
    [BsonElement("overallScore")]
    public int OverallScore { get; set; }

    [BsonElement("summary")]
    public string Summary { get; set; } = string.Empty;

    [BsonElement("criteriaEvaluation")]
    public List<CriterionEvaluation> CriteriaEvaluation { get; set; } = new();

    [BsonElement("extractedInformation")]
    public ExtractedInformation ExtractedInformation { get; set; } = new();

    [BsonElement("redFlags")]
    public List<string> RedFlags { get; set; } = new();

    [BsonElement("analyzedAt")]
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}

public class CriterionEvaluation
{
    [BsonElement("criterion")]
    public string Criterion { get; set; } = string.Empty;

    [BsonElement("isMet")]
    public string IsMet { get; set; } = string.Empty; // "true", "false", "partially"

    [BsonElement("evidence")]
    public string Evidence { get; set; } = string.Empty;

    [BsonElement("score")]
    public int Score { get; set; }
}

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
}