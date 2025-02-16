using DevRain_Test_Tsyhanok.Interfaces;

namespace DevRain_Test_Tsyhanok.Services;

public class DocumentIngestionService : IDocumentIngestionService
{
    public async Task<Dictionary<string, float[]>> IngestDocumentsAsync(Dictionary<string, string> documentContent)
    {
        if (documentContent == null)
            throw new ArgumentNullException(nameof(documentContent));

        var result = new Dictionary<string, float[]>();
        foreach (var (key, value) in documentContent)
        {
            Console.WriteLine($"Processing document {key}");
            result.Add(key, Array.Empty<float>());
        }
        
        return await Task.FromResult(result);
    }
}