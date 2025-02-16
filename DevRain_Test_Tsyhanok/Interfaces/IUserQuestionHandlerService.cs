namespace DevRain_Test_Tsyhanok.Interfaces;

public interface IUserQuestionHandlerService
{
    Task<string> GetResponseWithCompletionsAsync(string question, Dictionary<string,string> filesContent, Dictionary<string, double[]> documentsEmbeddings);
    Task<string> GetResponseWithChatCompletionsAsync(string question, Dictionary<string,string> filesContent, Dictionary<string, double[]> documentsEmbeddings);
}