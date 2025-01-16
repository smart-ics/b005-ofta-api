using System.Data;
using System.Data.SqlClient;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.Helpers.DocNumberGenerator;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Infrastructure.Helpers;
using Xunit;

namespace Ofta.Infrastructure.DocContext.DocTypeAgg;

public class DocTypeNumberFormatDal: IDocTypeNumberFormatDal
{
    private readonly DatabaseOptions _opt;

    public DocTypeNumberFormatDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(DocTypeNumberFormatModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_DocTypeNumberFormat(
                DocTypeId,
                Format, 
                ResetBy)
            VALUES (
                @DocTypeId,
                @Format, 
                @ResetBy)";

        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@Format", model.Format, SqlDbType.VarChar);
        dp.AddParam("@ResetBy", model.ResetBy, SqlDbType.Int);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(DocTypeNumberFormatModel model)
    {
        const string sql = @"
            UPDATE
                OFTA_DocTypeNumberFormat
            SET 
                Format = @Format,
                ResetBy = @ResetBy
            WHERE
                DocTypeId = @DocTypeId";

        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@Format", model.Format, SqlDbType.VarChar);
        dp.AddParam("@ResetBy", model.ResetBy, SqlDbType.Int);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IDocTypeKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_DocTypeNumberFormat
            WHERE
                DocTypeId = @DocTypeId";

        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", key.DocTypeId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public DocTypeNumberFormatModel GetData(IDocTypeKey key)
    {
        const string sql = @"
            SELECT
                DocTypeId,
                Format, 
                ResetBy
            FROM
                OFTA_DocTypeNumberFormat
            WHERE
                DocTypeId = @DocTypeId";

        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", key.DocTypeId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<DocTypeNumberFormatModel>(sql, dp);
    }
}

public class DocTypeNumberFormatDalTest 
{
    private readonly DocTypeNumberFormatDal _sut;

    public DocTypeNumberFormatDalTest()
    {
        _sut = new DocTypeNumberFormatDal(ConnStringHelper.GetTestEnv());
    }

    [Fact]
    public void InsertTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Month
        };

        // ACT & ASSERT
        _sut.Insert(expected);
    }
    
    [Fact]
    public void UpdateTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Month
        };

        // ACT & ASSERT
        _sut.Update(expected);
    }
    
    [Fact]
    public void DeleteTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Month
        };

        // ACT & ASSERT
        _sut.Delete(expected);
    }
    
    [Fact]
    public void GetDataTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new DocTypeNumberFormatModel
        {
            DocTypeId = "A",
            Format = "B",
            ResetBy = ResetByEnum.Month
        };
        _sut.Insert(expected);

        // ACT
        var actual = _sut.GetData(expected);
        
        // ASSERT
        actual.Should().BeEquivalentTo(expected);
    }
}