using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Test.Infrastructure.KlaimBpjsContext;

public class BlueprintDocTypeDalTest
{
    private readonly IBlueprintDocTypeDal _sut = new BlueprintDocTypeDal(Options.Create(new DatabaseOptions
    {
        ServerName = "(Local)",
        DbName = "dev",
    }));
    
    private readonly BlueprintDocTypeModel _faker = new()
    {
        BlueprintId = "A",
        BlueprintDocTypeId = "B", 
        DocTypeId = "C",
        DocTypeName = string.Empty,
        NoUrut = 3,
    };
    
    [Fact]
    public void InsertTest()
    {
        using var trans = TransHelper.NewScope();
        _sut.Insert(new List<BlueprintDocTypeModel> { _faker });
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
        _sut.Insert(new List<BlueprintDocTypeModel> { _faker });
        var actual = _sut.ListData(_faker);
        actual.Should().BeEquivalentTo(new List<BlueprintDocTypeModel> { _faker });
    }
}