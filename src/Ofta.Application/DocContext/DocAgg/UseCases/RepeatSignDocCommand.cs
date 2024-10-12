using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Polly;


namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record RepeatSignDocCommand(string DocId, string UserOftaId) :
    IRequest, IDocKey, IUserOftaKey;

public class RepeatSignDocHandler : IRequestHandler<RepeatSignDocCommand>
{
    private readonly IDocBuilder _docBuilder;
    private readonly IDocWriter _docWriter;
    private readonly IKlaimBpjsPrintOutDal _klaimBpjsPrintOutDal;
    private readonly IKlaimBpjsBuilder _klaimBpjsBuilder;
    private readonly IKlaimBpjsWriter _klaimBpjsWriter;
    private readonly ICopyFileService _copyFileService;
    private readonly IMediator _mediator;


    public RepeatSignDocHandler(IDocBuilder docBuilder,
                                IDocWriter docWriter,
                                IKlaimBpjsPrintOutDal klaimBpjsPrintOutDal,
                                IKlaimBpjsBuilder klaimBpjsBuilder,
                                IKlaimBpjsWriter klaimBpjsWriter,
                                ICopyFileService copyFileService,
                                IMediator mediator)
    {
        _docBuilder = docBuilder;
        _docWriter = docWriter;
        _klaimBpjsPrintOutDal = klaimBpjsPrintOutDal;
        _klaimBpjsBuilder = klaimBpjsBuilder;
        _klaimBpjsWriter= klaimBpjsWriter;
        _mediator = mediator;   
        _copyFileService = copyFileService;

    }

    public Task<Unit> Handle(RepeatSignDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty())
            .Member(x => x.UserOftaId, y => y.NotEmpty());

        //  BUILD
        var aggdoc = _docBuilder
        .Load(request)
        .AddJurnal(DocStateEnum.ReSign, string.Empty)
        .Build();

        var klaimBpjsPrintOut = _klaimBpjsPrintOutDal.GetData(request) ??
            new KlaimBpjsPrintOutModel();

        var klaimBpjs = _klaimBpjsBuilder
                .Load((IKlaimBpjsKey)klaimBpjsPrintOut)
                .Build();

        var printOut = klaimBpjs.ListDocType
                    .SelectMany(x => x.ListPrintOut)
                    .FirstOrDefault(x => x.DocId == request.DocId)
                    ?? new KlaimBpjsPrintOutModel();

        var fallback = Policy<DocModel>
          .Handle<KeyNotFoundException>()
          .Fallback(() => _docBuilder
              .Create()
              .DocType(new DocTypeModel(aggdoc.DocTypeId))
              .User(aggdoc)
              .AddJurnal(DocStateEnum.Created, string.Empty)
              .Build());
        var doc = fallback.Execute(() =>
            _docBuilder.Load(new DocModel()).Build());


        //  WRITE
        //      doc
        doc = _docBuilder
            .Attach(doc)
            .AddJurnal(DocStateEnum.Submited, string.Empty)
            .Build();
        doc = _docWriter.Save(doc);
        doc = _docBuilder.Attach(doc)
            .GenRequestedDocUrl()
            .Build();
        doc = _docWriter.Save(doc);
        
        //      add signee
        foreach (var signee in aggdoc.ListSignees)
        {
            doc = _docBuilder
                .Attach(doc)
                .AddSignee(
                new UserOftaModel(signee.UserOftaId),
                signee.SignTag,
                signee.SignPosition,
                signee.SignPositionDesc,
                signee.SignUrl)
                .Build();
        }
        doc = _docWriter.Save(doc);

        //      klaim
        printOut.DocId = doc.DocId;
        printOut.DocUrl = doc.RequestedDocUrl;
        _klaimBpjsWriter.Save(klaimBpjs);

        //      Copy Paste fisik file;
        var copyFileRequest = new CopyFileRequest(doc.RequestedDocUrl, aggdoc.RequestedDocUrl);
        _ = _copyFileService.Execute(copyFileRequest);

        _mediator.Publish(new RepeatSignDocEvent(doc, request), cancellationToken);
        return Task.FromResult(Unit.Value);
    }
}

public record RepeatSignDocEvent(
    DocModel Agg,
    RepeatSignDocCommand Command) : INotification;