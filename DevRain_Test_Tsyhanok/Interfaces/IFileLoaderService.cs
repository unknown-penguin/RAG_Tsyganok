namespace DevRain_Test_Tsyhanok.Interfaces;

public interface IFileLoaderService
{
    Task<Dictionary<string, string>> LoadFilesAsync(string path);
}