using Dawn;
using MediatR;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.UseCases;

public record TilakaGetEmailByRegistrationIdQuery(string RegistrationId): IRequest<TilakaGetEmailByRegistrationIdResponse>, ITilakaRegistrationKey;

public record TilakaGetEmailByRegistrationIdResponse(string Email);

public class TilakaGetEmailByRegistrationIdHandler: IRequestHandler<TilakaGetEmailByRegistrationIdQuery, TilakaGetEmailByRegistrationIdResponse>
{
    private readonly ITilakaUserBuilder _builder;

    public TilakaGetEmailByRegistrationIdHandler(ITilakaUserBuilder builder)
    {
        _builder = builder;
    }

    public Task<TilakaGetEmailByRegistrationIdResponse> Handle(TilakaGetEmailByRegistrationIdQuery request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.RegistrationId, y => y.NotEmpty());

        // BUILD
        var aggregate = _builder
            .Load(request)
            .Build();

        // RESPONSE
        return Task.FromResult(new TilakaGetEmailByRegistrationIdResponse(aggregate.Email));
    }
}