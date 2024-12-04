using Dawn;
using FluentAssertions;
using MediatR;
using Moq;
using Nuna.Lib.DataTypeExtension;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Xunit;

namespace Ofta.Application.DocContext.DocAgg.UseCases.Event;

public class  UploadDocOnRepeatSignEventHandler
    : INotificationHandler<RepeatSignDocEvent>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly ISendToSignProviderService _uploadDocToProviderService;
    private readonly IReqSignToSignProviderService _getSignUrlService;

    public UploadDocOnRepeatSignEventHandler(IDocBuilder builder,
                                             IDocWriter writer,
                                             ISendToSignProviderService sendToSignProviderService,
                                             IReqSignToSignProviderService reqSignToSignProvider)
    {
        _builder = builder;
        _writer = writer;
        _uploadDocToProviderService = sendToSignProviderService;
        _getSignUrlService = reqSignToSignProvider;
    }

    public Task Handle(RepeatSignDocEvent notification, CancellationToken cancellationToken)
    {

        var agg = notification.Agg;
        var cmd = notification.Command;

        //  GUARD
        Guard.Argument(() => notification.Agg).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty());

        //  BUILD
        var uploadedDocId = agg.UploadedDocId;

        if (uploadedDocId.IsNullOrEmpty())
        {
            var uploadDocReq = new SendToSignProviderRequest(agg);
            var uploadDocResponse = _uploadDocToProviderService.Execute(uploadDocReq);
            uploadedDocId = uploadDocResponse.UploadedDocId ?? string.Empty;
        }

        var signee = agg.ListSignees.FirstOrDefault(x => x.UserOftaId == notification.Command.UserOftaId) ?? new DocSigneeModel();
        var getSignUrlReq = new ReqSignToSignProviderRequest(agg, signee, uploadedDocId);
        var getSignUrlResponse = _getSignUrlService.Execute(getSignUrlReq);

        if (getSignUrlResponse?.Signees != null)
        {
            foreach (var updatedSignee in getSignUrlResponse.Signees)
            {
                var originalSignee = agg.ListSignees
                    .FirstOrDefault(s => s.UserOftaId == updatedSignee.UserOftaId || s.Email == updatedSignee.Email);

                if (originalSignee != null)
                {
                    originalSignee.SignUrl = updatedSignee.SignUrl;
                }
            }
        }

        agg = _builder
            .Attach(agg)
            .AddJurnal(DocStateEnum.Uploaded, string.Empty)
            .UploadedDocId(uploadedDocId)
            .Build();


        //  WRITE
        _writer.Save(agg);
        return Task.CompletedTask;
    }
}

public class UploadDocOnRepeatSignEventHandlerTest
{
    private readonly UploadDocOnRepeatSignEventHandler _sut;
    private readonly Mock<IDocBuilder> _docBuilder;
    private readonly Mock<IDocWriter> _docWriter;
    private readonly Mock<ISendToSignProviderService> _uploadDocService;
    private readonly Mock<IReqSignToSignProviderService> _getSignUrlService;

    public UploadDocOnRepeatSignEventHandlerTest()
    {
        _docBuilder = new Mock<IDocBuilder>();
        _docWriter = new Mock<IDocWriter>();
        _uploadDocService = new Mock<ISendToSignProviderService>();
        _getSignUrlService = new Mock<IReqSignToSignProviderService>();
        _sut = new UploadDocOnRepeatSignEventHandler(
            _docBuilder.Object,
            _docWriter.Object,
            _uploadDocService.Object,
            _getSignUrlService.Object);

        _docBuilder.Setup(x => x.AddJurnal(It.IsAny<DocStateEnum>(), It.IsAny<string>())).Returns(_docBuilder.Object);
        _docBuilder.Setup(x => x.UploadedDocId(It.IsAny<string>())).Returns(_docBuilder.Object);
        
    }
    
    private static DocModel DocFaker()
        => new DocModel
        {
            DocId = "Doc-A",
            UploadedDocId = "Doc-A1",
            ListSignees = new List<DocSigneeModel>
            {
                new DocSigneeModel
                {
                    UserOftaId = "User-B",
                    Email = "Email-B",
                    SignUrl = "SignUrlLama"
                }
            }
        };

    private static ReqSignToSignProviderResponse GetSignUrlResponseFaker()
        => new ReqSignToSignProviderResponse
        {
            Signees = new List<DocSigneeModel>
            {
                new DocSigneeModel
                {
                    UserOftaId = "User-B",
                    Email = "Email-B",
                    SignUrl = "SignUrl-B"
                }
            }
        };

