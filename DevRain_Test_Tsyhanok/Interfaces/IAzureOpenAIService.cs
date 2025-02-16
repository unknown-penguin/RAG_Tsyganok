namespace DevRain_Test_Tsyhanok.Interfaces;

public interface IAzureOpenAIService
{
    Task<double[]> GenerateEmbeddingsAsync(string inputText);
}