using DevRain_Test_Tsyhanok.Interfaces;

namespace DevRain_Test_Tsyhanok.Services;

public class DocumentRetrievalService : IDocumentRetrievalService
{
    public string[] RetrieveRelevantDocuments(double[] userQuestion, Dictionary<string, double[]> documentEmbeddings)
    {
        if (userQuestion is null || documentEmbeddings is null)
            throw new ArgumentException("User question and document embeddings must be non-null");

        Dictionary<string, double> similarities = new Dictionary<string, double>();

        foreach (var (documentName, embeddings) in documentEmbeddings)
        {
            similarities.Add(documentName, CosineSimilarity(userQuestion, embeddings));
        }

        var bestDocuments = similarities.OrderByDescending(doc => doc.Value)
            .Select(doc => doc.Key)
            .Take(3)
            .ToArray();

        return bestDocuments;
    }

    private double CosineSimilarity(double[] vec1, double[] vec2)
    {
        if (vec1 is null || vec2 is null || vec1.Length != vec2.Length)
            throw new ArgumentException("Vectors must be non-null and of the same length");

        double dot = 0.0, normA = 0.0, normB = 0.0;

        for (int i = 0; i < vec1.Length; i++)
        {
            dot += vec1[i] * vec2[i];
            normA += vec1[i] * vec1[i];
            normB += vec2[i] * vec2[i];
        }

        if (normA == 0 || normB == 0)
            return 0;

        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}