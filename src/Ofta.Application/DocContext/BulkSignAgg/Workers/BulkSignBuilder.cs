using FluentAssertions;
using Moq;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.BulkSignAgg;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Xunit;

namespace Ofta.Application.DocContext.BulkSignAgg.Workers;

public interface IBulkSignBuilder : INunaBuilder<BulkSignModel>
{
    IBulkSignBuilder Create();
    IBulkSignBuilder Attach(BulkSignModel model);
    IBulkSignBuilder Load(IBulkSignKey key);
    IBulkSignBuilder UserOfta(IUserOftaKey key);
    IBulkSignBuilder AddDocument(IDocKey docId);
}

public class BulkSignBuilder: IBulkSignBuilder
{
    private BulkSignModel _aggregate = new();
    private readonly ITglJamDal _tglJamDal;
    private readonly IBulkSignDal _bulkSignDal;
    private readonly IBulkSignDocDal _bulkSignDocDal;
    private readonly IBulkSignDocSigneeDal _bulkSignDocSigneeDal;
    private readonly IUserOftaDal _userOftaDal;
    private readonly IDocBuilder _docBuilder;

    public BulkSignBuilder(ITglJamDal tglJamDal, IBulkSignDal bulkSignDal, IBulkSignDocDal bulkSignDocDal, IBulkSignDocSigneeDal bulkSignDocSigneeDal, IUserOftaDal userOftaDal, IDocBuilder docBuilder)
    {
        _tglJamDal = tglJamDal;
        _bulkSignDal = bulkSignDal;
        _bulkSignDocDal = bulkSignDocDal;
        _bulkSignDocSigneeDal = bulkSignDocSigneeDal;
        _userOftaDal = userOftaDal;
        _docBuilder = docBuilder;
    }

    public BulkSignModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public IBulkSignBuilder Create()
    {
        _aggregate = new BulkSignModel
        {
            BulkSignDate = _tglJamDal.Now,
            DocCount = 0,
            ListDoc = new List<BulkSignDocModel>(),
        };

        return this;
    }

    public IBulkSignBuilder Attach(BulkSignModel model)
    {
        _aggregate = model;
        return this;
    }

    public IBulkSignBuilder Load(IBulkSignKey key)
    {
        _aggregate = _bulkSignDal.GetData(key)
            ?? throw new KeyNotFoundException($"Bulk sign with id {key.BulkSignId} not found");

        _aggregate.ListDoc = _bulkSignDocDal.ListData(key)?.ToList()
            ?? new List<BulkSignDocModel>();

        _aggregate.ListDoc.ForEach(x =>
        {
            x.ListSignee = _bulkSignDocSigneeDal.ListData(x)?.ToList() 
                ?? new List<BulkSignDocSigneeModel>();
        });

        return this;
    }

    public IBulkSignBuilder UserOfta(IUserOftaKey key)
    {
        var userOfta = _userOftaDal.GetData(key)
            ?? throw new KeyNotFoundException($"User Ofta with id {key.UserOftaId} not found");
        
        _aggregate.UserOftaId = userOfta.UserOftaId;
        return this;
    }

    public IBulkSignBuilder AddDocument(IDocKey docId)
    {
        var document = _docBuilder
            .Load(docId)
            .Build();

        var newBulkSignDoc = new BulkSignDocModel
        {
            BulkSignId = _aggregate.BulkSignId,
            DocId = document.DocId,
            UploadedDocId = document.UploadedDocId,
            NoUrut = 0,
            ListSignee = document.ListSignees?.Select(x 
                => new BulkSignDocSigneeModel
                {
                    DocId = x.DocId,
                    UserOftaId = x.UserOftaId,
                    Email = x.Email,
                    SignTag = x.SignTag,
                    SignPosition = x.SignPosition,
                    SignPositionDesc = x.SignPositionDesc,
                    SignUrl = x.SignUrl,
                }
            ).ToList() ?? new List<BulkSignDocSigneeModel>()
        };
        
        _aggregate.ListDoc.RemoveAll(x => x.DocId == docId.DocId);
        _aggregate.ListDoc.Add(newBulkSignDoc);

        var noUrut = 1;
        _aggregate.ListDoc.ForEach(x =>
        {
            x.NoUrut = noUrut;
            noUrut++;
        });
        
        _aggregate.DocCount = _aggregate.ListDoc.Count;

        return this;
    }
}

