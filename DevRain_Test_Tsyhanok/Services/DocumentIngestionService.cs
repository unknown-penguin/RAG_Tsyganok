using DevRain_Test_Tsyhanok.Interfaces;

namespace DevRain_Test_Tsyhanok.Services;

public class DocumentIngestionService : IDocumentIngestionService
{
    private readonly IAzureOpenAIService _azureOpenAIService;
    public DocumentIngestionService(IAzureOpenAIService azureOpenAIService)
    {
        _azureOpenAIService = azureOpenAIService 
                              ?? throw new ArgumentNullException(nameof(azureOpenAIService));
    }
    
    public async Task<Dictionary<string, double[]>> IngestDocumentsAsync(Dictionary<string, string> documentContent)
    {
        if (documentContent == null)
            throw new ArgumentNullException(nameof(documentContent));

        var result = new Dictionary<string, double[]>();
        foreach (var (key, value) in documentContent)
        {
            var embeddings = await _azureOpenAIService.GenerateEmbeddingsAsync(value);
            result.Add(key, embeddings);
        }
        
        return await Task.FromResult(result);
    }
}