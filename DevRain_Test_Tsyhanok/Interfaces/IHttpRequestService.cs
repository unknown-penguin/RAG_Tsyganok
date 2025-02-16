using System.Text.Json;

namespace DevRain_Test_Tsyhanok.Interfaces;

public interface IHttpRequestService
{
    Task<JsonElement> PostAsync(string requestUri, object requestBody);
}