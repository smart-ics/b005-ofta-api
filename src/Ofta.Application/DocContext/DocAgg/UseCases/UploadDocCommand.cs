using Dawn;
using MediatR;
using Nuna.Lib.DataTypeExtension;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record UploadDocCommand(string DocId, string Email) : IRequest, IDocKey;

public class UploadDocHandler : IRequestHandler<UploadDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly ICopyFileService _copyFileService;
    private readonly ISendToSignProviderService _sendToSignProviderService;
    private readonly IReqSignToSignProviderService _reqSignToSignProviderService;

    public UploadDocHandler(IDocBuilder builder, IDocWriter writer, ICopyFileService copyFileService, ISendToSignProviderService sendToSignProviderService, IReqSignToSignProviderService reqSignToSignProviderService)
    {
        _builder = builder;
        _writer = writer;
        _copyFileService = copyFileService;
        _sendToSignProviderService = sendToSignProviderService;
        _reqSignToSignProviderService = reqSignToSignProviderService;
    }

    public Task<Unit> Handle(UploadDocCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty())
            .Member(x => x.Email, y => y.NotEmpty());
        
        // BUILD
        var aggregate = _builder
            .Load(request)
            .Build();
        
        //  BUILD
        var sendToSignProviderRequest = new SendToSignProviderRequest(aggregate);
        var sendToSignProviderResponse = _sendToSignProviderService.Execute(sendToSignProviderRequest);
        var uploadedDocId = sendToSignProviderResponse.UploadedDocId;
        
        var signee = aggregate.ListSignees.FirstOrDefault(x => x.Email == request.Email) ?? new DocSigneeModel();
        var reqSignToSignProviderRequest = new ReqSignToSignProviderRequest(aggregate, signee, uploadedDocId);
        var reqSignToSignProviderResponse = _reqSignToSignProviderService.Execute(reqSignToSignProviderRequest);

        if (!reqSignToSignProviderResponse.Success)
            throw new ArgumentException($"Tilaka: {reqSignToSignProviderResponse.Message}.");

        if (reqSignToSignProviderResponse?.Signees != null)
        {
            foreach (var updatedSignee in reqSignToSignProviderResponse.Signees)
            {
                var originalSignee = aggregate.ListSignees
                    .FirstOrDefault(s => s.UserOftaId == updatedSignee.UserOftaId || s.Email == updatedSignee.Email);

                if (originalSignee != null)
                    originalSignee.SignUrl = updatedSignee.SignUrl;
            }
        }

        if (aggregate.DocState != DocStateEnum.Uploaded) 
            aggregate = _builder
                .Attach(aggregate)
                .AddJurnal(DocStateEnum.Uploaded, request.Email)
                .UploadedDocId(uploadedDocId)
                .Build();
        
        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}