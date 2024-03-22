namespace Ofta.Infrastructure.Helpers;

public class RemoteCetakOptions
{
    public const string SECTION_NAME = "RemoteCetak";
    
    public string RemoteAddr { get; set; }
    public string ServerName { get; set; }
    public string DbName { get; set; }
}