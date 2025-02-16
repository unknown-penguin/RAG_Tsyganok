namespace DevRain_Test_Tsyhanok.Interfaces;

public interface IDocumentIngestionService
{
    Task<Dictionary<string, float[]>> IngestDocumentsAsync(Dictionary<string, string> documentContent);
}