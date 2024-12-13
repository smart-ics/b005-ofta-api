using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.UseCases;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;

namespace Ofta.Application.UserContext.TilakaAgg.UseCases;

public record TilakaSignatureImageGetQuery(string Email): IRequest<SignatureImageGetResponse>;

public record SignatureImageGetResponse(string TilakaName, string Message);

public class SignatureImageGetHandler: IRequestHandler<TilakaSignatureImageGetQuery, SignatureImageGetResponse>
{
    private readonly IGetSignatureImageService _getSignatureImage;
    private readonly ITilakaUserBuilder _builder;

    public SignatureImageGetHandler(IGetSignatureImageService getSignatureImage, ITilakaUserBuilder builder)
    {
        _getSignatureImage = getSignatureImage;
        _builder = builder;
    }

    public Task<SignatureImageGetResponse> Handle(TilakaSignatureImageGetQuery request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.Email, y => y.NotEmpty());
        
        // BUILD
        var tilakaUser = _builder
            .Load(request.Email)
            .Build();
        var signatureImage = _getSignatureImage.Execute(new GetSignatureImageRequest(tilakaUser.TilakaName));
        
        // RESPONSE
        var response = new SignatureImageGetResponse(tilakaUser.TilakaName, signatureImage.Message);
        return Task.FromResult(response);
    }
}