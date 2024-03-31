using FluentAssertions;
using Moq;
using Ofta.Application.Helpers;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Test.Application.TimelineContext.PostAgg;

public class PostBuilderTest
{
    private readonly PostBuilder _sut;
    private readonly Mock<ITglJamDal> _tglJamDal;
    private readonly Mock<IUserOftaDal> _userOftaDal;

    public PostBuilderTest()
    {
        _tglJamDal = new Mock<ITglJamDal>();
        _userOftaDal = new Mock<IUserOftaDal>();
        _sut = new PostBuilder(
            _tglJamDal.Object, 
            _userOftaDal.Object);
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
}