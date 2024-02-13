using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Ofta.Application.DocContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.BlueprintAgg;

namespace Ofta.Test.Application.DocContext.BlueprintAgg;

public class BlueprintValidatorTest
{
    private readonly BlueprintValidator _sut;
    private readonly Mock<IValidator<BlueprintDocTypeModel>> _blueprintDocTypeValidator;
    
    public BlueprintValidatorTest()
    {
        _blueprintDocTypeValidator = new Mock<IValidator<BlueprintDocTypeModel>>();
        _sut = new BlueprintValidator(_blueprintDocTypeValidator.Object);
    }
    
    [Fact]
    public void BlueprintName_ShouldNotEmpty()
    {
        var model = new BlueprintModel
        {
            BlueprintName = string.Empty
        };
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.BlueprintName);    
    }
    
    [Fact]
    public void ListDocType_ShouldNotNull()
    {
        var model = new BlueprintModel
        {
            ListDocType = null!
        };
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ListDocType);    
    }
    
    [Fact]
    public void ListDocType_ShouldNotEmpty()
    {
        var model = new BlueprintModel
        {
            ListDocType = new List<BlueprintDocTypeModel>()
        };
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ListDocType);    
    }
}