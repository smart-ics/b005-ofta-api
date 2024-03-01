using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record PublishDocCommand(string UploadedDocId, string DownloadUrl) : IRequest;

public class PublishDocHandler : IRequestHandler<PublishDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly IDocDal _docDal;
    private readonly IDownloadPublishedDocFromProviderService _downloader;

    public PublishDocHandler(IDocBuilder builder, 
        IDocWriter writer, 
        IDocDal docDal, 
        IDownloadPublishedDocFromProviderService downloader)
    {
        _builder = builder;
        _writer = writer;
        _docDal = docDal;
        _downloader = downloader;
    }

    public Task<Unit> Handle(PublishDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.UploadedDocId, y => y.NotEmpty())
            .Member(x => x.DownloadUrl, y => y.NotEmpty());
        
        //  BUILD
        IUploadedDocKey uploadedDocKey = new DocModel{UploadedDocId = request.UploadedDocId};
        var doc = _docDal.GetData(uploadedDocKey)
            ?? throw new KeyNotFoundException("UploadedDocId not found");
        var aggregate = _builder
            .Load(doc)
            .UploadedDocUrl(request.DownloadUrl)
            .GenPublishedDocUrl()
            .Build();

        var downloadReq = new DownloadPublishedDocFromProviderRequest(
            aggregate.UploadedDocUrl, aggregate.PublishedDocUrl);
        _downloader.Execute(downloadReq);
        
        aggregate = _builder
            .Attach(aggregate)
            .AddJurnal(DocStateEnum.Published, string.Empty)
            .Build();
        
        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}