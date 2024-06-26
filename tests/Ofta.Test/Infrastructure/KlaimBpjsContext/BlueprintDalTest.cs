﻿using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Test.Infrastructure.KlaimBpjsContext;

public class BlueprintDalTest
{
    private readonly IBlueprintDal _sut = new BlueprintDal(Options.Create(new DatabaseOptions
    {
        ServerName = "(Local)",
        DbName = "dev"
    }));

    private readonly BlueprintModel _faker = new()
    {
        BlueprintId = "A",
        BlueprintName = "B"
    };
    
    //  write unit test for BluepirntDal.Insert
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
        actual.Should().BeEquivalentTo(new List<BlueprintModel> { _faker });
    }
}