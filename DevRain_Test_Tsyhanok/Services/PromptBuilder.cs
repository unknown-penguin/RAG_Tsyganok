using System.Text;
using DevRain_Test_Tsyhanok.Interfaces;

namespace DevRain_Test_Tsyhanok.Services;

public class PromptBuilder : IPromptBuilder
{
    [Obsolete("BuildPrompt is deprecated, please use BuildChatPrompt instead.")]
    public string BuildPrompt(string userQuestion, Dictionary<string, string> filesContent,
        string[] relevantDocumentsNames)
    {
        var prompt = new StringBuilder();

        foreach (var documentName in relevantDocumentsNames)
        {
            if (!filesContent.TryGetValue(documentName, out var documentContent))
                throw new NullReferenceException($"Document with name {documentName} not found");

            prompt.AppendLine(documentContent);
            prompt.AppendLine("<|endoftext|>");
        }

        prompt.AppendLine(userQuestion);

        return prompt.ToString();
    }

    public string BuildChatPrompt(Dictionary<string, string> filesContent,
        string[] relevantDocumentsNames)
    {
        var prompt = new StringBuilder();

        foreach (var documentName in relevantDocumentsNames)
        {
            if (!filesContent.TryGetValue(documentName, out var documentContent))
                throw new NullReferenceException($"Document with name {documentName} not found");

            prompt.AppendLine(documentContent);
            prompt.AppendLine("\n");
        }

        return prompt.ToString();
    }
}