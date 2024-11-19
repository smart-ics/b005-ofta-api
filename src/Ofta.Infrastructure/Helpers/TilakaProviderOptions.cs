using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofta.Infrastructure.Helpers;

public class TilakaProviderOptions
{
    public const string SECTION_NAME = "TilakaProvider";
    public string BaseApiUrl { get; set; }
    public string TokenEndPoint { get; set; }
    public string UploadEndpoint { get; set; }
    public string ClientID { get; set; }
    public string SecretKey { get; set; }
    public string Reason { get; set; }
    public string Location { get; set; }
    public int YearExpiration { get; set; }
    public string CompanyName { get; set; }
    public string ConsentText { get; set; }
    public string Version { get; set; }
    public IEnumerable<SignPositionLayout> SignPositionLayout { get; set; }
}

public class SignPositionLayout
{
    public int SignPosition { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int CoordinateX { get; set; }
    public int CoordinateY { get; set; }
    public int PageNumber { get; set; }
}