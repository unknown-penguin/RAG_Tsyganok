using DevRain_Test_Tsyhanok.Interfaces;
using DevRain_Test_Tsyhanok.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace DevRain_Test_Tsyhanok;

public class DevRain_Test_Tsyhanok
{
    private static IConfiguration configuration;

    public static async Task Main(string[] args)
    {
        try
        {
            LoadConfiguration();
            if (configuration == null)
            {
                Console.Error.WriteLine("Configuration not loaded.");
                return;
            }

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            Console.WriteLine("Starting application...");
            var appService = serviceProvider.GetRequiredService<IAppService>();
            await appService.RunAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unhandled exception: {ex.Message}");
        }
    }

    private static void LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                optional: true, reloadOnChange: true);
        configuration = builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IConfiguration>(configuration);
        services.AddHttpClient();
        services.AddTransient<IFileReader, TextFileReader>();
        services.AddTransient<IFileLoaderService, FileLoaderService>();
        services.AddTransient<IDocumentIngestionService, DocumentIngestionService>();
        services.AddTransient<IHttpRequestService, HttpRequestService>();
        services.AddTransient<IAzureOpenAIService, AzureOpenAIService>();
        services.AddTransient<IAppService, AppService>();
        services.AddTransient<IUserQuestionHandlerService, UserQuestionHandlerService>();
        services.AddTransient<IDocumentRetrievalService, DocumentRetrievalService>();
        services.AddTransient<IPromptBuilder, PromptBuilder>();
    }
}