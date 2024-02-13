using FluentValidation;
using FluentValidation.Results;
using Moq;
using Nuna.Lib.AutoNumberHelper;
using Ofta.Application.DocContext.BlueprintAgg.Contracts;
using Ofta.Application.DocContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.BlueprintAgg;

namespace Ofta.Test.Application.DocContext.BlueprintAgg;

public class BlueprintWriterTest
{
    private readonly BlueprintWriter _sut;
    private readonly Mock<IBlueprintDal> _blueprintDal;
    private readonly Mock<IBlueprintDocTypeDal> _blueprintDocTypeDal;
    private readonly Mock<IBlueprintSigneeDal> _blueprintSigneeDal;
    private readonly Mock<IValidator<BlueprintModel>> _validator;
    private readonly Mock<INunaCounterBL> _counter;
    

    public BlueprintWriterTest()
    {
        _blueprintDal = new Mock<IBlueprintDal>();
        _blueprintDocTypeDal = new Mock<IBlueprintDocTypeDal>();
        _blueprintSigneeDal = new Mock<IBlueprintSigneeDal>();
        _validator = new Mock<IValidator<BlueprintModel>>();
        _counter = new Mock<INunaCounterBL>();
        _sut = new BlueprintWriter(_blueprintDal.Object,
            _blueprintDocTypeDal.Object,
            _blueprintSigneeDal.Object,
            _validator.Object,
            _counter.Object);
    }
    
    [Fact]
    public void Save_WhenModelIsValid_ShouldSave()
    {
        //  ARRANGE
        var model = new BlueprintModel
        {
            ListDocType = new List<BlueprintDocTypeModel>()
        };
        _validator.Setup(x => x.Validate(model)).Returns(new ValidationResult());
        
        //  ACT
        _sut.Save(model);
        
        //  ASSERT
        _blueprintDal.Verify(x => x.GetData(model), Times.Once);
        _blueprintDal.Verify(x => x.Insert(model), Times.Once);
        _blueprintDocTypeDal.Verify(x => x.Delete(model), Times.Once);
        _blueprintSigneeDal.Verify(x => x.Delete(model), Times.Once);
        _blueprintDocTypeDal.Verify(x => x.Insert(model.ListDocType), Times.Once);
    }

}