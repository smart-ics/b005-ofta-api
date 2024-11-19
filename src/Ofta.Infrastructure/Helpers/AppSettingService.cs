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
    public int UserExpirationTime => _tilakaOptions.YearExpiration;

    public TilakaSignPosition SignPositionLeft => GetSignPositionLeft();
    public TilakaSignPosition SignPositionCenter => GetSignPositionCenter();
    public TilakaSignPosition SignPositionRight => GetSignPositionRight();

    private TilakaSignPosition GetSignPositionLeft() 
    {
        var signPosition = _tilakaOptions.SignPositionLayout.First(x => x.SignPosition == 0);
        return new TilakaSignPosition(signPosition.Width, signPosition.Height, signPosition.CoordinateX, signPosition.CoordinateY, signPosition.PageNumber);
    }

    private TilakaSignPosition GetSignPositionCenter()
    {
        var signPosition = _tilakaOptions.SignPositionLayout.First(x => x.SignPosition == 1);
        return new TilakaSignPosition(signPosition.Width, signPosition.Height, signPosition.CoordinateX, signPosition.CoordinateY, signPosition.PageNumber);
    }

    private TilakaSignPosition GetSignPositionRight()
    {
        var signPosition = _tilakaOptions.SignPositionLayout.First(x => x.SignPosition == 2);
        return new TilakaSignPosition(signPosition.Width, signPosition.Height, signPosition.CoordinateX, signPosition.CoordinateY, signPosition.PageNumber);
    }
}