using FluentValidation;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.Workers;

public class DocValidator : AbstractValidator<DocModel>
{
    public DocValidator()
    {
        RuleFor(x => x.DocDate).NotEmpty();
        RuleFor(x => x.DocTypeId).NotEmpty();
        RuleFor(x => x.DocTypeName).NotEmpty();
        RuleFor(x => x.UserOftaId).NotEmpty();
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.DocState).IsInEnum();
    }
}