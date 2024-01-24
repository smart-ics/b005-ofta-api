namespace Ofta.Infrastructure.Helpers;

public class SignLayout
{
    public int SignPosition { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int w { get; set; }
    public int h { get; set; }
}

public class TekenAjaProviderOptions
{
    public const string SECTION_NAME = "TekenAjaProvider";    
    public string UploadEnpoint { get; set; }
    public string ApiKey { get; set; }
    public int ValidityPeriod { get; set; }
    public List<SignLayout> SignLayout { get; set; }
}