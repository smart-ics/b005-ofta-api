using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofta.Infrastructure.Helpers;

public class TilakaProviderOptions
{
    public const string SECTION_NAME = "TilakaProvider";
    public string TokenEndPoint { get; set; }
    public string UploadEnpoint { get; set; }
    public string ClientID { get; set; }
    public string SecretKey { get; set; }
}