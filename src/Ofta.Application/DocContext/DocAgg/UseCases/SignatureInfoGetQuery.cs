using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Contracts;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record SignatureInfoGetQuery(string DocumentId): IRequest<SignatureInfoGetResponse>;

public record SignerGetResponse(string Name, string DateTime, string Reason, string Location);

public record SignatureInfoGetResponse(string FileName, IEnumerable<SignerGetResponse> ListSigner);

public class SignatureInfoGetHandler: IRequestHandler<SignatureInfoGetQuery, SignatureInfoGetResponse>
{
    private readonly IGetSignatureInfoService _getSignatureInfo;
    private readonly ITilakaUserDal _tilakaUserDal;

    public SignatureInfoGetHandler(IGetSignatureInfoService getSignatureInfo, ITilakaUserDal tilakaUserDal)
    {
        _getSignatureInfo = getSignatureInfo;
        _tilakaUserDal = tilakaUserDal;
    }

    public Task<SignatureInfoGetResponse> Handle(SignatureInfoGetQuery request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.DocumentId, y => y.NotEmpty());
        
        // BUILD
        var signatureInfo = _getSignatureInfo.Execute(new GetSignatureInfoRequest(request.DocumentId));
        var signers = signatureInfo.Signers.Select(BuildSignerItemResponse).ToList();
        var response = new SignatureInfoGetResponse(signatureInfo.Filename, signers);
        
        // RESPONSE
        return Task.FromResult(response);
    }

    private SignerGetResponse BuildSignerItemResponse(SignerResponse signer)
    {
        var tilakaUser = _tilakaUserDal.GetData(signer)
            ?? throw new KeyNotFoundException($"Tilaka user with tilaka name: {signer.TilakaName} not found");

        return new SignerGetResponse(tilakaUser.UserOftaName, signer.DateTime, signer.Reason, signer.Location);
    }
}