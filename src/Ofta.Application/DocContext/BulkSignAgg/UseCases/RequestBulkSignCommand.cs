using Dawn;
using FluentAssertions;
using MediatR;
using Moq;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Application.DocContext.BulkSignAgg.Workers;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.DocContext.BulkSignAgg;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Xunit;

namespace Ofta.Application.DocContext.BulkSignAgg.UseCases;

public record RequestBulkSignSuccessEvent(
    BulkSignModel Agg,
    RequestBulkSignCommand Command
) : INotification;

public record RequestBulkSignCommand(string UserOftaId, List<string> ListDocId): IRequest<RequestBulkSignResponse>, IUserOftaKey;

public record SigneeResponse(string UserOftaId, string Email, string TilakaName, string SignUrl);
public record RequestBulkSignResponse(string BulkSignId, IEnumerable<SigneeResponse> Signees);

public class RequestBulkSignHandler: IRequestHandler<RequestBulkSignCommand, RequestBulkSignResponse>
{
    private BulkSignModel _aggregate = new();
    private readonly ITilakaUserBuilder _tilakaUserBuilder;
    private readonly IBulkSignBuilder _builder;
    private readonly IBulkSignWriter _writer;
    private readonly IRequestBulkSignService _service;
    private readonly IMediator _mediator;

    public RequestBulkSignHandler(ITilakaUserBuilder tilakaUserBuilder, IBulkSignBuilder builder, IBulkSignWriter writer, IRequestBulkSignService service, IMediator mediator)
    {
        _tilakaUserBuilder = tilakaUserBuilder;
        _builder = builder;
        _writer = writer;
        _service = service;
        _mediator = mediator;
    }

    public Task<RequestBulkSignResponse> Handle(RequestBulkSignCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.UserOftaId, y => y.NotEmpty())
            .Member(x => x.ListDocId, y => y.NotEmpty());
        
        // BUILD
        _aggregate = _builder
            .Create()
            .UserOfta(request)
            .UpdateState(BulkSignStateEnum.Requested)
            .Build();
        
        request.ListDocId.ForEach(docId 
            => _aggregate = _builder
                .Attach(_aggregate)
                .AddDocument(new DocModel(docId))
                .Build());
        
        // WRITE
        _aggregate = _writer.Save(_aggregate);
        var requestSign = _service.Execute(new ReqBulkSignRequest(_aggregate));
        if (!requestSign.Success)
            throw new ArgumentException(requestSign.Message);
        
        requestSign.BulkSign.ListDoc.ForEach(UpdateSignUrl);
        _ = _writer.Save(_aggregate);
        _mediator.Publish(new RequestBulkSignSuccessEvent(_aggregate, request), CancellationToken.None);
        
        var response = BuildResponse(_aggregate);
        return Task.FromResult(response);
    }

    private void UpdateSignUrl(BulkSignDocModel updatedSignUrl)
    {
        var originalDoc = _aggregate.ListDoc.FirstOrDefault(x => x.DocId == updatedSignUrl.DocId);
        if (originalDoc is not null)
        {
            updatedSignUrl.ListSignee.ForEach(updatedSignee =>
            {
                var originalSignee = originalDoc.ListSignee
                    .FirstOrDefault(x => x.UserOftaId == updatedSignee.UserOftaId || x.Email == updatedSignee.Email);

                if (originalSignee is not null)
                    originalSignee.SignUrl = updatedSignee.SignUrl;
            });
        }
    }
    
    private RequestBulkSignResponse BuildResponse(BulkSignModel bulkSign)
    {
        var listAllSignee = new List<SigneeResponse>();
        bulkSign.ListDoc.ForEach(doc =>
        {
            var listSigneeEachDoc = doc.ListSignee
                .Select(x =>
                {
                    var tilakaUser = _tilakaUserBuilder
                        .Load(x.Email)
                        .Build();

                    var tilakaName = tilakaUser is not null ? tilakaUser.TilakaName : string.Empty;
                    var signee = new SigneeResponse(x.UserOftaId, x.Email, tilakaName, x.SignUrl);
                    
                    return signee;
                }).ToList();
            
            listAllSignee.AddRange(listSigneeEachDoc);
        });

        var signatures = listAllSignee
            .GroupBy(s => s.Email)
            .Select(signee => new SigneeResponse(
                signee.First().UserOftaId,
                signee.First().Email,
                signee.First().TilakaName,
                signee.First().SignUrl
            )).ToList();

        return new RequestBulkSignResponse(
            bulkSign.BulkSignId,
            signatures
        );
    }
}