    [Fact]
    public void GivenDocSudahDiupload_AndSignUrlSudahTerbentuk_AndMenggunakanUserOftaIdSebagaiAcuanSign_ThenDocSignUrlBerubah()
    {
        //  ARRANGE
        _getSignUrlService.Setup(x => x.Execute(It.IsAny<ReqSignToSignProviderRequest>()))
            .Returns(GetSignUrlResponseFaker());
        DocModel? actual = null;
        _docBuilder.Setup(x => x.Attach(It.IsAny<DocModel>())).Callback<DocModel>(x => actual = x);
        _docBuilder.Setup(x => x.Attach(It.IsAny<DocModel>())).Returns(_docBuilder.Object);
        _docBuilder.Setup(x => x.Build()).Returns(actual!);
        var command = new RepeatSignDocCommand("Doc-A", "User-B");
        
        //  ACT
        _sut.Handle(new RepeatSignDocEvent(DocFaker(), command), CancellationToken.None);

        //  ASSERT
        _uploadDocService.Verify(x => x.Execute(It.IsAny<SendToSignProviderRequest>()), Times.Never);
        actual?.ListSignees.First().SignUrl.Should().Be("SignUrl-B");
    }
    
    [Fact]
    public void GivenDocSudahDiupload_AndSignUrlSudahTerbentuk_AndMenggunakanEmailSebagaiAcuanSign_ThenSignUrlBerubah()
    {
        //  ARRANGE
        _getSignUrlService.Setup(x => x.Execute(It.IsAny<ReqSignToSignProviderRequest>()))
            .Returns(GetSignUrlResponseFaker());
        DocModel? actual = null;
        _docBuilder.Setup(x => x.Attach(It.IsAny<DocModel>())).Callback<DocModel>(x => actual = x);
        _docBuilder.Setup(x => x.Attach(It.IsAny<DocModel>())).Returns(_docBuilder.Object);
        _docBuilder.Setup(x => x.Build()).Returns(actual!);
        var command = new RepeatSignDocCommand("Doc-A", "Email-B");
        
        //  ACT
        _sut.Handle(new RepeatSignDocEvent(DocFaker(), command), CancellationToken.None);

        //  ASSERT
        _uploadDocService.Verify(x => x.Execute(It.IsAny<SendToSignProviderRequest>()), Times.Never);
        actual?.ListSignees.First().SignUrl.Should().Be("SignUrl-B");
    }
    
    [Fact]
    public void GivenDocSudahDiupload_AndSignUrlBelumTerbentuk_ThenDocSignUrlTidakBerubah()
    {
        //  ARRANGE
        _getSignUrlService.Setup(x => x.Execute(It.IsAny<ReqSignToSignProviderRequest>()))
            .Returns((null as ReqSignToSignProviderResponse)!);
        DocModel? actual = null;
        _docBuilder.Setup(x => x.Attach(It.IsAny<DocModel>())).Callback<DocModel>(x => actual = x);
        _docBuilder.Setup(x => x.Attach(It.IsAny<DocModel>())).Returns(_docBuilder.Object);
        _docBuilder.Setup(x => x.Build()).Returns(actual!);
        var command = new RepeatSignDocCommand("Doc-A", "Email-B");
        
        //  ACT
        _sut.Handle(new RepeatSignDocEvent(DocFaker(), command), CancellationToken.None);
        
        //  ASSERT
        _uploadDocService.Verify(x => x.Execute(It.IsAny<SendToSignProviderRequest>()), Times.Never);
        actual?.ListSignees.First().SignUrl.Should().Be("SignUrlLama");
    }
    
    [Fact]
    public void GivenDocBelumDiupload_AndSignUrlTerbentuk_ThenProsesUploadDoc_AndDocSignUrlBerubah()
    {
        //  ARRANGE
        var agg = DocFaker();
        agg.UploadedDocId = string.Empty;
        _uploadDocService.Setup(x => x.Execute(It.IsAny<SendToSignProviderRequest>()))
            .Returns(new SendToSignProviderResponse{ UploadedDocId = "Doc-A1"});
        
        _getSignUrlService.Setup(x => x.Execute(It.IsAny<ReqSignToSignProviderRequest>()))
            .Returns(GetSignUrlResponseFaker());
        DocModel? actual = null;
        _docBuilder.Setup(x => x.Attach(It.IsAny<DocModel>())).Callback<DocModel>(x => actual = x);
        _docBuilder.Setup(x => x.Attach(It.IsAny<DocModel>())).Returns(_docBuilder.Object);
        _docBuilder.Setup(x => x.Build()).Returns(actual!);
        var command = new RepeatSignDocCommand("Doc-A", "User-B");
        
        //  ACT
        _sut.Handle(new RepeatSignDocEvent(agg, command), CancellationToken.None);

        //  ASSERT
        _uploadDocService.Verify(x => x.Execute(It.IsAny<SendToSignProviderRequest>()), Times.Once);
        actual?.ListSignees.First().SignUrl.Should().Be("SignUrl-B");
    }
}
