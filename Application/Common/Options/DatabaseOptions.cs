namespace Application.Common.Options;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string MigrationsAssembly { get; set; } = string.Empty;

    public int QueryTimeoutDuration { get; set; }

    public bool EnableQueryTimeout { get; set; }
}