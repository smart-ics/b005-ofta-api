namespace Ofta.Application.Helpers;

public interface IAppSettingService
{
    string RemoteCetakAddress { get; }
    string OftaMyDocWebUrl { get; }
    int UserRegistrationExpiration { get; }
    TilakaSignPosition SignPositionLeft { get; }
    TilakaSignPosition SignPositionCenter { get; }
    TilakaSignPosition SignPositionRight { get; }
    TilakaSignPosition SignPositionResep { get; }
}

public record TilakaSignPosition(int Width, int Height, int CoordinateX, int CoordinateY, int PageNumber);