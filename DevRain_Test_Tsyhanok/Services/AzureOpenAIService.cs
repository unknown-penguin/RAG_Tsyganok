using DevRain_Test_Tsyhanok.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using DevRain_Test_Tsyhanok.Models;

namespace DevRain_Test_Tsyhanok.Services;

public class AzureOpenAIService : IAzureOpenAIService
{
    private readonly string _endpoint;
    private readonly IHttpRequestService _httpRequestService;
    private readonly DeploymentInfo _embeddedDeploymentInfo;
    private readonly DeploymentInfo _chatDeploymentInfo;

    public AzureOpenAIService(IConfiguration configuration, IHttpRequestService httpRequestService)
    {
        _endpoint = configuration["AzureOpenAI:Endpoint"]
                    ?? throw new ArgumentException("Parameter AzureOpenAI:Endpoint is not initialised");

        _httpRequestService = httpRequestService
                              ?? throw new ArgumentNullException(nameof(httpRequestService));

        _embeddedDeploymentInfo = new DeploymentInfo()
        {
            DeploymentId = configuration["AzureOpenAI:EmbeddingModel:DeploymentId"] ??
                           throw new ArgumentException(
                               "Parameter AzureOpenAI:EmbeddingModel:DeploymentId is not initialised"),
            TaskName = configuration["AzureOpenAI:EmbeddingModel:TaskName"] ??
                       throw new ArgumentException(
                           "Parameter AzureOpenAI:EmbeddingModel:DeploymentId is not initialised"),
            ApiVersion = configuration["AzureOpenAI:EmbeddingModel:ApiVersion"] ??
                         throw new ArgumentException(
                             "Parameter AzureOpenAI:EmbeddingModel:DeploymentId is not initialised")
        };

        _chatDeploymentInfo = new DeploymentInfo()
        {
            DeploymentId = configuration["AzureOpenAI:ChatCompletionModel:DeploymentId"] ??
                           throw new ArgumentException(
                               "Parameter AzureOpenAI:ChatCompletionModel:DeploymentId is not initialised"),
            TaskName = configuration["AzureOpenAI:ChatCompletionModel:TaskName"] ??
                       throw new ArgumentException(
                           "Parameter AzureOpenAI:ChatCompletionModel:DeploymentId is not initialised"),
            ApiVersion = configuration["AzureOpenAI:ChatCompletionModel:ApiVersion"] ??
                         throw new ArgumentException(
                             "Parameter AzureOpenAI:ChatCompletionModel:DeploymentId is not initialised")
        };
    }

    public async Task<double[]> GenerateEmbeddingsAsync(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText))
            throw new ArgumentNullException(nameof(inputText));

        var requestBody = new
        {
            input = inputText
        };

        var requestUri = GenerateRequestUri(_endpoint, _embeddedDeploymentInfo.DeploymentId,
            _embeddedDeploymentInfo.TaskName, _embeddedDeploymentInfo.ApiVersion);

        var response = await _httpRequestService.PostAsync(requestUri, requestBody).ConfigureAwait(false);

        return EmbeddingResponseHandler(response);
    }

    public async Task<string> GenerateChatResponseAsync(string excerpts, string questions)
    {
        if (string.IsNullOrWhiteSpace(excerpts))
            throw new ArgumentNullException(nameof(excerpts));

        if (string.IsNullOrWhiteSpace(questions))
            throw new ArgumentNullException(nameof(questions));

        var requestBody = new
        {
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = excerpts
                },
                new
                {
                    role = "user",
                    content = questions
                }
            }
        };

        var requestUri = GenerateRequestUri(_endpoint, _chatDeploymentInfo.DeploymentId, _chatDeploymentInfo.TaskName,
            _chatDeploymentInfo.ApiVersion, additionalDeploymentInfo: "chat");

        var response = await _httpRequestService.PostAsync(requestUri, requestBody).ConfigureAwait(false);

        return ChatResponseHandler(response);
    }

    [Obsolete("GenerateResponseAsync is deprecated, please use GenerateChatResponseAsync instead.")]
    public async Task<string> GenerateResponseAsync(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            throw new ArgumentNullException(nameof(prompt));

        var requestBody = new
        {
            prompt = prompt
        };

        var requestUri = GenerateRequestUri(_endpoint, _chatDeploymentInfo.DeploymentId, _chatDeploymentInfo.TaskName, _chatDeploymentInfo.ApiVersion);

        var response = await _httpRequestService.PostAsync(requestUri, requestBody).ConfigureAwait(false);

        return ResponseHandler(response);
    }

    private string GenerateRequestUri(string endpoint, string deploymentId, string taskName, string apiVersion,
        string additionalDeploymentInfo = "")
        => additionalDeploymentInfo != ""
            ? $"{endpoint}/openai/deployments/{deploymentId}/{additionalDeploymentInfo}/{taskName}/?api-version={apiVersion}"
            : $"{endpoint}/openai/deployments/{deploymentId}/{taskName}?api-version={apiVersion}";


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

    private string ChatResponseHandler(JsonElement response)
    {
        if (response.TryGetProperty("choices", out var choicesElement))
        {
            var choices = choicesElement.EnumerateArray().ToList();
            if (choices.Count != 0)
            {
                var firstChoice = choices.First();
                if (firstChoice.TryGetProperty("message", out var messageElement) &&
                    messageElement.TryGetProperty("role", out var roleElement) &&
                    roleElement.GetString() == "assistant" &&
                    messageElement.TryGetProperty("content", out var contentElement))
                    return contentElement.GetString() ?? string.Empty;
            }
        }

        return string.Empty;
    }

    [Obsolete("ResponseHandler is deprecated, please use ChatResponseHandler instead.")]
    private string ResponseHandler(JsonElement response)
    {
        if (response.TryGetProperty("object", out var objectElement) &&
            objectElement.GetString() == "list" &&
            response.TryGetProperty("data", out var dataElement))
        {
            var documents = dataElement.EnumerateArray().ToList();
            if (documents.Count > 0)
            {
                var document = documents.First();
                if (document.TryGetProperty("answer", out var answerElement))
                    return answerElement.GetString() ?? string.Empty;
            }
        }

        return string.Empty;
    }
}