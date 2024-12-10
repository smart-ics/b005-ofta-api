using Dawn;
using MediatR;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Application.DocContext.BulkSignAgg.Workers;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.BulkSignAgg;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.UseCases;

public record DownloadDocBulkSignCommand(string BulkSignId) : IRequest, IBulkSignKey;

public class DownloadDocBulkSignHandler : IRequestHandler<DownloadDocBulkSignCommand>
{
    private readonly IBulkSignBuilder _builder;
    private readonly IDocBuilder _docBuilder;
    private readonly IDocWriter _writer;
    private readonly ICheckBulkSignStatusService _checkBulkSignStatusService;
    private readonly IDownloadBulkSignFileService _downloadBulkSignFileService;

    public DownloadDocBulkSignHandler(IBulkSignBuilder builder, IDocBuilder docBuilder, IDocWriter writer, ICheckBulkSignStatusService checkBulkSignStatusService, IDownloadBulkSignFileService downloadBulkSignFileService)
    {
        _builder = builder;
        _docBuilder = docBuilder;
        _writer = writer;
        _checkBulkSignStatusService = checkBulkSignStatusService;
        _downloadBulkSignFileService = downloadBulkSignFileService;
    }
    
    public Task<Unit> Handle(DownloadDocBulkSignCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.BulkSignId, y => y.NotEmpty());

        // BUILD
        var aggregate = _builder
            .Load(request)
            .Build();
        
        var checkSignStatusRequest = new CheckBulkSignStatusRequest(aggregate);
        var checkSignStatus = _checkBulkSignStatusService.Execute(checkSignStatusRequest);
        if (!checkSignStatus.Success)
            throw new ArgumentException("Signing still in progress");

        checkSignStatus.ListFiles.ForEach(file =>
        {
            var bulkSignDoc = aggregate.ListDoc.First(x => x.UploadedDocId == file.Filename);
            var doc = _docBuilder
                .Load(bulkSignDoc)
                .GenPublishedDocUrl()
                .Build();
            
            var downloadReq = new DownloadBulkSignFileRequest(
                file.DownloadUrl, doc.PublishedDocUrl);
            _downloadBulkSignFileService.Execute(downloadReq);
            
            doc = _docBuilder
                .Attach(doc)
                .AddJurnal(DocStateEnum.Published, string.Empty)
                .UploadedDocUrl(file.DownloadUrl)
                .Build();
            
            _writer.Save(doc);
        });
        
        //  WRITE
        return Task.FromResult(Unit.Value);
    }
}