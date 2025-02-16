using DevRain_Test_Tsyhanok.Interfaces;

namespace DevRain_Test_Tsyhanok.Services;

public class UserQuestionHandlerService : IUserQuestionHandlerService
{
    private readonly IAzureOpenAIService _azureOpenAIService;
    private readonly IDocumentRetrievalService _documentRetrievalService;
    private readonly IPromptBuilder _promptBuilder;

    public UserQuestionHandlerService(IAzureOpenAIService azureOpenAIService,
        IDocumentRetrievalService documentRetrievalService,
        IPromptBuilder promptBuilder)
    {
        _azureOpenAIService = azureOpenAIService;
        _documentRetrievalService = documentRetrievalService;
        _promptBuilder = promptBuilder;
    }

    public async Task<string> GetResponseWithChatCompletionsAsync(string question, Dictionary<string, string> filesContent,
        Dictionary<string, double[]> documentsEmbeddings)
    {
        var embeddings = await GetEmbeddingsAsync(question);
        var relevantDocumentsNames = _documentRetrievalService.RetrieveRelevantDocuments(embeddings, documentsEmbeddings);

        var excerpts = _promptBuilder.BuildPrompt(question, filesContent, relevantDocumentsNames);
        var response = await _azureOpenAIService.GenerateChatResponseAsync(excerpts, question);

        return response;
    }
    
    [Obsolete("GetResponseWithCompletionAsync is deprecated, please use GetResponseWithChatCompletionAsync instead.")]
    public async Task<string> GetResponseWithCompletionsAsync(string question, Dictionary<string, string> filesContent,
        Dictionary<string, double[]> documentsEmbeddings)
    {
        var embeddings = await GetEmbeddingsAsync(question);
        var relevantDocumentsNames = _documentRetrievalService.RetrieveRelevantDocuments(embeddings, documentsEmbeddings);

        var prompt = _promptBuilder.BuildPrompt(question, filesContent, relevantDocumentsNames);
        var response = await _azureOpenAIService.GenerateResponseAsync(prompt);

        return response;
    }

    private async Task<double[]> GetEmbeddingsAsync(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
            throw new ArgumentNullException(nameof(question));

        var embeddings = await _azureOpenAIService.GenerateEmbeddingsAsync(question);
        return embeddings;
    }
}