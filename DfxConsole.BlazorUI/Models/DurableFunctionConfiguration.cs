namespace DfxConsole.BlazorUI.Models;

public class DurableFunctionConfiguration
{
    public string FunctionAppUrl { get; set; }

    public string AuthCode { get; set; }

    public string TaskHub { get; set; } = string.Empty;

    public string ConnectionName { get; set; } = string.Empty;

    public Dictionary<string, string> CustomHeaders { get; set; } = [];
}