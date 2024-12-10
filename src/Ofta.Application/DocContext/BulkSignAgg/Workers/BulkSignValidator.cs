using FluentValidation;
using Ofta.Domain.DocContext.BulkSignAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.Workers;

public class BulkSignValidator: AbstractValidator<BulkSignModel>
{
    public BulkSignValidator()
    {
        RuleFor(x => x.BulkSignDate).NotEmpty();
        RuleFor(x => x.UserOftaId).NotEmpty();
        RuleFor(x => x.ListDoc).NotEmpty();
    }
}