namespace DevRain_Test_Tsyhanok.Interfaces;

public interface IDocumentRetrievalService
{
    string[] RetrieveRelevantDocuments(double[] userQuestion, Dictionary<string, double[]> documentEmbeddings);
}