public class BulkSignBuilderTest
{
    private readonly Mock<ITglJamDal> _tglJamDal;
    private readonly Mock<IBulkSignDal> _bulkSignDal;
    private readonly Mock<IBulkSignDocDal> _bulkSignDocDal;
    private readonly Mock<IBulkSignDocSigneeDal> _bulkSignDocSigneeDal;
    private readonly Mock<IUserOftaDal> _userOftaDal;
    private readonly Mock<IDocBuilder> _docBuilder;
    private readonly BulkSignBuilder _sut;

    public BulkSignBuilderTest()
    {
        _tglJamDal = new Mock<ITglJamDal>();
        _bulkSignDal = new Mock<IBulkSignDal>();
        _bulkSignDocDal = new Mock<IBulkSignDocDal>();
        _bulkSignDocSigneeDal = new Mock<IBulkSignDocSigneeDal>();
        _userOftaDal = new Mock<IUserOftaDal>();
        _docBuilder = new Mock<IDocBuilder>();
        _sut = new BulkSignBuilder(_tglJamDal.Object, _bulkSignDal.Object, _bulkSignDocDal.Object, _bulkSignDocSigneeDal.Object, _userOftaDal.Object, _docBuilder.Object);
    }

    [Fact]
    public void GivenValidRequest_ThenCreateBulkSignObject()
    {
        // ARRANGE
        var fakerDateTime = DateTime.Now;
        var expectedBulkSign = new BulkSignModel
        {
            BulkSignId = string.Empty,
            BulkSignDate = fakerDateTime,
            UserOftaId = string.Empty,
            DocCount = 0,
            ListDoc = new List<BulkSignDocModel>(),
        };

        _tglJamDal.Setup(x => x.Now).Returns(fakerDateTime);
        
        // ACT
        var actual = _sut.Create().Build();

        // ASSERT
        actual.Should().BeEquivalentTo(expectedBulkSign);
    }
    
    [Fact]
    public void GivenValidRequest_ThenLoadBulkSignObject()
    {
        // ARRANGE
        var fakerSignee = new BulkSignDocSigneeModel
        {
            DocId = "DOC1",
            UserOftaId = "USER-001",
            Email = "user@email.com",
            SignTag = "Mengetahun",
            SignPosition = SignPositionEnum.SignLeft,
            SignPositionDesc = "Desc"
        };
        
        var fakerDocument = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC1",
            UploadedDocId = "DOC1",
            NoUrut = 1,
            ListSignee = new List<BulkSignDocSigneeModel>(),
        };
        
        var fakerListSignee = new List<BulkSignDocSigneeModel>{fakerSignee};
        var fakerBulkSignListDoc = new List<BulkSignDocModel>{fakerDocument};
        var fakerBulkSign = new BulkSignModel
        {
            BulkSignId = "A",
            BulkSignDate = DateTime.Now,
            UserOftaId = "B",
            DocCount = 1,
        };
        
        _bulkSignDal.Setup(x => x.GetData(It.IsAny<IBulkSignKey>()))
            .Returns(fakerBulkSign);
        _bulkSignDocDal.Setup(x => x.ListData(It.IsAny<IBulkSignKey>()))
            .Returns(fakerBulkSignListDoc);
        _bulkSignDocSigneeDal.Setup(x => x.ListData(It.IsAny<IDocKey>()))
            .Returns(fakerListSignee);

