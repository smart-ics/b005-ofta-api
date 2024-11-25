using System.Data;
using System.Data.SqlClient;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Ofta.Infrastructure.Helpers;
using Xunit;

namespace Ofta.Infrastructure.CallbackContext.CallbackSignStatusAgg;

public class CallbackSignStatusDocDal: ICallbackSignStatusDocDal
{
    private readonly DatabaseOptions _opt;

    public CallbackSignStatusDocDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<CallbackSignStatusDocModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        
        conn.Open();
        bcp.AddMap("RequestId", "RequestId");
        bcp.AddMap("TilakaName", "TilakaName");
        bcp.AddMap("UploadedDocId", "UploadedDocId");
        bcp.AddMap("UserSignState", "UserSignState");
        bcp.AddMap("DownloadDocUrl", "DownloadDocUrl");

        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_CallbackSignStatusDoc";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(ICallbackSignStatusKey key)
    {
        const string sql = @"
            DELETE 
                FROM OFTA_CallbackSignStatusDoc
            WHERE
                RequestId = @RequestId
            AND
                TilakaName = @TilakaName";

        var dp = new DynamicParameters();
        dp.AddParam("@RequestId", key.RequestId, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", key.TilakaName, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<CallbackSignStatusDocModel> ListData(ICallbackSignStatusKey filter)
    {
        const string sql = @"
            SELECT 
                RequestId, TilakaName, UploadedDocId, UserSignState, DownloadDocUrl
            FROM 
                OFTA_CallbackSignStatusDoc
            WHERE 
                RequestId = @RequestId
            AND
                TilakaName = @TilakaName";

        var dp = new DynamicParameters();
        dp.AddParam("@RequestId", filter.RequestId, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", filter.TilakaName, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<CallbackSignStatusDocModel>(sql, dp);
    }
}

public class CallbackSignStatusDocDalTest 
{
    private readonly CallbackSignStatusDocDal _sut;

    public CallbackSignStatusDocDalTest()
    {
        _sut = new CallbackSignStatusDocDal(ConnStringHelper.GetTestEnv());
    }

    [Fact]
    public void InsertTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackSignStatusDocModel
        {
            RequestId = "A",
            TilakaName = "B",
            UploadedDocId = "C",
            DownloadDocUrl = "D",
            UserSignState = SignStateEnum.NotSigned
        };

        // ACT & ASSERT
        _sut.Insert(new List<CallbackSignStatusDocModel>() { expected });
    }
    
    [Fact]
    public void DeleteTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackSignStatusDocModel
        {
            RequestId = "A",
            TilakaName = "B",
        };

        // ACT & ASSERT
        _sut.Delete(expected);
    }
    
    [Fact]
    public void GetDataTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackSignStatusDocModel
        {
            RequestId = "A",
            TilakaName = "B",
            UploadedDocId = "C",
            DownloadDocUrl = "D",
            UserSignState = SignStateEnum.NotSigned
        };
        _sut.Insert(new List<CallbackSignStatusDocModel>() { expected });

        // ACT
        var actual = _sut.ListData(expected);
        
        // ASSERT
        actual.Should().ContainEquivalentOf(expected);
    }
}