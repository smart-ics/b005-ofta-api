using System.Text.Json.Serialization;
using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.Contracts;

public interface IGetSignatureImageService: INunaService<GetSignatureImageResponse, GetSignatureImageRequest>
{
}

public record GetSignatureImageRequest(string TilakaName);

public record GetSignatureImageResponse(
    bool Success,
    string Message,
    ImageResponse Data
);

public record ImageResponse(string SignatureBase64, string Name);