using FluentAssertions;
using Moq;
using Ofta.Domain.DocContext.DocTypeAgg;
using Xunit;

namespace Ofta.Application.Helpers.DocNumberGenerator.Tests;

public class DocNumberGeneratorTest
{
    private readonly Mock<IDocTypeNumberFormatDal> _numberFormatDal;
    private readonly Mock<IDocTypeNumberValueDal> _numberValueDal;
    private readonly DocNumberGenerator _sut;

    public DocNumberGeneratorTest()
    {
        _numberFormatDal = new Mock<IDocTypeNumberFormatDal>();
        _numberValueDal = new Mock<IDocTypeNumberValueDal>();
        _sut = new DocNumberGenerator(_numberFormatDal.Object, _numberValueDal.Object);
    }
    
    private static List<DocTypeNumberValueModel> FakerDocTypeNumbers() =>
    [
        new()
        {
            DocTypeId = "A",
            Value = 1,
            PeriodeHari = 1,
            PeriodeBulan = 2,
            PeriodeTahun = 2025
        },
        new()
        {
            DocTypeId = "A",
            Value = 1,
            PeriodeHari = 1,
            PeriodeBulan = 3,
            PeriodeTahun = 2025
        },
    ];
    
     [Fact]
     public void GivenInvalidDocType_ThenReturnEmptyString_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/DWS/RSUA/SKD/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(null as DocTypeNumberFormatModel);

         var fakerCreatedDate = new DateTime(2025, 2, 1);

