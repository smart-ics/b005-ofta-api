using FluentValidation;
using MediatR;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record RemoveVisibilityPostCommand(string PostId, string VisibilityReff)
    : IRequest, IPostKey;

public class RemoveVisibilityPostHandler : IRequestHandler<RemoveVisibilityPostCommand>
{
    private readonly IPostBuilder _builder;
    private readonly IPostWriter _writer;
    private readonly IValidator<RemoveVisibilityPostCommand> _guard;

    public RemoveVisibilityPostHandler(IPostBuilder builder, 
        IPostWriter writer, 
        IValidator<RemoveVisibilityPostCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(RemoveVisibilityPostCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .RemoveVisibility(request.VisibilityReff)
            .Build();
        
        //  WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class RemoveVisibilityPostGuard : AbstractValidator<RemoveVisibilityPostCommand>
{
    public RemoveVisibilityPostGuard()
    {
        RuleFor(x => x.PostId).NotEmpty();
        RuleFor(x => x.VisibilityReff).NotEmpty();
    }
}