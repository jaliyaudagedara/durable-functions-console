using DfxConsole.BlazorUI.Constants;
using DfxConsole.BlazorUI.Models;
using System.Text.Json;

namespace DfxConsole.BlazorUI.Services;

public class DurableFunctionService
{
    private readonly HttpClient _httpClient;
    private readonly TimeZoneService _timeZoneService;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public DurableFunctionService(HttpClient httpClient, TimeZoneService timeZoneService)
    {
        _httpClient = httpClient;
        _timeZoneService = timeZoneService;
    }

    public async Task<PagedResponse<List<InstanceInfo>>> GetInstancesAsync(
        DurableFunctionConfiguration config,
        DateTime? createdTimeFrom = null,
        DateTime? createdTimeTo = null,
        List<string>? runtimeStatus = null,
        string? instanceIdPrefix = null,
        bool showInput = false,
        int? top = null,
        string? continuationToken = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["taskHub"] = config.TaskHub,
            ["connection"] = config.ConnectionName,
            ["code"] = config.AuthCode
        };

        if (createdTimeFrom.HasValue)
        {
            DateTime utcTime = await _timeZoneService.ConvertLocalToUtcAsync(createdTimeFrom.Value);
            queryParams["createdTimeFrom"] = utcTime.ToString("o");
        }
        if (createdTimeTo.HasValue)
        {
            DateTime utcTime = await _timeZoneService.ConvertLocalToUtcAsync(createdTimeTo.Value);
            queryParams["createdTimeTo"] = utcTime.ToString("o");
        }
        if (runtimeStatus?.Any() == true)
        {
            queryParams["runtimeStatus"] = string.Join(",", runtimeStatus);
        }
        if (!string.IsNullOrEmpty(instanceIdPrefix))
        {
            queryParams["instanceIdPrefix"] = instanceIdPrefix;
        }
        queryParams["showInput"] = showInput.ToString().ToLower();
        if (top.HasValue)
        {
            queryParams["top"] = top.Value.ToString();
        }

