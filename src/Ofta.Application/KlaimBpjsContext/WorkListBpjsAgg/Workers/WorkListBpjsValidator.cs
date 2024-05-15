using FluentValidation;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;


namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;

public class WorkListBpjsValidator : AbstractValidator<WorkListBpjsModel>
{
    public WorkListBpjsValidator()
    {
        RuleFor(x => x.OrderKlaimBpjsId).NotEmpty();
        RuleFor(x => x.OrderKlaimBpjsDate).NotEmpty();
        RuleFor(x => x.RegId).NotEmpty();
        RuleFor(x => x.PasienId).NotEmpty();
        RuleFor(x => x.PasienName).NotEmpty();
        RuleFor(x => x.NoSep).NotEmpty();
        RuleFor(x => x.LayananName).NotEmpty();
        RuleFor(x => x.DokterName).NotEmpty();
    }

}