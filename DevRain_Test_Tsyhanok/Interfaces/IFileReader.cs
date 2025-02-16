namespace DevRain_Test_Tsyhanok.Interfaces;

public interface IFileReader
{
    bool CanRead(string fileExtension);
    Task<string> ReadAsync(string filePath);
}