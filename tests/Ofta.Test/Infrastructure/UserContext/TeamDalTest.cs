using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.TransactionHelper;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.UserContext.TeamAgg;

namespace Ofta.Test.Infrastructure.UserContext;

public class TeamDalTest
{
    private readonly TeamDal _sut = new TeamDal(Options.Create(new DatabaseOptions
    {
        ServerName = "(Local)",
        DbName = "dev",
    }));
    
    private readonly TeamModel _faker = new()
    {
        TeamId = "A",
        TeamName = "B"
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
        var actual = _sut.ListData();
        actual.Should().BeEquivalentTo(new List<TeamModel> { _faker });
    }
}