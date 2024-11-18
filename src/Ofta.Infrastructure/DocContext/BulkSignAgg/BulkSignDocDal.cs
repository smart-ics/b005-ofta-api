using System.Data;
using System.Data.SqlClient;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Domain.DocContext.BulkSignAgg;
using Ofta.Infrastructure.Helpers;
using Xunit;

namespace Ofta.Infrastructure.DocContext.BulkSignAgg;

public class BulkSignDocDal: IBulkSignDocDal
{
    private readonly DatabaseOptions _opt;

    public BulkSignDocDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<BulkSignDocModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        
        conn.Open();
        bcp.AddMap("BulkSignId", "BulkSignId");
        bcp.AddMap("DocId", "DocId");
        bcp.AddMap("UploadedDocId", "UploadedDocId");
        bcp.AddMap("RequestBulkSignState", "RequestBulkSignState");
        bcp.AddMap("NoUrut", "NoUrut");

        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_BulkSignDoc";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(IBulkSignKey key)
    {
        const string sql = @"
            DELETE 
                FROM OFTA_BulkSignDoc
            WHERE
                BulkSignId = @BulkSignId";

        var dp = new DynamicParameters();
        dp.AddParam("@BulkSignId", key.BulkSignId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<BulkSignDocModel> ListData(IBulkSignKey filter)
    {
        const string sql = @"
            SELECT 
                BulkSignId, DocId, UploadedDocId, RequestBulkSignState, NoUrut
            FROM 
                OFTA_BulkSignDoc
            WHERE 
                BulkSignId = @BulkSignId";

        var dp = new DynamicParameters();
        dp.AddParam("@BulkSignId", filter.BulkSignId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<BulkSignDocModel>(sql, dp);
    }
}

public class BulkSignDocDalTest
{
    private readonly BulkSignDocDal _sut;

    public BulkSignDocDalTest()
    {
        _sut = new BulkSignDocDal(ConnStringHelper.GetTestEnv());
    }

    [Fact]
    public void InsertTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC-1",
            UploadedDocId = "UPLOADED-DOC-1",
            RequestBulkSignState = RequestBulkSignStateEnum.Success,
            NoUrut = 1,
        };
        
        // ACT & ASSERT
        _sut.Insert(new List<BulkSignDocModel>() { expected });
    }

    [Fact]
    public void DeleteTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new BulkSignDocModel
        {
            BulkSignId = "A",
        };
        
        // ACT & ASSERT
        _sut.Delete(expected);
    }

    [Fact]
    public void ListDataTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new BulkSignDocModel
        {
            BulkSignId = "A",
            DocId = "DOC-1",
            UploadedDocId = "UPLOADED-DOC-1",
            RequestBulkSignState = RequestBulkSignStateEnum.Success,
            NoUrut = 1,
        };
        _sut.Insert(new List<BulkSignDocModel>() { expected });

        // ACT
        var actual = _sut.ListData(expected);
        
        // ASSERT
        actual.Should().ContainEquivalentOf(expected);
    }
}