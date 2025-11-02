namespace DfxConsole.BlazorUI.Constants;

public static class RuntimeStatus
{
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Canceled = "Canceled";
    public const string Terminated = "Terminated";
    public const string Pending = "Pending";

    public static List<string> GetAll() => new()
    {
        Running, Completed, Failed, Canceled, Terminated, Pending
    };
}