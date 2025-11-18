using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace Backend.Data;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string CandidatesCollectionName { get; set; } = "Candidates";
}

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }

    // Shortcut property for Candidates collection
    public IMongoCollection<Models.Mongo.Candidate> Candidates =>
        GetCollection<Models.Mongo.Candidate>(nameof(Candidates));
}