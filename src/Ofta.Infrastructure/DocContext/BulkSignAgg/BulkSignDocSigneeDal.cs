using System.Data;
using System.Data.SqlClient;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Domain.DocContext.BulkSignAgg;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Infrastructure.Helpers;
using Xunit;

namespace Ofta.Infrastructure.DocContext.BulkSignAgg;

public class BulkSignDocSigneeDal: IBulkSignDocSigneeDal
{
    private readonly DatabaseOptions _opt;

    public BulkSignDocSigneeDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }


    public void Insert(IEnumerable<BulkSignDocSigneeModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        
        conn.Open();
        bcp.AddMap("DocId", "DocId");
        bcp.AddMap("UserOftaId", "UserOftaId");
        bcp.AddMap("Email", "Email");
        bcp.AddMap("SignTag", "SignTag");
        bcp.AddMap("SignPosition", "SignPosition");
        bcp.AddMap("SignPositionDesc", "SignPositionDesc");
        bcp.AddMap("SignUrl", "SignUrl");

        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_BulkSignDocSignee";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(IDocKey key)
    {
        const string sql = @"
            DELETE 
                FROM OFTA_BulkSignDocSignee
            WHERE
                DocId = @DocId";

        var dp = new DynamicParameters();
        dp.AddParam("@DocId", key.DocId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<BulkSignDocSigneeModel> ListData(IDocKey filter)
    {
        const string sql = @"
            SELECT 
                DocId, UserOftaId, Email, SignTag,
                SignPosition, SignPositionDesc, SignUrl
            FROM 
                OFTA_BulkSignDocSignee
            WHERE 
                DocId = @DocId";

        var dp = new DynamicParameters();
        dp.AddParam("@DocId", filter.DocId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<BulkSignDocSigneeModel>(sql, dp);
    }
}

public class BulkSignDocSigneeDalTest
{
    private readonly BulkSignDocSigneeDal _sut;

    public BulkSignDocSigneeDalTest()
    {
        _sut = new BulkSignDocSigneeDal(ConnStringHelper.GetTestEnv());
    }

    [Fact]
    public void InsertTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new BulkSignDocSigneeModel
        {
            DocId = "DOC-1",
            UserOftaId = "USER-001",
            Email = "user@email.com",
            SignTag = "Mengetahui",
            SignPosition = SignPositionEnum.SignLeft,
            SignPositionDesc = "",
            SignUrl = "",
        };
        
        // ACT & ASSERT
        _sut.Insert(new List<BulkSignDocSigneeModel>() { expected });
    }

    [Fact]
    public void DeleteTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new BulkSignDocSigneeModel
        {
            DocId = "DOC-1",
        };
        
        // ACT & ASSERT
        _sut.Delete(expected);
    }

    [Fact]
    public void ListDataTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new BulkSignDocSigneeModel
        {
            DocId = "DOC-1",
            UserOftaId = "USER-001",
            Email = "user@email.com",
            SignTag = "Mengetahui",
            SignPosition = SignPositionEnum.SignLeft,
            SignPositionDesc = "",
            SignUrl = "",
        };
        _sut.Insert(new List<BulkSignDocSigneeModel>() { expected });

        // ACT
        var actual = _sut.ListData(expected);
        
        // ASSERT
        actual.Should().ContainEquivalentOf(expected);
    }
}