using FluentAssertions;
using Moq;
using Ofta.Domain.DocContext.DocTypeAgg;
using Xunit;

namespace Ofta.Application.Helpers.DocNumberGenerator.Tests;

public class GetNumberYearlyResetTest
{
    private readonly Mock<IDocTypeNumberValueDal> _numberValueDal;
    private readonly DocNumberGenerator _sut;

    public GetNumberYearlyResetTest()
    {
        Mock<IDocTypeNumberFormatDal> numberFormatDal = new();
        _numberValueDal = new Mock<IDocTypeNumberValueDal>();
        _sut = new DocNumberGenerator(numberFormatDal.Object, _numberValueDal.Object);
    }
    
    [Fact]
    public void GivenEmptyListNumbering_GetNumberYearlyReset_ThenCreateNewNumbering_Test()
    {
        // ARRANGE
        var fakerDocTypeNumberFormat = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Year
        };
        
        var fakerCreatedDate = new DateTime(2025, 1, 1);
        
        var expectedNewDocTypeNumberValue = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 2,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };
        
        var actualListNumbering = new List<DocTypeNumberValueModel>();
        
        _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
            .Returns(null as List<DocTypeNumberValueModel> ?? []);

        _numberValueDal.Setup(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()))
            .Callback<IEnumerable<DocTypeNumberValueModel>>(k => actualListNumbering = k.ToList());

        // ACT
        var actual = _sut.GetNumberYearlyReset(fakerDocTypeNumberFormat, fakerCreatedDate);

        // ASSERT
        actual.Should().Be(1);
        actualListNumbering.Count.Should().Be(1);
        actualListNumbering.First().Should().BeEquivalentTo(expectedNewDocTypeNumberValue);
        
        _numberValueDal.Verify(x => x.ListData(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Delete(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()), Times.Once);
    }
    
    [Fact]
    public void GivenDifferentYear_GetNumberYearlyReset_ThenCreateNewNumbering_Test()
    {
        // ARRANGE
        var fakerDocTypeNumberFormat = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Year
        };

        var fakerDocTypeNumberValue1 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 10,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2023
        };
        
        var fakerDocTypeNumberValue2 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 20,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2024
        };

        var fakerDocTypeNumberValues = new List<DocTypeNumberValueModel>() { fakerDocTypeNumberValue1, fakerDocTypeNumberValue2 };
        var fakerCreatedDate = new DateTime(2025, 1, 1);
        
        var expectedNewDocTypeNumberValue = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 2,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };
        
        var actualListNumbering = new List<DocTypeNumberValueModel>();
        
        _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
            .Returns(fakerDocTypeNumberValues);

        _numberValueDal.Setup(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()))
            .Callback<IEnumerable<DocTypeNumberValueModel>>(k => actualListNumbering = k.ToList());

        // ACT
        var actual = _sut.GetNumberYearlyReset(fakerDocTypeNumberFormat, fakerCreatedDate);

        // ASSERT
        actual.Should().Be(1);
        actualListNumbering.Count.Should().Be(3);
        actualListNumbering[0].Should().BeEquivalentTo(fakerDocTypeNumberValue1);
        actualListNumbering[1].Should().BeEquivalentTo(fakerDocTypeNumberValue2);
        actualListNumbering[2].Should().BeEquivalentTo(expectedNewDocTypeNumberValue);
        
        _numberValueDal.Verify(x => x.ListData(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Delete(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()), Times.Once);
    }
    
    [Fact]
    public void GivenSameYear_GetNumberYearlyReset_ThenIncrementLatestNumbering_Test()
    {
        // ARRANGE
        var fakerDocTypeNumberFormat = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Year
        };

        var fakerDocTypeNumberValue1 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 10,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2024
        };
        
        var fakerDocTypeNumberValue2 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 20,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };

        var fakerDocTypeNumberValues = new List<DocTypeNumberValueModel>() { fakerDocTypeNumberValue1, fakerDocTypeNumberValue2 };
        var fakerCreatedDate = new DateTime(2025, 1, 1);
        
        var expectedDocTypeNumberValue2 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 21,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };
        
        var actualListNumbering = new List<DocTypeNumberValueModel>();
        
        _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
            .Returns(fakerDocTypeNumberValues);

        _numberValueDal.Setup(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()))
            .Callback<IEnumerable<DocTypeNumberValueModel>>(k => actualListNumbering = k.ToList());

        // ACT
        var actual = _sut.GetNumberYearlyReset(fakerDocTypeNumberFormat, fakerCreatedDate);

        // ASSERT
        actual.Should().Be(20);
        actualListNumbering.Count.Should().Be(2);
        actualListNumbering[0].Should().BeEquivalentTo(fakerDocTypeNumberValue1);
        actualListNumbering[1].Should().BeEquivalentTo(expectedDocTypeNumberValue2);
        
        _numberValueDal.Verify(x => x.ListData(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Delete(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()), Times.Once);
    }
}