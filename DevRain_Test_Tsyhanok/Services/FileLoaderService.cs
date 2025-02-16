using DevRain_Test_Tsyhanok.Interfaces;

namespace DevRain_Test_Tsyhanok.Services;

public class FileLoaderService: IFileLoaderService
{
    private readonly IEnumerable<IFileReader> _fileReaders;

    public FileLoaderService(IEnumerable<IFileReader> fileReaders)
    {
        _fileReaders = fileReaders;
    }

    public async Task<Dictionary<string, string>> LoadFilesAsync(string path)
    {
        var files = new Dictionary<string, string>();
        var filePaths = Directory.GetFiles(path);

        foreach (var filePath in filePaths)
        {
            var extension = Path.GetExtension(filePath);
            var reader = _fileReaders.FirstOrDefault(r => r.CanRead(extension));
            if (reader != null)
            {
                var content = await reader.ReadAsync(filePath);
                files.Add(Path.GetFileName(filePath), content);
            }
            else
            {
                Console.WriteLine($"No reader available for file extension {extension}");
            }
        }

        return files;
    }
}