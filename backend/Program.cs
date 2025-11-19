using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Recruitment System API",
        Version = "v1",
        Description = "AI-Powered Recruitment Automation System with Hybrid Database (SQL Server + MongoDB)"
    });
});

// Configure SQL Server with Entity Framework Core
builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServerConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    )
);

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);
builder.Services.AddSingleton<MongoDbContext>();

// Register Services
builder.Services.AddScoped<IJobPostingService, JobPostingService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<ICvParserService, CvParserService>();
builder.Services.AddHttpClient<ILlmAnalyzerService, LlmAnalyzerService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Recruitment System API V1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at app's root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
