using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.TransactionHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.TimelineContext.PostAgg;

namespace Ofta.Test.Infrastructure.TimelineContext.PostAgg;

public class PostDalTest
{
    private readonly IPostDal _sut = new PostDal(Options.Create(new DatabaseOptions
    {
        ServerName = "(Local)",
        DbName = "dev"
    }));

    private readonly PostModel _faker = new()
    {
        PostId = "A",
        PostDate = new DateTime(2024,03,29),
        UserOftaId = "B",
        UserOftaName = string.Empty,
        Msg = "C",
        DocId = "D",
        CommentCount = 1,
        LikeCount = 9
    };
    
    [Fact]
    public void InsertTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Insert(_faker);
    }

    [Fact]
    public void UpdateTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Update(_faker);
    }

    [Fact]
    public void DeleteTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Delete(_faker);
    }
    
    [Fact]
    public void GetDataTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Insert(_faker);
        var actual = _sut.GetData(_faker);
        actual.Should().BeEquivalentTo(_faker);
    }

    [Fact]
    public void ListDataTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Insert(_faker);
        var actual = _sut.ListData(new Periode(new DateTime(2024 ,3, 29)),new UserOftaModel("B"));
        actual.Should().BeEquivalentTo(new List<PostModel> { _faker });
    }
    
}