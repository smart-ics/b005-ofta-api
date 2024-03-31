using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record AttachDocPostCommand(string PostId, string DocId) :
    IRequest, IPostKey, IDocKey;

public class AttachDocPostHandler : IRequestHandler<AttachDocPostCommand>
{
    private readonly IPostBuilder _builder;
    private readonly IPostWriter _writer;
    private readonly IDocDal _docDal;
    private readonly IValidator<AttachDocPostCommand> _guard;

    public AttachDocPostHandler(IPostBuilder builder, 
        IPostWriter writer, 
        IDocDal docDal, 
        IValidator<AttachDocPostCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _docDal = docDal;
        _guard = guard;
    }

    public Task<Unit> Handle(AttachDocPostCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .AttachDoc(request)
            .Build();
        
        //  WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class AttachDocPostGuard : AbstractValidator<AttachDocPostCommand>
{
    public AttachDocPostGuard()
    {
        RuleFor(x => x.PostId).NotEmpty();
        RuleFor(x => x.DocId).NotEmpty();
    }
}