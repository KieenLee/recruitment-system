using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace Backend.Services;

public interface ICvParserService
{
    Task<string> SaveCvFileAsync(IFormFile file, int jobId);
    Task<string> ExtractTextFromPdfAsync(string filePath);
    string GetCvStoragePath();
}

public class CvParserService : ICvParserService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CvParserService> _logger;

    public CvParserService(IWebHostEnvironment environment, ILogger<CvParserService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public string GetCvStoragePath()
    {
        var path = Path.Combine(_environment.ContentRootPath, "Storage", "CVs");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    public async Task<string> SaveCvFileAsync(IFormFile file, int jobId)
    {
        try
        {
            var storagePath = GetCvStoragePath();
            var fileName = $"{jobId}_{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(storagePath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving CV file");
            throw;
        }
    }

    public async Task<string> ExtractTextFromPdfAsync(string filePath)
    {
        try
        {
            using var pdfReader = new PdfReader(filePath);
            using var pdfDocument = new PdfDocument(pdfReader);

            var text = new StringBuilder();
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
            {
                var page = pdfDocument.GetPage(i);
                text.Append(PdfTextExtractor.GetTextFromPage(page));
            }

            return text.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from PDF");
            return string.Empty;
        }
    }
}