using FluentValidation.TestHelper;
using Ofta.Application.DocContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.BlueprintAgg;

namespace Ofta.Test.Application.DocContext.BlueprintAgg;

public class BlueprintSigneeValidatorTest
{
    private readonly BluePrintSigneeValidator _sut;

    public BlueprintSigneeValidatorTest()
    {
        _sut = new BluePrintSigneeValidator();
    }
    
    [Fact]
    public void UserOftaId_ShouldNotEmpty()
    {
        var model = new BlueprintSigneeModel
        {
            UserOftaId = string.Empty
        };
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UserOftaId);    
    }

    [Fact]
    public void Email_ShouldNotEmpty()
    {
        var model = new BlueprintSigneeModel
        {
            Email = string.Empty
        };
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);    
    }   
}