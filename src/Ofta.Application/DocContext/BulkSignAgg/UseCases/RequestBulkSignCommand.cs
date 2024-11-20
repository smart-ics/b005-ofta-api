using Dawn;
using FluentAssertions;
using MediatR;
using Moq;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Application.DocContext.BulkSignAgg.Workers;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.DocContext.BulkSignAgg;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.TilakaAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Xunit;

namespace Ofta.Application.DocContext.BulkSignAgg.UseCases;

public record RequestBulkSignSuccessEvent(
    BulkSignModel Agg,
    RequestBulkSignCommand Command
) : INotification;

public record RequestBulkSignCommand(string UserOftaId, List<string> ListDocId): IRequest<RequestBulkSignResponse>, IUserOftaKey;

public record SigneeResponse(string UserOftaId, string Email, string TilakaName, string SignUrl);
public record RequestBulkSignResponse(string BulkSignId, SigneeResponse Signee);

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
            originalDoc.SignUrl = updatedSignUrl.SignUrl;
    }
    
    private RequestBulkSignResponse BuildResponse(BulkSignModel bulkSign)
    {
        var signee = bulkSign.ListDoc.First();
        var tilakaUser = _tilakaUserBuilder
            .Load(bulkSign.Email)
            .Build();
        
        var tilakaName = tilakaUser is not null ? tilakaUser.TilakaName : string.Empty;
        var signeeResponse = new SigneeResponse(bulkSign.UserOftaId, tilakaUser?.Email ?? string.Empty, tilakaName, signee.SignUrl);
        
        return new RequestBulkSignResponse(
            bulkSign.BulkSignId,
            signeeResponse
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
            BulkSignState = BulkSignStateEnum.Requested,
            ListDoc = new List<BulkSignDocModel>()
        };

        _builder.Setup(x => x.Create().UserOfta(It.IsAny<IUserOftaKey>()).UpdateState(It.IsAny<BulkSignStateEnum>()).Build())
            .Returns(fakerBulkSign);
        
        var fakerDocument1 = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC1",
            UploadedDocId = "DOC1",
            NoUrut = 1,
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
            SignTag = "Mengetahui",
            SignPosition = SignPositionEnum.SignLeft,
            SignPositionDesc = "",
            SignUrl = ""
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
            Email = "B",
            DocCount = 0,
            BulkSignState = BulkSignStateEnum.Requested,
            ListDoc = new List<BulkSignDocModel>()
        };

        _builder.Setup(x => x.Create().UserOfta(It.IsAny<IUserOftaKey>()).UpdateState(It.IsAny<BulkSignStateEnum>()).Build())
            .Returns(fakerBulkSign);
        
        var fakerDocument1 = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC1",
            UploadedDocId = "DOC1",
            NoUrut = 1,
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
            SignTag = "Mengetahui",
            SignPosition = SignPositionEnum.SignLeft,
            SignPositionDesc = "",
            SignUrl = ""
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
            Email = "B",
            DocCount = 2,
            ListDoc = new List<BulkSignDocModel>{fakerDocument1, fakerDocument2}
        };
        
        BulkSignModel actualBulkSign = null;
        fakerBulkSign.BulkSignId = "BULK-001";
        _writer.Setup(x => x.Save(It.IsAny<BulkSignModel>()))
            .Callback<BulkSignModel>(bulkSign => actualBulkSign = bulkSign)
            .Returns(fakerBulkSign);

        fakerBulkSign.ListDoc[0].SignUrl = "signUrl1";
        fakerBulkSign.ListDoc[1].SignUrl = "signUrl2";
        _service.Setup(x => x.Execute(It.IsAny<ReqBulkSignRequest>()))
            .Returns(new ReqBulkSignResponse(true, "A", fakerBulkSign));
        
        _writer.Setup(x => x.Save(It.IsAny<BulkSignModel>()))
            .Callback<BulkSignModel>(bulkSign => actualBulkSign = bulkSign)
            .Returns(fakerBulkSign);

        var fakerTilakaUser = new TilakaUserModel
        {
            UserOftaId = "A",
            Email = "B",
            TilakaName = "C",
        };
        _tilakaUserBuilder.Setup(x => x.Load(It.IsAny<string>()).Build())
            .Returns(fakerTilakaUser);
        
        var expectedResponse = new RequestBulkSignResponse("BULK-001", new SigneeResponse("A", "B", "C", "signUrl1"));
        
        // ACT
        var actual = await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        _mediator.Verify(x => x.Publish(It.IsAny<RequestBulkSignSuccessEvent>(), It.IsAny<CancellationToken>()), Times.Once());
        actual.Should().BeEquivalentTo(expectedResponse);
        actualBulkSign.Should().BeEquivalentTo(expectedBulkSign);
    }
}