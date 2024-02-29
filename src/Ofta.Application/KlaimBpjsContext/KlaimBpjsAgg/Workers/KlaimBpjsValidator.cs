using FluentValidation;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public class KlaimBpjsValidator : AbstractValidator<KlaimBpjsModel>
{
    public KlaimBpjsValidator()
    {
        RuleFor(x => x.UserOftaId).NotEmpty();
    }
}