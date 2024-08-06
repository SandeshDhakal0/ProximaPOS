using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Services.Base;

public class BaseService(HttpClient httpClient, ILocalStorageService localStorage)
{
    private readonly string _baseUrl = AppConfiguration.BaseUrl;

    public async Task<GlobalState> GetGlobalState()
    {
        var userDetails = await GetAsync<Derived<LoginResponseDetailsDto>>("authenticate/profile");

        if (userDetails?.Result == null || userDetails.Result.UserId == 0)
        {
            await localStorage.RemoveItemAsync("access_token");

            return new GlobalState();
        }

        var result = userDetails.Result;

        var globalState = new GlobalState
        {
            UserId = result.UserId,
            EmployeeId = result.EmployeeId,
            Name = result.Name,
            RoleId = result.RoleId,
            RoleName = result.RoleName,
            RoleType = result.RoleType,
            CompanyId = result.CompanyId,
            CompanyName = result.CompanyName,
            OrganizationId = result.OrganizationId,
            OrganizationName = result.OrganizationName,
            IsAdmin = result.IsAdmin,
            IsSuperAdmin = result.IsSuperAdmin
        };

        return globalState;
    }

    public async Task<bool> IsUserLoggedIn()
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        if (accessToken == null) return false;

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = tokenHandler.ReadJwtToken(accessToken);

        var expiryDateTime = jwtToken.ValidTo;

        return expiryDateTime > DateTime.UtcNow;
    }

    public async Task LogOutUser()
    {
        await localStorage.RemoveItemAsync("access_token");
    }
    
    public async Task<T?> GetAsync<T>(string endpoint, IDictionary<string, string>? parameters = null)
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        if (accessToken != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var fullUrl = $"{_baseUrl}/api/{endpoint}";

        if (parameters is { Count: > 0 })
        {
            var queryString = string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            
            fullUrl += "?" + queryString;
        }

        var response = await httpClient.GetAsync(fullUrl);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }


    public async Task<T?> PostAsync<T>(string endpoint, StringContent stringContent)
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        if (accessToken != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        
        var response = await httpClient.PostAsync($"{_baseUrl}/api/{endpoint}", stringContent);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }
    
    public async Task<T?> UpdateAsync<T>(string endpoint, StringContent stringContent)
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        if (accessToken != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        
        var response = await httpClient.PatchAsync($"{_baseUrl}/api/{endpoint}", stringContent);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }

    public async Task<T?> DeleteAsync<T>(string endpoint)
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        if (accessToken != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        
        var response = await httpClient.DeleteAsync($"{_baseUrl}/api/{endpoint}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }
    
    public async Task<HttpResponseMessage> GetAsyncOnly(string endpoint, IDictionary<string, string>? parameters = null)
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        if (accessToken != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var fullUrl = $"{_baseUrl}/api/{endpoint}";

        if (parameters is { Count: > 0 })
        {
            var queryString = string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            fullUrl += "?" + queryString;
        }

        var response = await httpClient.GetAsync(fullUrl);
        
        return response;
    }
    
    public async Task<Derived<bool>> DeleteAsyncWithId(string endpoint, int vendorId, bool status)
    {
        var url = $"{_baseUrl}/api/{endpoint}/{vendorId}/{status}";
        
        var response = await httpClient.DeleteAsync(url);

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<Derived<bool>>())!;
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        
        throw new ApplicationException($"Error: {response.StatusCode}, Message: {errorMessage}");
    }
}