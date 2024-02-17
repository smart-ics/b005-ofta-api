using FluentValidation;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;

public class OrderKlaimBpjsValidator : AbstractValidator<OrderKlaimBpjsModel>
{
    public OrderKlaimBpjsValidator()
    {
        RuleFor(x => x.UserOftaId).NotEmpty();
        RuleFor(x => x.RegId).NotEmpty();
        RuleFor(x => x.PasienId).NotEmpty();
        RuleFor(x => x.PasienName).NotEmpty();
        RuleFor(x => x.NoSep).NotEmpty();
        RuleFor(x => x.LayananName).NotEmpty();
        RuleFor(x => x.DokterName).NotEmpty();
    }
    
}