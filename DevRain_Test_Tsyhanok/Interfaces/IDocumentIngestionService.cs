namespace DevRain_Test_Tsyhanok.Interfaces;

public interface IDocumentIngestionService
{
    Task<Dictionary<string, double[]>> IngestDocumentsAsync(Dictionary<string, string> documentContent);
}