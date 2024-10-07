using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public interface ISignDocTarget
{
    string DocId { get; init; }
    string Email { get; init; }
}
public record SignDocCommand(string DocId, string Email) : IRequest, ISignDocTarget;

public class SignDocHandler : IRequestHandler<SignDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly IDocDal _docDal;
    private readonly IUserOftaDal _userOftaDal;
    private readonly IExecuteSignToSignProviderService _executeSignToSignProviderService;

    public SignDocHandler(IDocBuilder builder, IDocWriter writer, IDocDal docDal, IUserOftaDal userOftaDal,
                          IExecuteSignToSignProviderService executeSignToSignProviderService)
    {
        _builder = builder;
        _writer = writer;
        _docDal = docDal;
        _userOftaDal = userOftaDal;
        _executeSignToSignProviderService = executeSignToSignProviderService;
    }

    public Task<Unit> Handle(SignDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty())
            .Member(x => x.Email, y => y.NotEmpty());

        //  BUILD
        IDocKey docKey = new DocModel { DocId = request.DocId };
        var doc = _docDal.GetData(docKey);

        if (doc == null)
        {
            throw new KeyNotFoundException("UploadedDocId or DocId not found");
        }

        var userOfta = _userOftaDal.GetData(request.Email)
                ?? throw new KeyNotFoundException("User Ofta not found");

        var executeSignToSignProviderRequest = new ExecuteSignToSignProviderRequest(doc, userOfta);
        var executesignToSignProviderResponse = _executeSignToSignProviderService.Execute(executeSignToSignProviderRequest);

        if (executesignToSignProviderResponse.Status != "Succes")
        {
            throw new KeyNotFoundException(executesignToSignProviderResponse.Message);
        }

        var aggregate = _builder
            .Load(doc)
            .Sign(request.Email)
            .AddJurnal(DocStateEnum.Signed, request.Email)
            .Build();

        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
    
}