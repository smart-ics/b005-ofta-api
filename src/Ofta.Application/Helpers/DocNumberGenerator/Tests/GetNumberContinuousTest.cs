using FluentAssertions;
using Moq;
using Ofta.Domain.DocContext.DocTypeAgg;
using Xunit;

namespace Ofta.Application.Helpers.DocNumberGenerator.Tests;

public class GetNumberContinuousTest
{
    private readonly Mock<IDocTypeNumberValueDal> _numberValueDal;
    private readonly DocNumberGenerator _sut;

    public GetNumberContinuousTest()
    {
        Mock<IDocTypeNumberFormatDal> numberFormatDal = new();
        _numberValueDal = new Mock<IDocTypeNumberValueDal>();
        _sut = new DocNumberGenerator(numberFormatDal.Object, _numberValueDal.Object);
    }
    
    [Fact]
    public void GivenEmptyListNumbering_GetNumberContinuous_ThenCreateNewNumbering_Test()
    {
        // ARRANGE
        var fakerDocTypeNumberFormat = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Continuous
        };
        
        var expectedNewDocTypeNumberValue = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 2,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 3000
        };
        
        var actualListNumbering = new List<DocTypeNumberValueModel>();
        
        _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
            .Returns(null as List<DocTypeNumberValueModel> ?? []);

        _numberValueDal.Setup(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()))
            .Callback<IEnumerable<DocTypeNumberValueModel>>(k => actualListNumbering = k.ToList());

        // ACT
        var actual = _sut.GetNumberContinuous(fakerDocTypeNumberFormat);

        // ASSERT
        actual.Should().Be(1);
        actualListNumbering.Count.Should().Be(1);
        actualListNumbering.First().Should().BeEquivalentTo(expectedNewDocTypeNumberValue);
        
        _numberValueDal.Verify(x => x.ListData(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Delete(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()), Times.Once);
    }
    
    [Fact]
    public void GivenListNumberingNotEmpty_GetNumberContinuous_ThenIncrementLatestNumbering_Test()
    {
        // ARRANGE
        var fakerDocTypeNumberFormat = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Continuous
        };

        var fakerDocTypeNumberValue1 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 10,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 3000
        };

        var fakerDocTypeNumberValues = new List<DocTypeNumberValueModel>() { fakerDocTypeNumberValue1 };
        var expectedNewDocTypeNumberValue = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 11,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 3000
        };
        
        var actualListNumbering = new List<DocTypeNumberValueModel>();
        
        _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
            .Returns(fakerDocTypeNumberValues);

        _numberValueDal.Setup(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()))
            .Callback<IEnumerable<DocTypeNumberValueModel>>(k => actualListNumbering = k.ToList());

        // ACT
        var actual = _sut.GetNumberContinuous(fakerDocTypeNumberFormat);

        // ASSERT
        actual.Should().Be(10);
        actualListNumbering.Count.Should().Be(1);
        actualListNumbering.First().Should().BeEquivalentTo(expectedNewDocTypeNumberValue);
        
        _numberValueDal.Verify(x => x.ListData(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Delete(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()), Times.Once);
    }
}