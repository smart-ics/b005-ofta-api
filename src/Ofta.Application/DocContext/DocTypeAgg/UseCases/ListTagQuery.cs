using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;


namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record ListTagQuery() : IRequest<IEnumerable<ListTagResponse>>;

public record ListTagResponse(string Tag);

public class ListTagHandler : IRequestHandler<ListTagQuery, IEnumerable<ListTagResponse>>
{
    private readonly IDocTypeTagDal _docTypeTagDal;

    public ListTagHandler(IDocTypeTagDal docTypeTagDal)
    {
        _docTypeTagDal = docTypeTagDal;
    }

    public Task<IEnumerable<ListTagResponse>> Handle(ListTagQuery request, CancellationToken cancellationToken)
    {
        
        var listTag = _docTypeTagDal.ListData()?.ToList() ?? new List<DocTypeTagModel>();

        var response = listTag.Select(x => new ListTagResponse(x.Tag));
        return Task.FromResult(response);
    }
}