         // ACT
         var actual = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual.Should().BeEmpty();
     }
     
     [Fact]
     public void GivenFormatSKRIDewasa_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/DWS/RSUA/SKRI/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/DWS/RSUA/SKRI/2025");
         actual2.Should().BeEquivalentTo("002/III/DWS/RSUA/SKRI/2025");
         actual3.Should().BeEquivalentTo("003/III/DWS/RSUA/SKRI/2025");
     }
     
     [Fact]
     public void GivenFormatSKRIBedahIsolasi_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/BDH-ISO/RSUA/SKRI/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/BDH-ISO/RSUA/SKRI/2025");
         actual2.Should().BeEquivalentTo("002/III/BDH-ISO/RSUA/SKRI/2025");
         actual3.Should().BeEquivalentTo("003/III/BDH-ISO/RSUA/SKRI/2025");
     }
     
     [Fact]
     public void GivenFormatSKRIKebidanan_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/BDN/RSUA/SKRI/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/BDN/RSUA/SKRI/2025");
         actual2.Should().BeEquivalentTo("002/III/BDN/RSUA/SKRI/2025");
         actual3.Should().BeEquivalentTo("003/III/BDN/RSUA/SKRI/2025");
     }
     
     [Fact]
     public void GivenFormatSKRIAnak_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/ANK/RSUA/SKRI/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/ANK/RSUA/SKRI/2025");
         actual2.Should().BeEquivalentTo("002/III/ANK/RSUA/SKRI/2025");
         actual3.Should().BeEquivalentTo("003/III/ANK/RSUA/SKRI/2025");
     }
     
     [Fact]
     public void GivenFormatSKDDewasa_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/DWS/RSUA/SKD/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/DWS/RSUA/SKD/2025");
         actual2.Should().BeEquivalentTo("002/III/DWS/RSUA/SKD/2025");
         actual3.Should().BeEquivalentTo("003/III/DWS/RSUA/SKD/2025");
     }
     
     [Fact]
     public void GivenFormatSKDBedahIsolasi_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/BDH-ISO/RSUA/SKD/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/BDH-ISO/RSUA/SKD/2025");
         actual2.Should().BeEquivalentTo("002/III/BDH-ISO/RSUA/SKD/2025");
         actual3.Should().BeEquivalentTo("003/III/BDH-ISO/RSUA/SKD/2025");
     }
     
     [Fact]
     public void GivenFormatSKDKebidanan_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/BDN/RSUA/SKD/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/BDN/RSUA/SKD/2025");
         actual2.Should().BeEquivalentTo("002/III/BDN/RSUA/SKD/2025");
         actual3.Should().BeEquivalentTo("003/III/BDN/RSUA/SKD/2025");
     }
     
     [Fact]
     public void GivenFormatSKDAnak_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/ANK/RSUA/SKD/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/ANK/RSUA/SKD/2025");
         actual2.Should().BeEquivalentTo("002/III/ANK/RSUA/SKD/2025");
         actual3.Should().BeEquivalentTo("003/III/ANK/RSUA/SKD/2025");
     }
     
     [Fact]
     public void GivenFormatSKMDewasa_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/DWS/RSUA/SKM/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/DWS/RSUA/SKM/2025");
         actual2.Should().BeEquivalentTo("002/III/DWS/RSUA/SKM/2025");
         actual3.Should().BeEquivalentTo("003/III/DWS/RSUA/SKM/2025");
     }
     
     [Fact]
     public void GivenFormatSKMBedahIsolasi_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/BDH-ISO/RSUA/SKM/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/BDH-ISO/RSUA/SKM/2025");
         actual2.Should().BeEquivalentTo("002/III/BDH-ISO/RSUA/SKM/2025");
         actual3.Should().BeEquivalentTo("003/III/BDH-ISO/RSUA/SKM/2025");
     }
     
     [Fact]
     public void GivenFormatSKMKebidanan_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/BDN/RSUA/SKM/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/BDN/RSUA/SKM/2025");
         actual2.Should().BeEquivalentTo("002/III/BDN/RSUA/SKM/2025");
         actual3.Should().BeEquivalentTo("003/III/BDN/RSUA/SKM/2025");
     }
     
     [Fact]
     public void GivenFormatSKMAnak_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/ANK/RSUA/SKM/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/ANK/RSUA/SKM/2025");
         actual2.Should().BeEquivalentTo("002/III/ANK/RSUA/SKM/2025");
         actual3.Should().BeEquivalentTo("003/III/ANK/RSUA/SKM/2025");
     }
     
     [Fact]
     public void GivenFormatSKSDewasa_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/DWS/RSUA/SKS/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/DWS/RSUA/SKS/2025");
         actual2.Should().BeEquivalentTo("002/III/DWS/RSUA/SKS/2025");
         actual3.Should().BeEquivalentTo("003/III/DWS/RSUA/SKS/2025");
     }
     
     [Fact]
     public void GivenFormatSKSBedahIsolasi_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/BDH-ISO/RSUA/SKS/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/BDH-ISO/RSUA/SKS/2025");
         actual2.Should().BeEquivalentTo("002/III/BDH-ISO/RSUA/SKS/2025");
         actual3.Should().BeEquivalentTo("003/III/BDH-ISO/RSUA/SKS/2025");
     }
     
     [Fact]
     public void GivenFormatSKSKebidanan_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/BDN/RSUA/SKS/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/BDN/RSUA/SKS/2025");
         actual2.Should().BeEquivalentTo("002/III/BDN/RSUA/SKS/2025");
         actual3.Should().BeEquivalentTo("003/III/BDN/RSUA/SKS/2025");
     }
     
     [Fact]
     public void GivenFormatSKSAnak_ThenGenerateExpectedDocNumber_Test()
     {
         // ARRANGE
         var fakerNumberFormat = new DocTypeNumberFormatModel
         {
             DocTypeId = "A",
             Format = "{NOURUT,3}/{ROMAWIM}/ANK/RSUA/SKS/{Y,4}",
             ResetBy = ResetByEnum.Month
         };
         
         _numberFormatDal.Setup(x => x.GetData(It.IsAny<IDocTypeKey>()))
             .Returns(fakerNumberFormat);

         _numberValueDal.Setup(x => x.ListData(It.IsAny<IDocTypeKey>()))
             .Returns(FakerDocTypeNumbers());

         var fakerCreatedDate = new DateTime(2025, 3, 1);

         // ACT
         var actual1 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual2 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);
         var actual3 = _sut.Generate(fakerNumberFormat, fakerCreatedDate);

         // ASSERT
         actual1.Should().BeEquivalentTo("001/III/ANK/RSUA/SKS/2025");
         actual2.Should().BeEquivalentTo("002/III/ANK/RSUA/SKS/2025");
         actual3.Should().BeEquivalentTo("003/III/ANK/RSUA/SKS/2025");
     }
}