        var expectedBulkSignDoc = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC1",
            UploadedDocId = "DOC1",
            NoUrut = 1,
            ListSignee = fakerListSignee,
        };
        
        var expectedBulkSignListDoc = new List<BulkSignDocModel>{expectedBulkSignDoc};
        var expectedBulkSign = new BulkSignModel
        {
            BulkSignId = "A",
            BulkSignDate = fakerBulkSign.BulkSignDate,
            UserOftaId = "B",
            DocCount = 1,
            ListDoc = expectedBulkSignListDoc,
        };

        // ACT
        var actual = _sut
            .Load(expectedBulkSign)
            .Build();

        // ASSERT
        actual.Should().BeEquivalentTo(expectedBulkSign);
        actual.ListDoc.Should().BeEquivalentTo(expectedBulkSignListDoc);
        actual.ListDoc.First().ListSignee.Should().BeEquivalentTo(fakerListSignee);
    }

    [Fact]
    public void GivenNewDocument_ThenAddToListDoc_AndAdjustNoUrut()
    {
        // ARRANGE
        var fakerNewDocument = new DocModel
        {
            DocId = "DOC3",
            UploadedDocId = "DOC3",
        };
        var fakerDocument1 = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC1",
            UploadedDocId = "DOC1",
            NoUrut = 1,
            ListSignee = new List<BulkSignDocSigneeModel>(),
        };
        var fakerDocument2 = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC2",
            UploadedDocId = "DOC2",
            NoUrut = 2,
            ListSignee = new List<BulkSignDocSigneeModel>(),
        };
        
        var fakerBulkSignListDoc = new List<BulkSignDocModel>{fakerDocument1, fakerDocument2};
        var fakerBulkSign = new BulkSignModel
        {
            BulkSignId = "A",
            BulkSignDate = DateTime.Now,
            UserOftaId = "B",
            DocCount = 2,
        };
        
        _bulkSignDal.Setup(x => x.GetData(It.IsAny<IBulkSignKey>()))
            .Returns(fakerBulkSign);
        _bulkSignDocDal.Setup(x => x.ListData(It.IsAny<IBulkSignKey>()))
            .Returns(fakerBulkSignListDoc);
        _docBuilder.Setup(x => x.Load(It.IsAny<IDocKey>()).Build())
            .Returns(fakerNewDocument);
        
        var expectedNewDocument = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC3",
            UploadedDocId = "DOC3",
            NoUrut = 3,
            ListSignee = new List<BulkSignDocSigneeModel>(),
        };
        
        var expectedBulkSignListDoc = new List<BulkSignDocModel>{fakerDocument1, fakerDocument2, expectedNewDocument};
        var expectedBulkSign = new BulkSignModel
        {
            BulkSignId = "A",
            BulkSignDate = fakerBulkSign.BulkSignDate,
            UserOftaId = "B",
            DocCount = 3,
            ListDoc = expectedBulkSignListDoc,
        };

        // ACT
        var actual = _sut
            .Load(expectedBulkSign)
            .AddDocument(fakerNewDocument)
            .Build();

        // ASSERT
        actual.Should().BeEquivalentTo(expectedBulkSign);
        actual.ListDoc.Should().BeEquivalentTo(expectedBulkSignListDoc);
    }
    
    [Fact]
    public void GivenExistingDocument_ThenOverwriteListDoc_AndAdjustNoUrut()
    {
        // ARRANGE
        var fakerNewDocument = new DocModel
        {
            DocId = "DOC2",
            UploadedDocId = "DOC2",
        };
        var fakerDocument1 = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC1",
            UploadedDocId = "DOC1",
            NoUrut = 1,
            ListSignee = new List<BulkSignDocSigneeModel>(),
        };
        var fakerDocument2 = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC2",
            UploadedDocId = "DOC2",
            NoUrut = 2,
            ListSignee = new List<BulkSignDocSigneeModel>(),
        };
        var fakerDocument3 = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC3",
            UploadedDocId = "DOC3",
            NoUrut = 3,
            ListSignee = new List<BulkSignDocSigneeModel>(),
        };
        
        var fakerBulkSignListDoc = new List<BulkSignDocModel>{fakerDocument1, fakerDocument2, fakerDocument3};
        var fakerBulkSign = new BulkSignModel
        {
            BulkSignId = "A",
            BulkSignDate = DateTime.Now,
            UserOftaId = "B",
            DocCount = 3,
        };
        
        _bulkSignDal.Setup(x => x.GetData(It.IsAny<IBulkSignKey>()))
            .Returns(fakerBulkSign);
        _bulkSignDocDal.Setup(x => x.ListData(It.IsAny<IBulkSignKey>()))
            .Returns(fakerBulkSignListDoc);
        _docBuilder.Setup(x => x.Load(It.IsAny<IDocKey>()).Build())
            .Returns(fakerNewDocument);

        fakerDocument3.NoUrut = 2;
        fakerDocument2.NoUrut = 3;
        var expectedBulkSignListDoc = new List<BulkSignDocModel>{fakerDocument1, fakerDocument3, fakerDocument2};
        var expectedBulkSign = new BulkSignModel
        {
            BulkSignId = "A",
            BulkSignDate = fakerBulkSign.BulkSignDate,
            UserOftaId = "B",
            DocCount = 3,
            ListDoc = expectedBulkSignListDoc,
        };

        // ACT
        var actual = _sut
            .Load(fakerBulkSign)
            .AddDocument(fakerNewDocument)
            .Build();

        // ASSERT
        actual.Should().BeEquivalentTo(expectedBulkSign);
        actual.ListDoc.Should().BeEquivalentTo(expectedBulkSignListDoc);
    }
}