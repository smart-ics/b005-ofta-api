using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.TImelineContext.CommentAgg.Contracts;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.TimelineContext.CommentAgg;

namespace Ofta.Test.Infrastructure.TimelineContext.CommentAgg;

public class CommentDalTest
{
    private readonly ICommentDal _sut = new CommentDal(Options.Create(new DatabaseOptions
    {
        ServerName = "(Local)",
        DbName = "dev",
    }));

    private readonly CommentModel _faker = new()
    {
        CommentId = "A",
        CommentDate = new DateTime(2024,03,29),
        PostId = "B",
        UserOftaId = "C",
        Msg = "D",
        ReactCount = 2,
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
        var actual = _sut.ListData(new PostModel("B"));
        actual.Should().BeEquivalentTo(new List<CommentModel> { _faker });
    }
}