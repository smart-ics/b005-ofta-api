using System.Data;
using System.Data.SqlClient;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.Helpers.DocNumberGenerator;
using Ofta.Domain.DocContext.BulkSignAgg;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Infrastructure.Helpers;
using Xunit;

namespace Ofta.Infrastructure.DocContext.DocTypeAgg;

public class DocTypeNumberValueDal: IDocTypeNumberValueDal
{
    private readonly DatabaseOptions _opt;

    public DocTypeNumberValueDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<DocTypeNumberValueModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        
        conn.Open();
        bcp.AddMap("DocTypeId", "DocTypeId");
        bcp.AddMap("Value", "Value");
        bcp.AddMap("PeriodeHari", "PeriodeHari");
        bcp.AddMap("PeriodeBulan", "PeriodeBulan");
        bcp.AddMap("PeriodeTahun", "PeriodeTahun");

        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_DocTypeNumberValue";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(IDocTypeKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_DocTypeNumberValue
            WHERE
                DocTypeId = @DocTypeId";

        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", key.DocTypeId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<DocTypeNumberValueModel> ListData(IDocTypeKey filter)
    {
        const string sql = @"
            SELECT
                DocTypeId,
                Value, 
                PeriodeHari,
                PeriodeBulan,
                PeriodeTahun
            FROM
                OFTA_DocTypeNumberValue
            WHERE
                DocTypeId = @DocTypeId";

        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", filter.DocTypeId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocTypeNumberValueModel>(sql, dp);
    }
}

public class DocTypeNumberValueDalTest
{
    private readonly DocTypeNumberValueDal _sut;

    public DocTypeNumberValueDalTest()
    {
        _sut = new DocTypeNumberValueDal(ConnStringHelper.GetTestEnv());
    }

    [Fact]
    public void InsertTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 1,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };
        
        // ACT & ASSERT
        _sut.Insert(new List<DocTypeNumberValueModel>() { expected });
    }

    [Fact]
    public void DeleteTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
        };
        
        // ACT & ASSERT
        _sut.Delete(expected);
    }

    [Fact]
    public void ListDataTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new DocTypeNumberValueModel
        {
            DocTypeId = "A",
            Value = 1,
            PeriodeHari = 1,
            PeriodeBulan = 1,
            PeriodeTahun = 2025
        };
        _sut.Insert(new List<DocTypeNumberValueModel>() { expected });

        // ACT
        var actual = _sut.ListData(expected);
        
        // ASSERT
        actual.Should().ContainEquivalentOf(expected);
    }
}