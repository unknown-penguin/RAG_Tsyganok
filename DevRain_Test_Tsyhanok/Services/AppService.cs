using DevRain_Test_Tsyhanok.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DevRain_Test_Tsyhanok.Services
{
    public class AppService : IAppService
    {
        private readonly IConfiguration _configuration;
        private readonly IFileLoaderService _fileLoaderService;
        private readonly IDocumentIngestionService _documentIngestionService;
        private readonly IUserQuestionHandlerService _userQuestionHandlerService;

        public AppService(IConfiguration configuration,
            IFileLoaderService fileLoaderService,
            IDocumentIngestionService documentIngestionService,
            IUserQuestionHandlerService userQuestionHandlerService)
        {
            _configuration = configuration;
            _fileLoaderService = fileLoaderService;
            _documentIngestionService = documentIngestionService;
            _userQuestionHandlerService = userQuestionHandlerService;
        }


        public async Task RunAsync()
        {
            var folderPath = GetFolderPath();
            
            var filesContent = await GetFilesContent(folderPath).ConfigureAwait(false);
            
            var documentsEmbeddings = await GetEmbeddings(filesContent).ConfigureAwait(false);
            
            RequestUserInputAsync(out var userQuestion);
            if(userQuestion == "exit")
                return;
            
            var response = await GetResponse(userQuestion, filesContent, documentsEmbeddings).ConfigureAwait(false);

            PrintResponse(response);
            
        }
        private string GetFolderPath()
        {
            var folderPath = _configuration["Documents:FolderPath"];
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new NullReferenceException( "Folder path is not set in the configuration file");
            return folderPath;
        }
        
        private async Task<Dictionary<string,string>> GetFilesContent(string folderPath)
        {
            Console.WriteLine("Loading files...");
            var filesContent = await _fileLoaderService.LoadFilesAsync(folderPath).ConfigureAwait(false);
            if (filesContent.Count == 0)
                throw new Exception("No files found in the specified folder");
            return filesContent;
        }
        
        private async Task<Dictionary<string,double[]>> GetEmbeddings(Dictionary<string,string> filesContent)
        {
            Console.WriteLine("Ingesting documents...");
            var documentsEmbeddings = await _documentIngestionService.IngestDocumentsAsync(filesContent).ConfigureAwait(false);
            if (documentsEmbeddings.Count == 0)
                throw new Exception("No embeddings recieved for the documents");
            
            Console.WriteLine("Documents ingested successfully");
            return documentsEmbeddings;
        }
        private void RequestUserInputAsync(out string userQuestion)
        {
            Console.WriteLine("Enter the question or enter \"exit\":");
            while (true)
            {
                userQuestion = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(userQuestion))
                    Console.WriteLine("Enter the question or enter \"exit\":");
                if (!string.IsNullOrWhiteSpace(userQuestion))
                    break;
            }
        }
        private async Task<string> GetResponse(string userQuestion, Dictionary<string,string> filesContent, Dictionary<string,double[]> documentsEmbeddings)
        {
            var result = await _userQuestionHandlerService.GetResponseWithChatCompletionsAsync(userQuestion, filesContent, documentsEmbeddings)
                .ConfigureAwait(false);
            
            if(string.IsNullOrWhiteSpace(result))
                throw new Exception("No response received");
            
            return result;
        }
        private void PrintResponse(string response)
        {
            Console.WriteLine("Response: ");
            Console.WriteLine(response);
        }
    }
}