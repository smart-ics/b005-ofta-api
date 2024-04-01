using FluentAssertions;
using Moq;
using Ofta.Application.Helpers;
using Ofta.Application.TImelineContext.CommentAgg.Workers;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Test.Application.TimelineContext;

public class CommentBuilderTest
{
    private readonly CommentBuilder _sut;
    private readonly Mock<ITglJamDal> _tglJamDal;
    private readonly Mock<IPostDal> _postDal;
    private readonly Mock<IUserOftaDal> _userOftaDal;

    public CommentBuilderTest()
    {
        _tglJamDal = new Mock<ITglJamDal>();
        _postDal = new Mock<IPostDal>();
        _userOftaDal = new Mock<IUserOftaDal>();
        _sut = new CommentBuilder(
            _tglJamDal.Object, 
            _postDal.Object, 
            _userOftaDal.Object);
    }

    [Fact]
    public void Create_DateShouldBeNow()
    {
        _tglJamDal.Setup(x => x.Now).Returns(new DateTime(2024, 3, 31));
        var actual = _sut
            .Create()
            .Build();
        actual.CommentDate.Should().Be(new DateTime(2024,3,31));
    }

    [Fact]
    public void Create_ListReactShouldNotNull()
    {
        var actual = _sut.Create().Build();
        actual.ListReact.Should().NotBeNull();
    }
    
    [Fact]
    public void Msg_MsgShouldBeSet()
    {
        var actual = _sut.Create().Msg("A").Build();
        actual.Msg.Should().Be("A");
    }

    [Fact]
    public void User_ValidUserShouldBeSet()
    {
        _userOftaDal.Setup(x => x.GetData(It.IsAny<IUserOftaKey>()))
            .Returns(new UserOftaModel("A"));
        var actual = _sut.Create().User(new UserOftaModel("A")).Build();
        actual.UserOftaId.Should().Be("A");
    }
}