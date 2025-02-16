namespace DevRain_Test_Tsyhanok.Interfaces;

public interface IAzureOpenAIService
{
    Task<double[]> GenerateEmbeddingsAsync(string inputText);
    Task<string> GenerateChatResponseAsync(string excerpts, string questions);
    Task<string> GenerateResponseAsync(string prompt);
}