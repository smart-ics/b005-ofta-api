using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.TransactionHelper;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.UserContext.TeamAgg;

namespace Ofta.Test.Infrastructure.UserContext;

public class TeamUserOftaDalTest
{
    private readonly TeamUserOftaDal _sut = new TeamUserOftaDal(Options.Create(new DatabaseOptions
    {
        ServerName = "(Local)",
        DbName = "dev",
    }));
    
    private readonly TeamUserOftaModel _faker = new()
    {
        TeamId = "A",
        UserOftaId = "B"
    };

    [Fact]
    public void InsertTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Insert(new List<TeamUserOftaModel> { _faker });
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
        _sut.Insert(new List<TeamUserOftaModel> { _faker });
        var actual = _sut.ListData(_faker);
        actual.Should().BeEquivalentTo(new List<TeamUserOftaModel> { _faker });
    }
}