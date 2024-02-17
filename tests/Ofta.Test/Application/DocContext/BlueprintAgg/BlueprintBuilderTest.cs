using FluentAssertions;
using Moq;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Test.Application.DocContext.BlueprintAgg;

public class BlueprintBuilderTest
{
    private readonly BlueprintBuilder _sut;
    private readonly Mock<IBlueprintDal> _blueprintDal;
    private readonly Mock<IBlueprintDocTypeDal> _blueprintDocTypeDal;
    private readonly Mock<IBlueprintSigneeDal> _blueprintSigneeDal;
    private readonly Mock<IDocTypeDal> _docTypeDal;
    private readonly Mock<IUserOftaDal> _userOftaDal;
    
    public BlueprintBuilderTest()
    {
        _blueprintDal = new Mock<IBlueprintDal>();
        _blueprintDocTypeDal = new Mock<IBlueprintDocTypeDal>();
        _blueprintSigneeDal = new Mock<IBlueprintSigneeDal>();
        _docTypeDal = new Mock<IDocTypeDal>();
        _userOftaDal = new Mock<IUserOftaDal>();
        
        _sut = new BlueprintBuilder(_blueprintDal.Object, 
            _blueprintSigneeDal.Object, 
            _blueprintDocTypeDal.Object, 
            _docTypeDal.Object, 
            _userOftaDal.Object);
    }
    
    [Fact]
    public  void Create_ListDocTypeShouldNotNull()
    {
        //  ACT
        var result = _sut.Create().Build();
        
        //  ASSERT
        result.ListDocType.Should().NotBeNull();
    }
    
    [Fact]
    public  void Load_ValidBlueprintKey_ShouldReturnBlueprintModel()
    {
        //  ARRANGE
        var blueprintKey = new Mock<IBlueprintKey>();
        _blueprintDal
            .Setup(x => x.GetData(It.IsAny<IBlueprintKey>()))
            .Returns(new BlueprintModel());
        
        //  ACT
        var result = _sut.Load(blueprintKey.Object).Build();
        
        //  ASSERT
        result.Should().NotBeNull();
    }
    
    [Fact]
    public  void Load_InvalidBlueprintKey_ShouldThrowKeyNotFoundEx()
    {
        //  ARRANGE
        var blueprintKey = new Mock<IBlueprintKey>();
        _blueprintDal
            .Setup(x => x.GetData(It.IsAny<IBlueprintKey>()))
            .Returns((null as BlueprintModel)!);
        
        //  ACT 
        var action = () => {_sut.Load(blueprintKey.Object).Build();};
        
        //  ASSERT
        action.Should().Throw<KeyNotFoundException>();
    }
    
    [Fact]
    public void AddDocType_ValidDocTypeKey_ShouldReturnBlueprintModel()
    {
        //  ARRANGE
        var docTypeKey = new Mock<IDocTypeKey>();
        _docTypeDal
            .Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
            .Returns(new DocTypeModel());
        
        //  ACT
        var result = _sut.Create().AddDocType(docTypeKey.Object).Build();
        
        //  ASSERT
        result.Should().NotBeNull();
    }
    
    [Fact]
    public void AddDocType_InvalidDocTypeKey_ShouldThrowKeyNotFoundEx()
    {
        //  ARRANGE
        var docTypeKey = new Mock<IDocTypeKey>();
        _docTypeDal
            .Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
            .Returns((null as DocTypeModel)!);
        
        //  ACT 
        var action = () => {_sut.Create().AddDocType(docTypeKey.Object).Build();};
        
        //  ASSERT
        action.Should().Throw<KeyNotFoundException>();
    }
    
    [Fact]
    public void RemoveDocType_ShouldRemoveDocType()
    {
        //  ARRANGE
        var docTypeKey = new Mock<IDocTypeKey>();
        var blueprintKey = new Mock<IBlueprintKey>();
        _blueprintDal.Setup(x => x.GetData(It.IsAny<IBlueprintKey>()))
            .Returns(new BlueprintModel());
        _blueprintDocTypeDal.Setup(x => x.ListData(It.IsAny<IBlueprintKey>()))
            .Returns(new List<BlueprintDocTypeModel>());
        
        //  ACT
        var result = _sut
            .Load(blueprintKey.Object)
            .RemoveDocType(docTypeKey.Object)
            .Build();
        
        //  ASSERT
        result.ListDocType.Should().BeEmpty();
    }
    
    [Fact]
    public void AddSignee_ValidEmail_ShouldReturnBlueprintModel()
    {
        //  ARRANGE
        var blueprint = new BlueprintModel
        {
            ListDocType = new List<BlueprintDocTypeModel>
            {
                new BlueprintDocTypeModel
                {
                    DocTypeId = "A",
                    ListSignee = new List<BlueprintSigneeModel>()
                }
            }
        };
        _userOftaDal.Setup(x => x.GetData(It.IsAny<string>()))
            .Returns(new UserOftaModel());

        //  ACT
        var result = _sut
            .Attach(blueprint)
            .AddSignee(new DocTypeModel("A"), "email@abc.org", "A", SignPositionEnum.SignLeft)
            .Build();
        
        //  ASSERT
        result
            .ListDocType.First()
            .ListSignee.First()
            .Email.Should().Be("email@abc.org");
    }
    
    [Fact]
    public void RemoveSignee_ValidEmail_ShouldReturnBlueprintModel()
    {
        //  ARRANGE
        var blueprint = new BlueprintModel
        {
            ListDocType = new List<BlueprintDocTypeModel>
            {
                new BlueprintDocTypeModel
                {
                    DocTypeId = "A",
                    ListSignee = new List<BlueprintSigneeModel>
                    {
                        new BlueprintSigneeModel
                        {
                            Email = "email@abc.org"
                        }
                    }
                }
            }
        };

        //  ACT
        var result = _sut
            .Attach(blueprint)
            .RemoveSignee(new DocTypeModel("A"), "email@abc.org")
            .Build();
        
        //  ASSERT
        result
            .ListDocType.First()
            .ListSignee.Should().BeEmpty();
    }

}