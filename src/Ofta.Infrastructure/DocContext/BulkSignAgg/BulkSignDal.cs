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

public class BulkSignDal: IBulkSignDal
{
    private readonly DatabaseOptions _opt;

    public BulkSignDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(BulkSignModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_BulkSign (BulkSignId, BulkSignDate, UserOftaId, DocCount, BulkSignState)
            VALUES (@BulkSignId, @BulkSignDate, @UserOftaId, @DocCount, @BulkSignState);";

        var dp = new DynamicParameters();
        dp.AddParam("@BulkSignId", model.BulkSignId, SqlDbType.VarChar);
        dp.AddParam("@BulkSignDate", model.BulkSignDate.ToString("yyyy-MM-dd"), SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@DocCount", model.DocCount, SqlDbType.Int);
        dp.AddParam("@BulkSignState", model.BulkSignState, SqlDbType.Bit);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(BulkSignModel model)
    {
        const string sql = @"
            UPDATE OFTA_BulkSign
                SET BulkSignDate = @BulkSignDate,
                    UserOftaId = @UserOftaId,
                    DocCount = @DocCount,
                    BulkSignState = @BulkSignState
            WHERE BulkSignId = @BulkSignId";

        var dp = new DynamicParameters();
        dp.AddParam("@BulkSignId", model.BulkSignId, SqlDbType.VarChar);
        dp.AddParam("@BulkSignDate", model.BulkSignDate.ToString("yyyy-MM-dd"), SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@DocCount", model.DocCount, SqlDbType.Int);
        dp.AddParam("@BulkSignState", model.BulkSignState, SqlDbType.Bit);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public BulkSignModel GetData(IBulkSignKey key)
    {
        const string sql = @"
            SELECT BulkSignId, BulkSignDate, UserOftaId, DocCount, BulkSignState
                FROM OFTA_BulkSign
            WHERE 
                BulkSignId = @BulkSignId";

        var dp = new DynamicParameters();
        dp.AddParam("@BulkSignId", key.BulkSignId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<BulkSignModel>(sql, dp);
    }
}

public class BulkSignDalTest
{
    private readonly BulkSignDal _sut;

    public BulkSignDalTest()
    {
        _sut = new BulkSignDal(ConnStringHelper.GetTestEnv());
    }

    [Fact]
    public void InsertTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new BulkSignModel
        {
            BulkSignId = "A",
            BulkSignDate = DateTime.Now,
            UserOftaId = "B",
            DocCount = 1,
            BulkSignState = BulkSignStateEnum.SuccessSign,
        };
        
        // ACT & ASSERT
        _sut.Insert(expected);
    }
    
    [Fact]
    public void UpdateTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new BulkSignModel
        {
            BulkSignId = "A",
            BulkSignDate = DateTime.Now,
            UserOftaId = "B",
            DocCount = 1,
            BulkSignState = BulkSignStateEnum.SuccessSign,
        };
        
        // ACT & ASSERT
        _sut.Update(expected);
    }
    
    [Fact]
    public void GetDataTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var faker = new BulkSignModel
        {
            BulkSignId = "A",
            BulkSignDate = DateTime.Now,
            UserOftaId = "B",
            DocCount = 1,
            BulkSignState = BulkSignStateEnum.SuccessSign,
        };
        var expected = new BulkSignModel
        {
            BulkSignId = "A",
            BulkSignDate = DateTime.Now.Date,
            UserOftaId = "B",
            DocCount = 1,
            BulkSignState = BulkSignStateEnum.SuccessSign,
        };
        _sut.Insert(faker);
        
        // ACT
        var actual = _sut.GetData(faker);
        
        // ASSERT
        actual.Should().BeEquivalentTo(expected);
    }
}