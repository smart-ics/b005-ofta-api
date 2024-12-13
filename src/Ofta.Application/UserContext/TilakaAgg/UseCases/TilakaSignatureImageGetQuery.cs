using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.UseCases;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;

namespace Ofta.Application.UserContext.TilakaAgg.UseCases;

public record TilakaSignatureImageGetQuery(string Email): IRequest<TilakaSignatureImageGetResponse>;

public record TilakaSignatureImageGetResponse(string TilakaName, string Message);

public class SignatureImageGetHandler: IRequestHandler<TilakaSignatureImageGetQuery, TilakaSignatureImageGetResponse>
{
    private readonly IGetSignatureImageService _getSignatureImage;
    private readonly ITilakaUserBuilder _builder;

    public SignatureImageGetHandler(IGetSignatureImageService getSignatureImage, ITilakaUserBuilder builder)
    {
        _getSignatureImage = getSignatureImage;
        _builder = builder;
    }

    public Task<TilakaSignatureImageGetResponse> Handle(TilakaSignatureImageGetQuery request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.Email, y => y.NotEmpty());
        
        // BUILD
        var tilakaUser = _builder
            .Load(request.Email)
            .Build();
        
        var signatureImage = _getSignatureImage.Execute(new GetSignatureImageRequest(tilakaUser.TilakaName));
        if (!signatureImage.Success)
            throw new KeyNotFoundException(signatureImage.Message);
        
        // RESPONSE
        var response = new TilakaSignatureImageGetResponse(tilakaUser.TilakaName, signatureImage.Message);
        return Task.FromResult(response);
    }
}