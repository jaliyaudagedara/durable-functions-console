using Microsoft.JSInterop;
using System.Text.Json;

namespace DfxConsole.BlazorUI.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> SetItemAsync<T>(string key, T value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            return await _jsRuntime.InvokeAsync<bool>("localStorageInterop.setItem", key, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting item in localStorage: {ex.Message}");
            return false;
        }
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("localStorageInterop.getItem", key);
            
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting item from localStorage: {ex.Message}");
            return default;
        }
    }

    public async Task<bool> RemoveItemAsync(string key)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("localStorageInterop.removeItem", key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item from localStorage: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ClearAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("localStorageInterop.clear");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing localStorage: {ex.Message}");
            return false;
        }
    }
}