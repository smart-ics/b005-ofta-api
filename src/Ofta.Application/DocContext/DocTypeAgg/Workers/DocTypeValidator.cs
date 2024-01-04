using FluentValidation;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.Workers;

public class DocTypeValidator : AbstractValidator<DocTypeModel>
{
    public DocTypeValidator()
    {
        RuleFor(x => x.DocTypeName).NotEmpty();
    }
}