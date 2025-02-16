using DevRain_Test_Tsyhanok.Interfaces;

namespace DevRain_Test_Tsyhanok.Services;

public class TextFileReader : IFileReader
{
    public bool CanRead(string fileExtension) =>
        fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase);

    public async Task<string> ReadAsync(string filePath) =>
        await File.ReadAllTextAsync(filePath);
}