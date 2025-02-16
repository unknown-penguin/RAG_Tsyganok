using DevRain_Test_Tsyhanok.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace DevRain_Test_Tsyhanok.Services;

public class AzureOpenAIService : IAzureOpenAIService
{
    private readonly string _endpoint;
    private readonly string _apiKey;
    private readonly IHttpRequestService _httpRequestService;

    public AzureOpenAIService(IConfiguration configuration, IHttpRequestService httpRequestService)
    {
        _endpoint = configuration["AzureOpenAI:Endpoint"]
                    ?? throw new ArgumentException("Parameter AzureOpenAI:Endpoint is not initialised");
        _apiKey = configuration["AzureOpenAI:ApiKey"]
                  ?? throw new ArgumentException("Parameter AzureOpenAI:ApiKey is not initialised");

        _httpRequestService = httpRequestService
                              ?? throw new ArgumentNullException(nameof(httpRequestService));
    }

    public async Task<double[]> GenerateEmbeddingsAsync(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText))
            throw new ArgumentNullException(nameof(inputText));

        var requestBody = new
        {
            input = inputText
        };

        var requestUri = GenerateRequestUri(_endpoint, "text-embedding-ada-002", "embeddings", "2024-05-01-preview");

        var response = await _httpRequestService.PostAsync(requestUri, requestBody).ConfigureAwait(false);

        return EmbeddingResponseHandler(response);
    }

    private string GenerateRequestUri(string endpoint, string deploymentId, string taskName, string apiVersion) =>
        $"{endpoint}/openai/deployments/{deploymentId}/{taskName}?api-version={apiVersion}";

    private double[] EmbeddingResponseHandler(JsonElement response)
    {
        if (response.TryGetProperty("object", out var objectElement) &&
            objectElement.GetString() == "list" &&
            response.TryGetProperty("data", out var dataElement))
        {
            var documents = dataElement.EnumerateArray().ToList();
            if (documents.Count > 0)
            {
                var document = documents.First();
                if (document.TryGetProperty("embedding", out var embeddingElement))
                    return embeddingElement.EnumerateArray().Select(e => e.GetDouble()).ToArray();
            }
        }

        return Array.Empty<double>();
    }
}