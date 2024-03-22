using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record ReOrderDocKlaimBpjsCommand(string KlaimBpjsId, int NoUrutAsal, int NoUrutTujuan) :
    IRequest, IKlaimBpjsKey;

public class ReOrderDocKlaimBpjsHandler : IRequestHandler<ReOrderDocKlaimBpjsCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<ReOrderDocKlaimBpjsCommand> _guard;
    private KlaimBpjsModel _agg = new();

    public ReOrderDocKlaimBpjsHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<ReOrderDocKlaimBpjsCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(ReOrderDocKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        if (request.NoUrutAsal == request.NoUrutTujuan)
            return Task.FromResult(Unit.Value);

        //  BUILD
        _agg = _builder
            .Load(request)
            .Build();
        if (!RecordEvent(request))
            return Task.FromResult(Unit.Value);
        ProsesGeser(request);        
        
        //  WRITE
        _writer.Save(_agg);
        return Task.FromResult(Unit.Value);
    }

    private bool RecordEvent(ReOrderDocKlaimBpjsCommand request)
    {
        var doc = _agg.ListDoc.FirstOrDefault(x => x.NoUrut == request.NoUrutAsal);
        if (doc is null)
            return false;
        _agg = _builder
            .Attach(_agg)
            .AddEvent(KlaimBpjsStateEnum.InProgress, 
                $"ReOrder {doc.DocTypeName} ({request.NoUrutAsal}) -> ({request.NoUrutTujuan})")
            .Build();
        
        return true;
    }

    private void ProsesGeser(ReOrderDocKlaimBpjsCommand request)
    {
        var selisih = Math.Abs(request.NoUrutTujuan - request.NoUrutAsal) + 1;
        var pointer = request.NoUrutAsal;
        for (var i = 1; i < selisih; i++)
        {
            if (request.NoUrutAsal < request.NoUrutTujuan)
            {
                MoveRight(pointer);
                pointer++;
            }
            else
            {
                MoveLeft(pointer);
                pointer--;
            }
        }
    }
    
    private void MoveRight(int noUrut)
    {
        var current = _agg.ListDoc.FirstOrDefault(x => x.NoUrut == noUrut);
        if (current is null)
            return;
        var next = _agg.ListDoc.FirstOrDefault(x => x.NoUrut == noUrut + 1);
        if (next is null)
            return;

        current.NoUrut = noUrut + 1;
        next.NoUrut = noUrut;
    }
    
    private void MoveLeft(int noUrut)
    {
        var current = _agg.ListDoc.FirstOrDefault(x => x.NoUrut == noUrut);
        if (current is null)
            return;
        var next = _agg.ListDoc.FirstOrDefault(x => x.NoUrut == noUrut - 1);
        if (next is null)
            return;

        current.NoUrut = noUrut - 1;
        next.NoUrut = noUrut;
    }
}

public class ReOrderKlaimBpjsGuard : AbstractValidator<ReOrderDocKlaimBpjsCommand>
{
    public ReOrderKlaimBpjsGuard()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.NoUrutAsal).GreaterThan(0);
        RuleFor(x => x.NoUrutTujuan).GreaterThan(0);
    }
}
