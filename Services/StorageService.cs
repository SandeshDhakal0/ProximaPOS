using Microsoft.JSInterop;
using System.Threading.Tasks;


namespace TheHighInnovation.POS.WEB.Services
{
    public class StorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public StorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetItemAsync(string key)
        {
            return await _jsRuntime.InvokeAsync<string>("localStorageHelper.get", key);
        }

        public async Task SetItemAsync(string key, string value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorageHelper.set", key, value);
        }

        public async Task RemoveItemAsync(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorageHelper.remove", key);
        }

    }
}