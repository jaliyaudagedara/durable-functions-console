namespace DfxConsole.BlazorUI.Models;

public class PagedResponse<T>
{
    public T Data { get; set; } = default!;

    public string? ContinuationToken { get; set; }

    public bool HasMoreResults => !string.IsNullOrEmpty(ContinuationToken);

    public int? RowCount { get; set; }
}
