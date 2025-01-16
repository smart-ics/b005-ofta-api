using FluentAssertions;
using Moq;
using Ofta.Domain.DocContext.DocTypeAgg;
using Xunit;

namespace Ofta.Application.Helpers.DocNumberGenerator.Tests;

public class GetNumberDailyResetTest
{
    private readonly Mock<IDocTypeNumberValueDal> _numberValueDal;
    private readonly DocNumberGenerator _sut;

    public GetNumberDailyResetTest()
    {
        Mock<IDocTypeNumberFormatDal> numberFormatDal = new();
        _numberValueDal = new Mock<IDocTypeNumberValueDal>();
        _sut = new DocNumberGenerator(numberFormatDal.Object, _numberValueDal.Object);
    }
    
    [Fact]
    public void GivenEmptyListNumbering_GetNumberDailyReset_ThenCreateNewNumbering_Test()
    {
        // ARRANGE
        var fakerDocTypeNumberFormat = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Day
        };
        
        var fakerCreatedDate = new DateTime(2025, 2, 1);
        
        var expectedNewDocTypeNumberValue = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 2,
            PeriodeHari = 1,
            PeriodeBulan = 2,
            PeriodeTahun = 2025
        };
        
        var actualListNumbering = new List<DocTypeNumberValueModel>();
        
        _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
            .Returns(null as List<DocTypeNumberValueModel> ?? []);

        _numberValueDal.Setup(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()))
            .Callback<IEnumerable<DocTypeNumberValueModel>>(k => actualListNumbering = k.ToList());

        // ACT
        var actual = _sut.GetNumberDailyReset(fakerDocTypeNumberFormat, fakerCreatedDate);

        // ASSERT
        actual.Should().Be(1);
        actualListNumbering.Count.Should().Be(1);
        actualListNumbering.First().Should().BeEquivalentTo(expectedNewDocTypeNumberValue);
        
        _numberValueDal.Verify(x => x.ListData(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Delete(It.IsAny<IDocTypeKey>()), Times.Once);
        _numberValueDal.Verify(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()), Times.Once);
    }
    
    [Fact]
    public void GivenDifferentDaySameMonthSameYear_GetNumberDailyReset_ThenCreateNewNumbering_Test()
    {
        // ARRANGE
        var fakerDocTypeNumberFormat = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Day
        };

        var fakerDocTypeNumberValue1 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 10,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };
        
        var fakerDocTypeNumberValue2 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 20,
            PeriodeHari = 30,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };

        var fakerDocTypeNumberValues = new List<DocTypeNumberValueModel>() { fakerDocTypeNumberValue1, fakerDocTypeNumberValue2 };
        var fakerCreatedDate = new DateTime(2025, 1, 31);
        
        var expectedNewDocTypeNumberValue = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 2,
            PeriodeHari = 31,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };
        
        var actualListNumbering = new List<DocTypeNumberValueModel>();
        
        _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
            .Returns(fakerDocTypeNumberValues);

        _numberValueDal.Setup(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()))
            .Callback<IEnumerable<DocTypeNumberValueModel>>(k => actualListNumbering = k.ToList());

        // ACT
        var actual = _sut.GetNumberDailyReset(fakerDocTypeNumberFormat, fakerCreatedDate);

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
    public void GivenDifferentDayDifferentMonthSameYear_GetNumberDailyReset_ThenCreateNewNumbering_Test()
    {
        // ARRANGE
        var fakerDocTypeNumberFormat = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Day
        };

        var fakerDocTypeNumberValue1 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 10,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };
        
        var fakerDocTypeNumberValue2 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 20,
            PeriodeHari = 31,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };

        var fakerDocTypeNumberValues = new List<DocTypeNumberValueModel>() { fakerDocTypeNumberValue1, fakerDocTypeNumberValue2 };
        var fakerCreatedDate = new DateTime(2025, 2, 1);
        
        var expectedNewDocTypeNumberValue = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 2,
            PeriodeHari = 1,
            PeriodeBulan = 2,
            PeriodeTahun = 2025
        };
        
        var actualListNumbering = new List<DocTypeNumberValueModel>();
        
        _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
            .Returns(fakerDocTypeNumberValues);

        _numberValueDal.Setup(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()))
            .Callback<IEnumerable<DocTypeNumberValueModel>>(k => actualListNumbering = k.ToList());

        // ACT
        var actual = _sut.GetNumberDailyReset(fakerDocTypeNumberFormat, fakerCreatedDate);

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
    public void GivenDifferentDayDifferentMonthDifferentYear_GetNumberDailyReset_ThenCreateNewNumbering_Test()
    {
        // ARRANGE
        var fakerDocTypeNumberFormat = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Day
        };

        var fakerDocTypeNumberValue1 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 10,
            PeriodeHari = 30,
            PeriodeBulan = 11,
            PeriodeTahun = 2024
        };
        
        var fakerDocTypeNumberValue2 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 20,
            PeriodeHari = 31,
            PeriodeBulan = 12,
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
        var actual = _sut.GetNumberDailyReset(fakerDocTypeNumberFormat, fakerCreatedDate);

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
    public void GivenExactSameDate_GetNumberDailyReset_ThenIncrementLatestNumbering_Test()
    {
        // ARRANGE
        var fakerDocTypeNumberFormat = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Day
        };

        var fakerDocTypeNumberValue1 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 10,
            PeriodeHari = 1,
            PeriodeBulan = 2,
            PeriodeTahun = 2025
        };
        
        var fakerDocTypeNumberValue2 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 20,
            PeriodeHari = 2,
            PeriodeBulan = 2,
            PeriodeTahun = 2025
        };

        var fakerDocTypeNumberValues = new List<DocTypeNumberValueModel>() { fakerDocTypeNumberValue1, fakerDocTypeNumberValue2 };
        var fakerCreatedDate = new DateTime(2025, 2, 2);
        
        var expectedDocTypeNumberValue2 = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 21,
            PeriodeHari = 2,
            PeriodeBulan = 2,
            PeriodeTahun = 2025
        };
        
        var actualListNumbering = new List<DocTypeNumberValueModel>();
        
        _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
            .Returns(fakerDocTypeNumberValues);

        _numberValueDal.Setup(x => x.Insert(It.IsAny<IEnumerable<DocTypeNumberValueModel>>()))
            .Callback<IEnumerable<DocTypeNumberValueModel>>(k => actualListNumbering = k.ToList());

        // ACT
        var actual = _sut.GetNumberDailyReset(fakerDocTypeNumberFormat, fakerCreatedDate);

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