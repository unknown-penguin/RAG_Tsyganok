namespace DevRain_Test_Tsyhanok.Interfaces;

public interface IPromptBuilder
{
    string BuildPrompt(string userQuestion, Dictionary<string, string> filesContent, string[] relevantDocumentsNames);
}