using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.TImelineContext.CommentAgg.Contracts;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.TimelineContext.CommentAgg;

namespace Ofta.Test.Infrastructure.TimelineContext.CommentAgg;

public class CommentReactDalTest
{
    private readonly ICommentReactDal _sut = new CommentReactDal(Options.Create(new DatabaseOptions
    {
        ServerName = "(Local)",
        DbName = "dev"
    }));
    
    private readonly CommentReactModel _faker = new()
    {
        CommentId = "A",
        CommentReactDate = new DateTime(2024,3,29), 
        UserOftaId = "B"
    };
    
    [Fact]
    public void InsertTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Insert(new List<CommentReactModel> { _faker });
    }

    [Fact]
    public void DeleteTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Delete(_faker);
    }

    [Fact]
    public void ListDataTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Insert(new List<CommentReactModel> { _faker });
        var actual = _sut.ListData(_faker);
        actual.Should().BeEquivalentTo(new List<CommentReactModel> { _faker });
    }
}