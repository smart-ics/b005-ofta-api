using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.TransactionHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.KlaimBpjsContext.OrderKlaimBpjsAgg;

namespace Ofta.Test.Infrastructure.KlaimBpjsContext;

public class OrderKlaimBpjsDalTest
{
    private readonly IOrderKlaimBpjsDal _sut = new OrderKlaimBpjsDal(Options.Create(new DatabaseOptions
    {
        ServerName = "(Local)",
        DbName = "dev",
    }));

    private readonly OrderKlaimBpjsModel _faker =
        new OrderKlaimBpjsModel
        {
            OrderKlaimBpjsId = "A",
            OrderKlaimBpjsDate = new DateTime(2024, 4, 5),
            UserOftaId = "B",
            KlaimBpjsId = "B1", 
            RegId = "C",
            PasienId = "D",
            PasienName = "E",
            NoSep = "F",
            LayananName = "G",
            DokterName = "H",
            RajalRanap = RajalRanapEnum.Rajal,
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
        var actual = _sut.ListData(new Periode(new DateTime(2024,4,5)));
        actual.Should().BeEquivalentTo(new List<OrderKlaimBpjsModel> { _faker });
    }
}