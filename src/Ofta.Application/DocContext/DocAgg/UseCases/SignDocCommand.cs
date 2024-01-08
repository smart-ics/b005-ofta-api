using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record SignDocCommand(string UploadedDocId, string Email) : IRequest;

public class SignDocHandler : IRequestHandler<SignDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly IDocDal _docDal;

    public SignDocHandler(IDocBuilder builder, IDocWriter writer, IDocDal docDal)
    {
        _builder = builder;
        _writer = writer;
        _docDal = docDal;
    }

    public Task<Unit> Handle(SignDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.UploadedDocId, y => y.NotEmpty())
            .Member(x => x.Email, y => y.NotEmpty());
        
        //  BUILD
        IUploadedDocKey uploadedDocKey = new DocModel{UploadedDocId = request.UploadedDocId};
        var doc = _docDal.GetData(uploadedDocKey)
            ?? throw new KeyNotFoundException("UploadedDocId not found");
        var aggregate = _builder
            .Load(doc)
            .Sign(request.Email)
            .DocState(DocStateEnum.Signed, request.Email)
            .Build();
        
        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}