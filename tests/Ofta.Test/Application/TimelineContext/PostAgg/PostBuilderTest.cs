using FluentAssertions;
using Moq;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.Helpers;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Test.Application.TimelineContext.PostAgg;

public class PostBuilderTest
{
    private readonly PostBuilder _sut;
    private readonly Mock<ITglJamDal> _tglJamDal;
    private readonly Mock<IUserOftaDal> _userOftaDal;
    private readonly Mock<IPostDal> _postDal;
    private readonly Mock<IPostReactDal> _postReactDal;
    private readonly Mock<IPostVisibilityDal> _postVisibilityDal;
    private readonly Mock<IDocDal> _docDal;

    public PostBuilderTest()
    {
        _tglJamDal = new Mock<ITglJamDal>();
        _userOftaDal = new Mock<IUserOftaDal>();
        _postDal = new Mock<IPostDal>();
        _postVisibilityDal = new Mock<IPostVisibilityDal>();
        _postReactDal = new Mock<IPostReactDal>();
        _docDal = new Mock<IDocDal>();
        
        _sut = new PostBuilder(
            _tglJamDal.Object, 
            _userOftaDal.Object,
            _postDal.Object,
            _postVisibilityDal.Object,
            _postReactDal.Object,
            _docDal.Object);
    }

    [Fact]
    public void Create_PostDateShouldNow_AllListShouldNew()
    {
        //  ARRANGE
        _tglJamDal.Setup(x => x.Now).Returns(new DateTime(2024, 3, 31));
        //  ACT
        var actual = _sut.Create().Build();
        //  ASSERT
        actual.PostDate.Should().Be(new DateTime(2024, 3, 31));
        actual.ListReact.Should().NotBeNull();
        actual.ListVisibility.Should().NotBeNull();
    }

    [Fact]
    public void Msg_MsgShouldBeAsPassingParam()
    {
        var actual = _sut.Create().Msg("abc").Build();
        actual.Msg.Should().Be("abc");
    }

    [Fact]
    public void User_UserShouldBeAsPassingParam()
    {
        _userOftaDal
            .Setup(x => x.GetData(It.IsAny<IUserOftaKey>()))
            .Returns(new UserOftaModel
            {
                UserOftaId = "A",
                UserOftaName = "B"
            });
        var actual = _sut.Create().User(new UserOftaModel("A")).Build();
        actual.UserOftaId.Should().Be("A");
        actual.UserOftaName.Should().Be("B");
    }
    
    [Fact]
    public void AddVisibility_VisibilityAdded()
    {
        var actual = _sut.Create().AddVisibility("A").Build();
        actual.ListVisibility.Count.Should().Be(1);
        actual.ListVisibility.First().VisibilityReff.Should().Be("A");
    }
    
    [Fact]
    public void AddVisibility_NotDuplicatedVisibility()
    {
        var actual = _sut.Create()
            .AddVisibility("A")
            .AddVisibility("A")
            .Build();
        actual.ListVisibility.Count.Should().Be(1);
        actual.ListVisibility.First().VisibilityReff.Should().Be("A");
    }
    
    [Fact]
    public void RemoveVisibility_VisibilityReffDeleted()
    {
        var actual = _sut.Create()
            .AddVisibility("A")
            .RemoveVisibility("A")
            .Build();
        actual.ListVisibility.Count.Should().Be(0);
    }

    [Fact]
    public void Load_LoadExistedPost()
    {
        _postDal.Setup(x => x.GetData(It.IsAny<IPostKey>()))
            .Returns(new PostModel("A"));
        _postReactDal.Setup(x => x.ListData(It.IsAny<IPostKey>()))
            .Returns(new List<PostReactModel>());
        _postVisibilityDal.Setup(x => x.ListData(It.IsAny<IPostKey>()))
            .Returns(new List<PostVisibilityModel>());

        var actual = _sut.Load(new PostModel("A")).Build();
        actual.PostId.Should().Be("A");
        actual.ListVisibility.Should().NotBeNull();
        actual.ListReact.Should().NotBeNull();
    }
    
    [Fact]
    public void AttachDoc_ValidDocAttached()
    {
        IDocKey docKey = new DocModel("A");
        _docDal
            .Setup(x => x.GetData(docKey))
            .Returns(new DocModel("A"));

        var actual = _sut.Create().AttachDoc(docKey).Build();
        actual.DocId.Should().Be("A");
    }
}