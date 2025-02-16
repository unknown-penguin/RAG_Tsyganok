using DevRain_Test_Tsyhanok.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DevRain_Test_Tsyhanok.Services
{
    public class AppService : IAppService
    {
        private readonly IConfiguration _configuration;
        private readonly IFileLoaderService _fileLoaderService;
        private readonly IDocumentIngestionService _documentIngestionService;

        public AppService(IConfiguration configuration,
            IFileLoaderService fileLoaderService,
            IDocumentIngestionService documentIngestionService)
        {
            _configuration = configuration;
            _fileLoaderService = fileLoaderService;
            _documentIngestionService = documentIngestionService;
        }

        public async Task RunAsync()
        {
            var folderPath = _configuration["Documents:FolderPath"];
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                Console.Error.WriteLine("Documents folder path not specified.");
                return;
            }

            Console.WriteLine("Loading files...");
            var filesContent = await _fileLoaderService.LoadFilesAsync(folderPath).ConfigureAwait(false);

            Console.WriteLine("Ingesting documents...");
            var documents = await _documentIngestionService.IngestDocumentsAsync(filesContent).ConfigureAwait(false);
            
        }
    }
}