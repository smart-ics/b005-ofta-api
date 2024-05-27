using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsPrintOutScanCommand(string KlaimBpjsId) : IRequest, IKlaimBpjsKey;

public class KlaimBpjsPrintOutScanHandler : IRequestHandler<KlaimBpjsPrintOutScanCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<KlaimBpjsPrintOutScanCommand> _guard;
    private readonly IReffIdFinderFactory _finderFactory;
    private KlaimBpjsModel _agg;

    public KlaimBpjsPrintOutScanHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<KlaimBpjsPrintOutScanCommand> guard, 
        IReffIdFinderFactory finderFactory)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _finderFactory = finderFactory;
    }

    public Task<Unit> Handle(KlaimBpjsPrintOutScanCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        _agg = _builder
            .Load(request)
            .Build();
        var listReffIdPrintOut = GetListReffIdPrintOut(_agg); 
        _agg = listReffIdPrintOut
            .Aggregate(_agg, (current, item) => _builder
                .Attach(current)
                .AddPrintOut(new DocTypeModel(item.DocTypeId), item.ReffId)
                .Build());

        //  WRITE
        _ = _writer.Save(_agg);
        return Task.FromResult(Unit.Value);
    }

    private List<DocTypePrintOutDto> GetListReffIdPrintOut(KlaimBpjsModel klaimBpjs)
    {
        List<DocTypePrintOutDto> result = new();
        klaimBpjs.ListDocType.ForEach(docType =>
        {
            var listReffId = _finderFactory
                .Factory(docType,docType)
                .Find(klaimBpjs.RegId)
                .ToList();
            listReffId.ForEach(reffId =>
            {
                if (!string.IsNullOrWhiteSpace(reffId))
                {
                    result.Add(new DocTypePrintOutDto(docType.DocTypeId, reffId));
                }
            });
        });
        return result;
    }

    private class DocTypePrintOutDto
    {
        public DocTypePrintOutDto(string docTYpe, string reffId) => (DocTypeId, ReffId) = (docTYpe, reffId);
        public string DocTypeId { get; set; }
        public string ReffId { get; set; }
    }
}

public class ScanReffIdKlaimBpjsGuard : AbstractValidator<KlaimBpjsPrintOutScanCommand>
{
    public ScanReffIdKlaimBpjsGuard()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
    }
}