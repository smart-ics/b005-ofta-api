using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record PublishDocCommand(string DocId) : IRequest;

public class PublishDocHandler : IRequestHandler<PublishDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly IDocDal _docDal;
    private readonly ICheckSignStatusFromSignProviderService _checkSignStatusFromSignProviderService;
    private readonly IDownloadPublishedDocFromProviderService _downloader;

    public PublishDocHandler(IDocBuilder builder, 
        IDocWriter writer, 
        IDocDal docDal, 
        ICheckSignStatusFromSignProviderService checkSignStatusFromSignProviderService,
        IDownloadPublishedDocFromProviderService downloader)
    {
        _builder = builder;
        _writer = writer;
        _docDal = docDal;
        _checkSignStatusFromSignProviderService = checkSignStatusFromSignProviderService;
        _downloader = downloader;
    }

    public Task<Unit> Handle(PublishDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty());

        //  BUILD
        IDocKey docKey = new DocModel { DocId = request.DocId };
        var doc = _docDal.GetData(docKey);

        if (doc == null)
        {
            throw new KeyNotFoundException("UploadedDocId or DocId not found");
        }

        var aggregate = _builder
            .Load(doc)
            .GenPublishedDocUrl()
            .Build();

        //checksign
        var checkSignFromSignProviderRequest = new CheckSignStatusFromSignProviderRequest(doc);
        var checksignFromSignProviderResponse = _checkSignStatusFromSignProviderService.Execute(checkSignFromSignProviderRequest);

        var downloadUrl = checksignFromSignProviderResponse.DownloadUrl;

        if (checksignFromSignProviderResponse == null )
        {
            throw new KeyNotFoundException("Doc Not Ready");
        }

        var downloadReq = new DownloadPublishedDocFromProviderRequest(
            checksignFromSignProviderResponse.DownloadUrl, aggregate.PublishedDocUrl);
        _downloader.Execute(downloadReq);
        
        aggregate = _builder
            .Attach(aggregate)
            .AddJurnal(DocStateEnum.Published, string.Empty)
            .UploadedDocUrl(downloadUrl)
            .Build();
        
        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}