using FluentValidation;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;

public class BlueprintValidator : AbstractValidator<BlueprintModel>
{
    public BlueprintValidator(IValidator<BlueprintDocTypeModel> blueprintDocTypeValidator)
    {
        RuleFor(x => x.BlueprintName).NotEmpty();
        RuleFor(x => x.ListDocType).NotNull();
        RuleForEach(x => x.ListDocType).SetValidator(blueprintDocTypeValidator);
    }
}
 
public class BluePrintDocTypeValidator : AbstractValidator<BlueprintDocTypeModel>
{
    public BluePrintDocTypeValidator(IValidator<BlueprintSigneeModel> blueprintSigneeValidator)
    {
        RuleFor(x => x.DocTypeId).NotEmpty();
        RuleFor(x => x.DocTypeName).NotEmpty();
        RuleFor(x => x.ListSignee).NotNull();
        RuleForEach(x => x.ListSignee).SetValidator(blueprintSigneeValidator);
    }
}

public class BluePrintSigneeValidator : AbstractValidator<BlueprintSigneeModel>
{
    public BluePrintSigneeValidator()
    {
        RuleFor(x => x.UserOftaId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
    }
}
