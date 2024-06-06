using System.Net.Http.Json;
using System.Net.Http.Headers;
using TheHighInnovation.POS.WEB.Model.Response.Base;

namespace TheHighInnovation.POS.WEB.Services.Base;

public class BaseService
{
    public readonly string _baseUrl = AppConfiguration.BaseUrl;
    private readonly HttpClient _httpClient;
    private readonly StorageService _storageService;

    public BaseService(
        HttpClient httpClient, StorageService storageService)
    {
        _httpClient = httpClient;
        _storageService = storageService;
    }

    public string GetBaseUrl()
    {
        return _baseUrl;
    }
    
    public async Task<T?> GetAsync<T>(string endpoint, IDictionary<string, string>? parameters = null)
    {
        var accessToken = await _storageService.GetItemAsync("access_token");

        if (accessToken != null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var fullUrl = $"{_baseUrl}/api/{endpoint}";

        if (parameters is { Count: > 0 })
        {
            var queryString = string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            
            fullUrl += "?" + queryString;
        }

        var response = await _httpClient.GetAsync(fullUrl);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }


    public async Task<T?> PostAsync<T>(string endpoint, StringContent stringContent)
    {
        var response = await _httpClient.PostAsync($"{_baseUrl}/api/{endpoint}", stringContent);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }

    public async Task<T?> DeleteAsync<T>(string endpoint)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/{endpoint}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }

    public async Task<Derived<bool>> DeleteAsyncWithId<T>(string endpoint, int vendorId, bool status)
    {
        // Construct the query string
        var url = $"{_baseUrl}/api/{endpoint}/{vendorId}/{status}";
        var response = await _httpClient.DeleteAsync(url);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Derived<bool>>();
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new ApplicationException($"Error: {response.StatusCode}, Message: {errorMessage}");
        }
    }


}