using Dawn;
using MediatR;
using Nuna.Lib.DataTypeExtension;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

// public record UploadDocCommand(string DocId) : IRequest, IDocKey;
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
        var oldDoc = _builder
            .Load(request)
            .Build();
        
        var newDoc = _builder
            .Create()
            .DocDate(oldDoc.DocDate)
            .DocType(new DocTypeModel(oldDoc.DocTypeId))
            .User(oldDoc)
            .GenRequestedDocUrl()
            .AddJurnal(oldDoc.DocState, string.Empty)
            .Build();
        newDoc = _writer.Save(newDoc);

        oldDoc.ListSignees.ForEach(signee =>
        {
            newDoc = _builder
                .Attach(newDoc)
                .AddSignee(new UserOftaModel(signee.UserOftaId),
                    signee.SignTag,
                    signee.SignPosition,
                    signee.SignPositionDesc,
                    ""
                )
                .Build();
        });
        newDoc = _writer.Save(newDoc);

        oldDoc.ListScope.ForEach(scope =>
        {
            newDoc = _builder
                .Attach(newDoc)
                .AddScope((IScope)scope)
                .Build();
        });
        newDoc = _writer.Save(newDoc);
        
        var copyFileRequest = new CopyFileRequest(newDoc.RequestedDocUrl, oldDoc.RequestedDocUrl);
        _ = _copyFileService.Execute(copyFileRequest);
        
        _writer.Delete(oldDoc);
        
        //  BUILD
        // var uploadedDocId = aggregate.UploadedDocId;
        //
        // if (uploadedDocId.IsNullOrEmpty())
        // {
        //     var sendToSignProviderRequest = new SendToSignProviderRequest(aggregate);
        //     var sendToSignProviderResponse = _sendToSignProviderService.Execute(sendToSignProviderRequest);
        //     uploadedDocId = sendToSignProviderResponse.UploadedDocId ?? string.Empty;
        // }
        
        var sendToSignProviderRequest = new SendToSignProviderRequest(newDoc);
        var sendToSignProviderResponse = _sendToSignProviderService.Execute(sendToSignProviderRequest);
        var uploadedDocId = sendToSignProviderResponse.UploadedDocId;
        
        // var reqSignToSignProviderRequest = new ReqSignToSignProviderRequest(aggregate, uploadedDocId);
        var signee = newDoc.ListSignees.FirstOrDefault(x => x.Email == request.Email) ?? new DocSigneeModel();
        var reqSignToSignProviderRequest = new ReqSignToSignProviderRequest(newDoc, signee, uploadedDocId);
        var reqSignToSignProviderResponse = _reqSignToSignProviderService.Execute(reqSignToSignProviderRequest);

        if (!reqSignToSignProviderResponse.Success)
            throw new ArgumentException($"Tilaka: {reqSignToSignProviderResponse.Message}.");

        if (reqSignToSignProviderResponse?.Signees != null)
        {
            foreach (var updatedSignee in reqSignToSignProviderResponse.Signees)
            {
                var originalSignee = newDoc.ListSignees
                    .FirstOrDefault(s => s.UserOftaId == updatedSignee.UserOftaId || s.Email == updatedSignee.Email);

                if (originalSignee != null)
                    originalSignee.SignUrl = updatedSignee.SignUrl;
            }
        }

        if (newDoc.DocState != DocStateEnum.Uploaded) 
            newDoc = _builder
                .Attach(newDoc)
                .AddJurnal(DocStateEnum.Uploaded, string.Empty)
                .UploadedDocId(uploadedDocId)
                .Build();


        //  WRITE
        _writer.Save(newDoc);
        return Task.FromResult(Unit.Value);
    }
}