        var url = BuildUrl(config.FunctionAppUrl, "/runtime/webhooks/durableTask/instances", queryParams);
        return await ExecutePagedRequestAsync<List<InstanceInfo>>(HttpMethod.Get, url, config, continuationToken);
    }

    public async Task<InstanceInfo?> GetInstanceAsync(
        DurableFunctionConfiguration config,
        string instanceId,
        bool showHistory = true,
        bool showHistoryOutput = false,
        bool showInput = true,
        bool returnInternalServerErrorOnFailure = false)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["taskHub"] = config.TaskHub,
            ["connection"] = config.ConnectionName,
            ["code"] = config.AuthCode,
            ["showHistory"] = showHistory.ToString().ToLower(),
            ["showHistoryOutput"] = showHistoryOutput.ToString().ToLower(),
            ["showInput"] = showInput.ToString().ToLower(),
            ["returnInternalServerErrorOnFailure"] = returnInternalServerErrorOnFailure.ToString().ToLower()
        };

        var url = BuildUrl(config.FunctionAppUrl, $"/runtime/webhooks/durabletask/instances/{instanceId}", queryParams);
        return await ExecuteRequestAsync<InstanceInfo>(HttpMethod.Get, url, config);
    }

    public async Task<string> PurgeInstanceAsync(
        DurableFunctionConfiguration config,
        string instanceId)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["taskHub"] = config.TaskHub,
            ["connection"] = config.ConnectionName,
            ["code"] = config.AuthCode
        };

        var url = BuildUrl(config.FunctionAppUrl, $"/runtime/webhooks/durabletask/instances/{instanceId}", queryParams);
        return await ExecuteRequestAsync<string>(HttpMethod.Delete, url, config) ?? string.Empty;
    }

    public async Task<string> PurgeInstancesAsync(
        DurableFunctionConfiguration config,
        DateTime? createdTimeFrom = null,
        DateTime? createdTimeTo = null,
        List<string>? runtimeStatus = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["taskHub"] = config.TaskHub,
            ["connection"] = config.ConnectionName,
            ["code"] = config.AuthCode
        };

        if (createdTimeFrom.HasValue)
        {
            DateTime utcTime = await _timeZoneService.ConvertLocalToUtcAsync(createdTimeFrom.Value);
            queryParams["createdTimeFrom"] = utcTime.ToString("o");
        }
        if (createdTimeTo.HasValue)
        {
            DateTime utcTime = await _timeZoneService.ConvertLocalToUtcAsync(createdTimeTo.Value);
            queryParams["createdTimeTo"] = utcTime.ToString("o");
        }
        if (runtimeStatus?.Any() == true)
        {
            queryParams["runtimeStatus"] = string.Join(",", runtimeStatus);
        }

        var url = BuildUrl(config.FunctionAppUrl, "/runtime/webhooks/durabletask/instances", queryParams);
        return await ExecuteRequestAsync<string>(HttpMethod.Delete, url, config) ?? string.Empty;
    }

    public async Task<string> TerminateInstanceAsync(
        DurableFunctionConfiguration config,
        string instanceId,
        string? reason = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["taskHub"] = config.TaskHub,
            ["connection"] = config.ConnectionName,
            ["code"] = config.AuthCode
        };

        if (!string.IsNullOrEmpty(reason))
        {
            queryParams["reason"] = reason;
        }

        var url = BuildUrl(config.FunctionAppUrl, $"/runtime/webhooks/durabletask/instances/{instanceId}/terminate", queryParams);
        return await ExecuteRequestAsync<string>(HttpMethod.Post, url, config) ?? string.Empty;
    }

    public async Task<string> SuspendInstanceAsync(
        DurableFunctionConfiguration config,
        string instanceId,
        string? reason = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["reason"] = reason ?? string.Empty,
            ["taskHub"] = config.TaskHub,
            ["connection"] = config.ConnectionName,
            ["code"] = config.AuthCode
        };

        var url = BuildUrl(config.FunctionAppUrl, $"/runtime/webhooks/durabletask/instances/{instanceId}/suspend", queryParams);
        return await ExecuteRequestAsync<string>(HttpMethod.Post, url, config) ?? string.Empty;
    }

    public async Task<string> ResumeInstanceAsync(
        DurableFunctionConfiguration config,
        string instanceId,
        string? reason = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["reason"] = reason ?? string.Empty,
            ["taskHub"] = config.TaskHub,
            ["connection"] = config.ConnectionName,
            ["code"] = config.AuthCode
        };

        var url = BuildUrl(config.FunctionAppUrl, $"/runtime/webhooks/durabletask/instances/{instanceId}/resume", queryParams);
        return await ExecuteRequestAsync<string>(HttpMethod.Post, url, config) ?? string.Empty;
    }

    public async Task<PagedResponse<List<EntityInfo>>> GetEntitiesAsync(
        DurableFunctionConfiguration config,
        string entityName,
        DateTime? lastOperationTimeFrom = null,
        DateTime? lastOperationTimeTo = null,
        bool fetchState = false,
        int? top = null,
        string? continuationToken = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["taskHub"] = config.TaskHub,
            ["connection"] = config.ConnectionName,
            ["code"] = config.AuthCode
        };

        if (lastOperationTimeFrom.HasValue)
        {
            DateTime utcTime = await _timeZoneService.ConvertLocalToUtcAsync(lastOperationTimeFrom.Value);
            queryParams["lastOperationTimeFrom"] = utcTime.ToString("o");
        }
        if (lastOperationTimeTo.HasValue)
        {
            DateTime utcTime = await _timeZoneService.ConvertLocalToUtcAsync(lastOperationTimeTo.Value);
            queryParams["lastOperationTimeTo"] = utcTime.ToString("o");
        }
        queryParams["fetchState"] = fetchState.ToString().ToLower();
        if (top.HasValue)
        {
            queryParams["top"] = top.Value.ToString();
        }

        var url = BuildUrl(config.FunctionAppUrl, $"/runtime/webhooks/durabletask/entities/{entityName}", queryParams);
        return await ExecutePagedRequestAsync<List<EntityInfo>>(HttpMethod.Get, url, config, continuationToken);
    }

    public async Task<string> GetEntityAsync(
        DurableFunctionConfiguration config,
        string entityName,
        string entityKey)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["taskHub"] = config.TaskHub,
            ["connection"] = config.ConnectionName,
            ["code"] = config.AuthCode
        };

        var url = BuildUrl(config.FunctionAppUrl, $"/runtime/webhooks/durabletask/entities/{entityName}/{entityKey}", queryParams);
        return await ExecuteRequestAsync<string>(HttpMethod.Get, url, config) ?? string.Empty;
    }

    private static string BuildUrl(string baseUrl, string path, Dictionary<string, string> queryParams)
    {
        var trimmedBaseUrl = baseUrl.TrimEnd('/');
        var query = string.Join("&", queryParams.Where(kv => !string.IsNullOrEmpty(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

        return string.IsNullOrEmpty(query) ? $"{trimmedBaseUrl}{path}" : $"{trimmedBaseUrl}{path}?{query}";
    }

    private void AddCustomHeaders(HttpRequestMessage request, DurableFunctionConfiguration config)
    {
        if (config.CustomHeaders == null || config.CustomHeaders.Count == 0)
        {
            return;
        }

        foreach (var header in config.CustomHeaders.Where(h => !string.IsNullOrWhiteSpace(h.Key) && !string.IsNullOrWhiteSpace(h.Value)))
        {
            try
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            catch
            {
                // Skip headers that can't be added
            }
        }
    }

    private async Task<PagedResponse<T>> ExecutePagedRequestAsync<T>(HttpMethod method, string url, DurableFunctionConfiguration config, string? continuationToken = null)
    {
        var request = new HttpRequestMessage(method, url);

        // Add custom headers
        AddCustomHeaders(request, config);

        // Add continuation token to request header if provided
        if (!string.IsNullOrEmpty(continuationToken))
        {
            request.Headers.Add(HttpHeaderKeys.ContinuationToken, continuationToken);
        }

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Extract continuation token from response header
        string? nextContinuationToken = null;
        if (response.Headers.TryGetValues(HttpHeaderKeys.ContinuationToken, out IEnumerable<string>? values))
        {
            nextContinuationToken = values.FirstOrDefault();
        }

        response.EnsureSuccessStatusCode();

        try
        {
            T? data = JsonSerializer.Deserialize<T>(content, _jsonOptions);

            // Calculate row count based on data type
            int? rowCount = null;
            if (data is IEnumerable<object> enumerable)
            {
                rowCount = enumerable.Count();
            }

            return new PagedResponse<T>
            {
                Data = data!,
                ContinuationToken = nextContinuationToken,
                RowCount = rowCount
            };
        }
        catch (JsonException)
        {
            return new PagedResponse<T>
            {
                Data = default!,
                ContinuationToken = null,
                RowCount = null
            };
        }
    }

    private async Task<T?> ExecuteRequestAsync<T>(HttpMethod method, string url, DurableFunctionConfiguration config)
    {
        try
        {
            var request = new HttpRequestMessage(method, url);

            // Add custom headers
            AddCustomHeaders(request, config);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            // Process the content based on type T and response status
            // For string types, handle empty content and pretty-print JSON
            if (typeof(T) == typeof(string))
            {
                // Try to pretty-print JSON if possible
                try
                {
                    var jsonDoc = JsonDocument.Parse(content);
                    return (T)(object)JsonSerializer.Serialize(jsonDoc, _jsonOptions);
                }
                catch
                {
                    return (T)(object)content;
                }
            }

            // For other types, deserialize normally
            try
            {
                return JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }
            catch (JsonException)
            {
                throw;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}