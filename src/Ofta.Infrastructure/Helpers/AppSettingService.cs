using Microsoft.Extensions.Options;
using Ofta.Application.Helpers;

namespace Ofta.Infrastructure.Helpers;

public class AppSettingService : IAppSettingService
{
    private readonly RemoteCetakOptions _remoteCetakOptions;
    private readonly OftaOptions _oftaOptions;

    public AppSettingService(IOptions<RemoteCetakOptions> remoteCetakOptions, IOptions<OftaOptions> oftaOptions)
    {
        _remoteCetakOptions = remoteCetakOptions.Value;
        _oftaOptions = oftaOptions.Value;
    }

    public string RemoteCetakAddress => _remoteCetakOptions.RemoteAddr;
    public string OftaMyDocWebUrl => _oftaOptions.MyDocWebUrl;
}