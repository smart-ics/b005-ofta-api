using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Test.Infrastructure.KlaimBpjsContext;


public class BlueprintSigneeDalTest
{
    private readonly IBlueprintSigneeDal _sut = new BlueprintSigneeDal(Options.Create(new DatabaseOptions
    {
        ServerName = "(Local)",
        DbName = "dev"
    }));

    private readonly BlueprintSigneeModel _faker = new()
    {
        BlueprintId = "A",
        BlueprintDocTypeId = "B",
        BlueprintSigneeId = "C",
        NoUrut = 3,
        Email = "D",
        SignPosition = SignPositionEnum.SignLeft,
        SignTag = "E",
        UserOftaId = "F"
    };

    [Fact]
    public void InsertTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Insert(new List<BlueprintSigneeModel> { _faker });
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
        _sut.Insert(new List<BlueprintSigneeModel> { _faker });
        var actual = _sut.ListData(_faker);
        actual.Should().BeEquivalentTo(new List<BlueprintSigneeModel> { _faker });
    }
}