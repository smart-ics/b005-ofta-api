using FluentValidation;
using MediatR;
using Nuna.Lib.DataTypeExtension;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record AddVisibilityPostCommand(string PostId, string VisibilityReff)
    : IRequest, IPostKey;

public record AddVisibilityPostHandler : IRequestHandler<AddVisibilityPostCommand>
{
    private readonly IPostBuilder _builder;
    private readonly IPostWriter _writer;
    private readonly IValidator<AddVisibilityPostCommand> _guard;

    public AddVisibilityPostHandler(IPostBuilder builder, 
        IPostWriter writer, 
        IValidator<AddVisibilityPostCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(AddVisibilityPostCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .AddVisibility(request.VisibilityReff)
            .Build();
        
        //  WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class AddVisibilityPostGuard : AbstractValidator<AddVisibilityPostCommand>
{
    public AddVisibilityPostGuard()
    {
        RuleFor(x => x.PostId).NotEmpty();
        RuleFor(x => x.VisibilityReff).NotEmpty();
    }
}