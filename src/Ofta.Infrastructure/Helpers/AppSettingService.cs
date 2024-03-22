using Microsoft.Extensions.Options;
using Ofta.Application.Helpers;

namespace Ofta.Infrastructure.Helpers;

public class AppSettingService : IAppSettingService
{
    private readonly RemoteCetakOptions _remoteCetakOptions;

    public AppSettingService(IOptions<RemoteCetakOptions> remoteCetakOptions)
    {
        _remoteCetakOptions = remoteCetakOptions.Value;
    }

    public string RemoteCetakAddress => _remoteCetakOptions.RemoteAddr;
}