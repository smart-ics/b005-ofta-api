using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Test.Application.DocContext.BlueprintAgg;

public class BlueprintDocTypeValidatorTest
{
    private readonly BluePrintDocTypeValidator _sut;
    private readonly Mock<IValidator<BlueprintSigneeModel>> _blueprintSigneeValidator;

    public BlueprintDocTypeValidatorTest()
    {
        _blueprintSigneeValidator = new Mock<IValidator<BlueprintSigneeModel>>();
        _sut = new BluePrintDocTypeValidator(_blueprintSigneeValidator.Object);
    }
    
    [Fact]
    public void DocTypeId_ShouldNotEmpty()
    {
        var model = new BlueprintDocTypeModel
        {
            DocTypeId = string.Empty
        };
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DocTypeId);    
    }
    
    [Fact]
    public void DocTypeName_ShouldNotEmpty()
    {
        var model = new BlueprintDocTypeModel
        {
            DocTypeName = string.Empty
        };
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DocTypeName);    
    }
    
    [Fact]
    public void ListSignee_ShouldNotNull()
    {
        var model = new BlueprintDocTypeModel
        {
            ListSignee = null!
        };
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ListSignee);    
    }
    
}