public class RequestBulkSignCommandHandlerTest
{
    private readonly Mock<ITilakaUserBuilder> _tilakaUserBuilder;
    private readonly Mock<IBulkSignBuilder> _builder;
    private readonly Mock<IBulkSignWriter> _writer;
    private readonly Mock<IRequestBulkSignService> _service;
    private readonly Mock<IMediator> _mediator;
    private readonly RequestBulkSignHandler _sut;

    public RequestBulkSignCommandHandlerTest()
    {
        _tilakaUserBuilder = new Mock<ITilakaUserBuilder>();
        _builder = new Mock<IBulkSignBuilder>();
        _writer = new Mock<IBulkSignWriter>();
        _service = new Mock<IRequestBulkSignService>();
        _mediator = new Mock<IMediator>();
        _sut = new RequestBulkSignHandler(_tilakaUserBuilder.Object, _builder.Object, _writer.Object, _service.Object, _mediator.Object);
    }

    [Fact]
    public async Task GivenNullRequest_ThenThrowArgumentException()
    {
        // ARRANGE
        RequestBulkSignCommand request = null;

        // ACT
        var actual = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await actual.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GivenEmptyUserOftaId_ThenThrowArgumentException()
    {
        // ARRANGE
        var request = new RequestBulkSignCommand("", new List<string>());

        // ACT
        var actual = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await actual.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GivenEmptyListDocId_ThenThrowArgumentException()
    {
        // ARRANGE
        var request = new RequestBulkSignCommand("A", new List<string>());

        // ACT
        var actual = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await actual.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GivenValidRequestAndRequestSignError_ThenSaveBulkSign_AndThrowArgumentException()
    {
        // ARRANGE
        var request = new RequestBulkSignCommand("A", new List<string>{"DOC1", "DOC2", "DOC3"});
        var fakerDateTime = DateTime.Now;
        var fakerBulkSign = new BulkSignModel
        {
            BulkSignDate = fakerDateTime,
            UserOftaId = "A",
            DocCount = 0,
            ListDoc = new List<BulkSignDocModel>()
        };

        _builder.Setup(x => x.Create().Build())
            .Returns(fakerBulkSign);
        
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

        fakerBulkSign.DocCount = 1;
        fakerBulkSign.ListDoc = new List<BulkSignDocModel> { fakerDocument1 };
        _builder.Setup(x => x.Attach(It.IsAny<BulkSignModel>()).AddDocument(It.IsAny<IDocKey>()).Build())
            .Returns(fakerBulkSign);
        
        fakerBulkSign.DocCount = 2;
        fakerBulkSign.ListDoc = new List<BulkSignDocModel> { fakerDocument1, fakerDocument2 };
        _builder.Setup(x => x.Attach(It.IsAny<BulkSignModel>()).AddDocument(It.IsAny<IDocKey>()).Build())
            .Returns(fakerBulkSign);

        var expectedBulkSign = new BulkSignModel
        {
            BulkSignId = "BULK-001",
            BulkSignDate = fakerDateTime,
            UserOftaId = "A",
            DocCount = 2,
            ListDoc = new List<BulkSignDocModel>{fakerDocument1, fakerDocument2}
        };
        
        BulkSignModel actualBulkSign = null;
        fakerBulkSign.BulkSignId = "BULK-001";
        _writer.Setup(x => x.Save(It.IsAny<BulkSignModel>()))
            .Callback((BulkSignModel k) => actualBulkSign = k)
            .Returns(fakerBulkSign);

        _service.Setup(x => x.Execute(It.IsAny<ReqBulkSignRequest>()))
            .Returns(new ReqBulkSignResponse(false, "A", expectedBulkSign));
        
        // ACT
        var actual  = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await actual.Should().ThrowAsync<ArgumentException>();
        actualBulkSign.Should().BeEquivalentTo(expectedBulkSign);
    }
    
    [Fact]
    public async Task GivenValidRequestAndRequestSignSuccess_ThenUpdateSignUrl_AndReturnExpected()
    {
        // ARRANGE
        var request = new RequestBulkSignCommand("A", new List<string>{"DOC1", "DOC2"});
        var fakerDateTime = DateTime.Now;
        var fakerBulkSign = new BulkSignModel
        {
            BulkSignDate = fakerDateTime,
            UserOftaId = "A",
            DocCount = 0,
            ListDoc = new List<BulkSignDocModel>()
        };

        _builder.Setup(x => x.Create().Build())
            .Returns(fakerBulkSign);

        var fakerSignee1 = new BulkSignDocSigneeModel
        {
            DocId = "DOC1",
            UserOftaId = "USER-001",
            Email = "signee1@email.com",
            SignTag = "Mengetahui",
            SignPosition = SignPositionEnum.SignLeft,
            SignPositionDesc = "",
            SignUrl = ""
        };
        var fakerDocument1 = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC1",
            UploadedDocId = "DOC1",
            NoUrut = 1,
            ListSignee = new List<BulkSignDocSigneeModel>{ fakerSignee1 },
        };
        
        var fakerSignee2 = new BulkSignDocSigneeModel
        {
            DocId = "DOC2",
            UserOftaId = "USER-002",
            Email = "signee2@email.com",
            SignTag = "Mengetahui",
            SignPosition = SignPositionEnum.SignLeft,
            SignPositionDesc = "",
            SignUrl = ""
        };
        var fakerDocument2 = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC2",
            UploadedDocId = "DOC2",
            NoUrut = 2,
            ListSignee = new List<BulkSignDocSigneeModel>{ fakerSignee2 },
        };

        fakerBulkSign.DocCount = 1;
        fakerBulkSign.ListDoc = new List<BulkSignDocModel> { fakerDocument1 };
        _builder.Setup(x => x.Attach(It.IsAny<BulkSignModel>()).AddDocument(It.IsAny<IDocKey>()).Build())
            .Returns(fakerBulkSign);
        
        fakerBulkSign.DocCount = 2;
        fakerBulkSign.ListDoc = new List<BulkSignDocModel> { fakerDocument1, fakerDocument2 };
        _builder.Setup(x => x.Attach(It.IsAny<BulkSignModel>()).AddDocument(It.IsAny<IDocKey>()).Build())
            .Returns(fakerBulkSign);

        var expectedBulkSign = new BulkSignModel
        {
            BulkSignId = "BULK-001",
            BulkSignDate = fakerDateTime,
            UserOftaId = "A",
            DocCount = 2,
            ListDoc = new List<BulkSignDocModel>{fakerDocument1, fakerDocument2}
        };
        
        BulkSignModel actual = null;
        fakerBulkSign.BulkSignId = "BULK-001";
        _writer.Setup(x => x.Save(It.IsAny<BulkSignModel>()))
            .Callback<BulkSignModel>(bulkSign => actual = bulkSign)
            .Returns(fakerBulkSign);

        fakerBulkSign.ListDoc[0].ListSignee.First().SignUrl = "signUrl1";
        fakerBulkSign.ListDoc[1].ListSignee.First().SignUrl = "signUrl2";
        _service.Setup(x => x.Execute(It.IsAny<ReqBulkSignRequest>()))
            .Returns(new ReqBulkSignResponse(true, "A", fakerBulkSign));
        
        _writer.Setup(x => x.Save(It.IsAny<BulkSignModel>()))
            .Callback<BulkSignModel>(bulkSign => actual = bulkSign)
            .Returns(fakerBulkSign);
        
        // ACT
        await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        _mediator.Verify(x => x.Publish(It.IsAny<RequestBulkSignSuccessEvent>(), It.IsAny<CancellationToken>()), Times.Once());
        actual.Should().BeEquivalentTo(expectedBulkSign);
    }
}