﻿using Microsoft.Extensions.Options;
using Ofta.Application.Helpers;

namespace Ofta.Infrastructure.Helpers;

public class AppSettingService : IAppSettingService
{
    private readonly RemoteCetakOptions _remoteCetakOptions;
    private readonly OftaOptions _oftaOptions;
    private readonly TilakaProviderOptions _tilakaOptions;

    public AppSettingService(IOptions<RemoteCetakOptions> remoteCetakOptions, IOptions<OftaOptions> oftaOptions, IOptions<TilakaProviderOptions> tilakaOptions)
    {
        _remoteCetakOptions = remoteCetakOptions.Value;
        _oftaOptions = oftaOptions.Value;
        _tilakaOptions = tilakaOptions.Value;
    }

    public string RemoteCetakAddress => _remoteCetakOptions.RemoteAddr;
    public string OftaMyDocWebUrl => _oftaOptions.MyDocWebUrl;
    public int UserRegistrationExpirationTime => _tilakaOptions.DaysExpirationRegistration;

    public TilakaSignPosition SignPositionLeft
    {
        get
        {
            var signPosition = _tilakaOptions.SignPositionLayout.First(x => x.SignPosition == 0);
            return new TilakaSignPosition(signPosition.Width, signPosition.Height, signPosition.CoordinateX, signPosition.CoordinateY, signPosition.PageNumber);
        }
    }

    public TilakaSignPosition SignPositionCenter
    {
        get
        {
            var signPosition = _tilakaOptions.SignPositionLayout.First(x => x.SignPosition == 1);
            return new TilakaSignPosition(signPosition.Width, signPosition.Height, signPosition.CoordinateX, signPosition.CoordinateY, signPosition.PageNumber);
        }
    }

    public TilakaSignPosition SignPositionRight
    {
        get
        {
            var signPosition = _tilakaOptions.SignPositionLayout.First(x => x.SignPosition == 2);
            return new TilakaSignPosition(signPosition.Width, signPosition.Height, signPosition.CoordinateX, signPosition.CoordinateY, signPosition.PageNumber);
        }
    }

    public TilakaSignPosition SignPositionResep
    {
        get
        {
            var signPosition = _tilakaOptions.SignPositionLayoutResep;
            return new TilakaSignPosition(signPosition.Width, signPosition.Height, signPosition.CoordinateX, signPosition.CoordinateY, signPosition.PageNumber);
        }
    }
}