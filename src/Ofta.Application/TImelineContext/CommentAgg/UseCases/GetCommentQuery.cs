using Dawn;
using MediatR;
using Ofta.Application.TImelineContext.CommentAgg.Workers;
using Ofta.Domain.TImelineContext.CommentAgg;


namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record GetCommentQuery(string CommentId) :
    IRequest<GetCommentResponse>, ICommentKey;

public record GetCommentResponse(
    string CommentId,
    string CommentDate,
    string PostId,
    string UserOftaId,
    string UserOftaName,
    string Msg,
    int ReactCount,
    List<GetCommentResponseReact> ListReact
    );

public record GetCommentResponseReact(
    string CommentReactDate,
    string UserOftaId,
    string UserOftaName
    );

public class GetCommentQueryyHandler : IRequestHandler<GetCommentQuery, GetCommentResponse>
{
    private readonly ICommentBuilder _builder;

    public GetCommentQueryyHandler(ICommentBuilder builder)
    {
        _builder = builder;
    }

    public Task<GetCommentResponse> Handle(GetCommentQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.CommentId, x => x.NotEmpty());

        //  BUILD
        var comment = _builder
            .Load(request)
            .Build();

        //  PROJECTION
        var response = new GetCommentResponse(
                comment.CommentId,
                comment.CommentDate.ToString("yyyy-MM-dd"),
                comment.PostId,
                comment.UserOftaId,
                comment.UserOftaName,
                comment.Msg,
                comment.ReactCount,
                comment.ListReact.Select(x => new GetCommentResponseReact(
                    x.CommentReactDate.ToString("yyyy-MM-dd"),
                    x.UserOftaId,
                    x.UserOftaName
                )).ToList()
        );
        return Task.FromResult(response);
    }
}

