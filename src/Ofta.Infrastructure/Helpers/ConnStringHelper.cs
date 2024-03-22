namespace Ofta.Infrastructure.Helpers;

public static class ConnStringHelper
{
    private static string _connString = string.Empty;
    private const string USER_ID = "oftaLogin";
    private const string PASS = "ofta123!";
    
    public static string Get(DatabaseOptions options)
    {
        if (_connString.Length == 0)
            _connString = Generate(options.ServerName, options.DbName);

        return _connString;
    }

    private static string Generate(string server, string db)
    {
        var result = $"Server={server};Database={db};User Id={USER_ID};Password={PASS};";
        return result;
    }
}

public static class ConnStringHelperRemoteCetak
{
    private static string _connString = string.Empty;
    private const string USER_ID = "HospitalX";
    private const string PASS = "intersoftindo";
    
    public static string Get(RemoteCetakOptions options)
    {
        if (_connString.Length == 0)
            _connString = Generate(options.ServerName, options.DbName);

        return _connString;
    }

    private static string Generate(string server, string db)
    {
        var result = $"Server={server};Database={db};User Id={USER_ID};Password={PASS};";
        return result;
    }
}