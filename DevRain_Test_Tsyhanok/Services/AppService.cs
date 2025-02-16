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
            var folderPath = GetFolderPath();
            if (string.IsNullOrWhiteSpace(folderPath))
                return;
            
            var filesContent = await GetFilesContent(folderPath).ConfigureAwait(false);
            if (filesContent.Count == 0)
                return;
            
            var documentsEmbeddings = await GetEmbeddings(filesContent).ConfigureAwait(false);
            if (documentsEmbeddings.Count == 0)
                return;
            
        }
        private string GetFolderPath()
        {
            var folderPath = _configuration["Documents:FolderPath"];
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                Console.Error.WriteLine("Documents folder path not specified.");
                return null;
            }
            return folderPath;
        }
        
        private async Task<Dictionary<string,string>> GetFilesContent(string folderPath)
        {
            Console.WriteLine("Loading files...");
            var filesContent = await _fileLoaderService.LoadFilesAsync(folderPath).ConfigureAwait(false);
            return filesContent;
        }
        
        private async Task<Dictionary<string,double[]>> GetEmbeddings(Dictionary<string,string> filesContent)
        {
            Console.WriteLine("Ingesting documents...");
            var documentsEmbeddings = await _documentIngestionService.IngestDocumentsAsync(filesContent).ConfigureAwait(false);
            return documentsEmbeddings;
        }
    }
}