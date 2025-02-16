using DevRain_Test_Tsyhanok.Interfaces;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace DevRain_Test_Tsyhanok.Services;

public class HttpRequestService : IHttpRequestService
{
    private readonly HttpClient _httpClient;

    public HttpRequestService(HttpClient httpClient,IConfiguration configuration)
    {
        
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.DefaultRequestHeaders.Add("api-key", configuration["AzureOpenAI:ApiKey"]);
    }
    
    public async Task<JsonElement> PostAsync(string requestUri, object requestBody)
    {
        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync(requestUri, content).ConfigureAwait(false);
        
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(jsonResponse))
        {
            await Console.Error.WriteLineAsync("Empty response.");
            return default;
        }
        
        return JsonSerializer.Deserialize<JsonElement>(jsonResponse);
    }
}