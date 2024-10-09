using FluentValidation;
using Ofta.Domain.DocContext.DraftOrderAgg;

namespace Ofta.Application.DocContext.DraftOrderAgg.Workers;

public class DraftOrderValidator: AbstractValidator<DraftOrderModel>
{
    public DraftOrderValidator()
    {
        RuleFor(x => x.DocTypeId).NotEmpty();
        RuleFor(x => x.DocTypeName).NotEmpty();
        RuleFor(x => x.DrafterUserId).NotEmpty();
        RuleFor(x => x.RequesterUserId).NotEmpty();
    }
}