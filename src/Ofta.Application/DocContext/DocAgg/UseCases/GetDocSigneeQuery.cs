using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;


namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record GetDocSigneeQuery(string DocId, string Email) : IRequest<GetDocSigneeResponse>, IDocKey;

public record GetDocSigneeResponse(
    string SigneeUserOftaId,
    string SigneeEmail,
    string SignTag,
    string SignPosition,
    int Level,
    string SignState,
    string SignedDate,
    string SignPositionDesc,
    string SignUrl);

public class GetDocSigneeQueryHandler : IRequestHandler<GetDocSigneeQuery, GetDocSigneeResponse>
{
    private readonly IDocSigneeDal _docSignee;

    public GetDocSigneeQueryHandler(IDocSigneeDal docSignee)
    {
        _docSignee = docSignee;
    }

    public Task<GetDocSigneeResponse> Handle(GetDocSigneeQuery request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, x => x.NotEmpty())
            .Member(x => x.Email, x => x.NotEmpty());

        // BUILD - Filtering by Email
        var docSigneeList = _docSignee.ListData(request)?.ToList()
            ?? new List<DocSigneeModel>();

        // Filtering the list based on the email from the request
        var filteredSignee = docSigneeList.FirstOrDefault(x => x.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        // Check if a matching signee is found
        if (filteredSignee == null)
        {
            throw new Exception($"No signee found for email: {request.Email}");
        }

        // PROJECTION
        var response = new GetDocSigneeResponse(
            filteredSignee.UserOftaId,
            filteredSignee.Email,
            filteredSignee.SignTag,
            filteredSignee.SignPosition.ToString(),
            filteredSignee.Level,
            filteredSignee.SignState.ToString(),
            filteredSignee.SignedDate.ToString("yyyy-MM-dd"),
            filteredSignee.SignPositionDesc,
            filteredSignee.SignUrl
        );

        return Task.FromResult(response);
    }
}
