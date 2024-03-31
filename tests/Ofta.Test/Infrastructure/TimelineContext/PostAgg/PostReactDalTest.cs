using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.TimelineContext.PostAgg;

namespace Ofta.Test.Infrastructure.TimelineContext.PostAgg;

public class PostReactDalTest
{
    private readonly IPostReactDal _sut = new PostReactDal(Options.Create(new DatabaseOptions
    {
        ServerName = "(Local)",
        DbName = "dev",
    }));
    
    private readonly PostReactModel _faker = new()
    {
        PostId = "A",
        PostReactDate = new DateTime(2024,3,29), 
        UserOftaId = "B",
    };
    
    [Fact]
    public void InsertTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Insert(new List<PostReactModel> { _faker });
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
        _sut.Insert(new List<PostReactModel> { _faker });
        var actual = _sut.ListData(_faker);
        actual.Should().BeEquivalentTo(new List<PostReactModel> { _faker });
    }
}