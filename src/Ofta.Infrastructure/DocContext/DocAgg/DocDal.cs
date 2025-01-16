using System.Data;
using System.Data.SqlClient;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.TransactionHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Ofta.Infrastructure.Helpers;
using Xunit;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class DocDal : IDocDal
{
    private readonly DatabaseOptions _opt;

    public DocDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(DocModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_Doc (
                DocId, DocDate, DocTypeId, UserOftaId, Email,
                DocState, DocName, RequestedDocUrl, UploadedDocId,
                UploadedDocUrl, PublishedDocUrl, DocNumber)
            VALUES (
                @DocId, @DocDate, @DocTypeId, @UserOftaId, @Email,
                @DocState,@DocName, @RequestedDocUrl, @UploadedDocId,
                @UploadedDocUrl, @PublishedDocUrl, @DocNumber)";

        var dp = new DynamicParameters();
        dp.AddParam("@DocId", model.DocId, SqlDbType.VarChar);
        dp.AddParam("@DocDate", model.DocDate, SqlDbType.DateTime);
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@Email", model.Email, SqlDbType.VarChar);
        dp.AddParam("@DocState", model.DocState, SqlDbType.Int);
        dp.AddParam("@DocName", model.DocName, SqlDbType.VarChar);
        dp.AddParam("@RequestedDocUrl", model.RequestedDocUrl, SqlDbType.VarChar);
        dp.AddParam("@UploadedDocId", model.UploadedDocId, SqlDbType.VarChar);
        dp.AddParam("@UploadedDocUrl", model.UploadedDocUrl, SqlDbType.VarChar);
        dp.AddParam("@PublishedDocUrl", model.PublishedDocUrl, SqlDbType.VarChar);
        dp.AddParam("@DocNumber", model.DocNumber, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(DocModel model)
    {
        const string sql = @"
            UPDATE
                OFTA_Doc
            SET
                DocId = @DocId, 
                DocDate = @DocDate, 
                DocTypeId = @DocTypeId, 
                UserOftaId = @UserOftaId, 
                Email = @Email,
                DocState = @DocState, 
                DocName = @DocName,
                RequestedDocUrl = @RequestedDocUrl, 
                UploadedDocId = @UploadedDocId,
                UploadedDocUrl = @UploadedDocUrl, 
                PublishedDocUrl = @PublishedDocUrl,
                DocNumber = @DocNumber
            WHERE
                DocId = @DocId ";

        var dp = new DynamicParameters();
        dp.AddParam("@DocId", model.DocId, SqlDbType.VarChar);
        dp.AddParam("@DocDate", model.DocDate, SqlDbType.DateTime);
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@Email", model.Email, SqlDbType.VarChar);
        dp.AddParam("@DocState", model.DocState, SqlDbType.Int);
        dp.AddParam("@DocName", model.DocName, SqlDbType.VarChar);
        dp.AddParam("@RequestedDocUrl", model.RequestedDocUrl, SqlDbType.VarChar);
        dp.AddParam("@UploadedDocId", model.UploadedDocId, SqlDbType.VarChar);
        dp.AddParam("@UploadedDocUrl", model.UploadedDocUrl, SqlDbType.VarChar);
        dp.AddParam("@PublishedDocUrl", model.PublishedDocUrl, SqlDbType.VarChar);
        dp.AddParam("@DocNumber", model.DocNumber, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IDocKey key)
    {
        const string sql = @"
            DELETE 
                FROM OFTA_Doc
            WHERE
                DocId = @DocId";

        var dp = new DynamicParameters();
        dp.AddParam("@DocId", key.DocId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public DocModel GetData(IDocKey key)
    {
        const string sql = @"
            SELECT
                aa.DocId, aa.DocDate, aa.DocTypeId, aa.UserOftaId, aa.Email,
                aa.DocState,aa.DocName, aa.RequestedDocUrl, aa.UploadedDocId,
                aa.UploadedDocUrl, aa.PublishedDocUrl, aa.DocNumber,
                ISNULL(bb.DocTypeName, '') AS DocTypeName
            FROM    
                OFTA_Doc aa
                LEFT JOIN OFTA_DocType bb ON aa.DocTypeId = bb.DocTypeId
            WHERE
                DocId = @DocId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DocId", key.DocId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<DocModel>(sql, dp);
    }

    public DocModel GetData(IUploadedDocKey key)
    {
        const string sql = @"
            SELECT
                aa.DocId, aa.DocDate, aa.DocTypeId, aa.UserOftaId, aa.Email,
                aa.DocState,aa.DocName, aa.RequestedDocUrl, aa.UploadedDocId,
                aa.UploadedDocUrl, aa.PublishedDocUrl, aa.DocNumber,
                ISNULL(bb.DocTypeName, '') AS DocTypeName
            FROM    
                OFTA_Doc aa
                LEFT JOIN OFTA_DocType bb ON aa.DocTypeId = bb.DocTypeId
            WHERE
                UploadedDocId = @UploadedDocId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UploadedDocId", key.UploadedDocId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<DocModel>(sql, dp);
    }
    
    public IEnumerable<DocModel> ListData(Periode filter1, IUserOftaKey filter2)
    {
        const string sql = @"
            SELECT
                aa.DocId, aa.DocDate, aa.DocTypeId, aa.UserOftaId, aa.Email,
                aa.DocState,aa.DocName, aa.RequestedDocUrl, aa.UploadedDocId,
                aa.UploadedDocUrl, aa.PublishedDocUrl, aa.DocNumber,
                ISNULL(bb.DocTypeName, '') AS DocTypeName
            FROM    
                OFTA_Doc aa
                LEFT JOIN OFTA_DocType bb ON aa.DocTypeId = bb.DocTypeId
            WHERE
                DocDate BETWEEN @Tgl1 AND @Tgl2 
                AND UserOftaId = @UserId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@Tgl1", filter1.Tgl1, SqlDbType.DateTime);
        dp.AddParam("@Tgl2", filter1.Tgl2, SqlDbType.DateTime);
        dp.AddParam("@UserId", filter2.UserOftaId , SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocModel>(sql, dp);
    }

    public IEnumerable<DocModel> ListData(IEnumerable<string> filter1, int pageNo)
    {
        const int PAGE_SIZE = 50;
        const string sql = @"
            SELECT DISTINCT
                aa.DocId, aa.DocDate, aa.DocTypeId, aa.UserOftaId, aa.Email,
                aa.DocState,aa.DocName, aa.RequestedDocUrl, aa.UploadedDocId,
                aa.UploadedDocUrl, aa.PublishedDocUrl, aa.DocNumber,
                ISNULL(bb.DocTypeName, '') AS DocTypeName
            FROM    
                OFTA_Doc aa
                LEFT JOIN OFTA_DocType bb ON aa.DocTypeId = bb.DocTypeId
                LEFT JOIN OFTA_DocScope cc ON aa.DocId = cc.DocId
            WHERE
                cc.ScopeReffId IN @ScopeReffIds
            ORDER BY
                aa.DocId DESC
            OFFSET 
                (@pageNumber - 1) * 50 ROWS
                FETCH NEXT 50 ROWS ONLY";

        var dp = new DynamicParameters();
        dp.Add("@ScopeReffIds", filter1.Select(x => x).ToArray());
        dp.AddParam("@pageNumber", pageNo, SqlDbType.Int);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocModel>(sql, dp);
    }

    public IEnumerable<DocModel> ListData(IEnumerable<string> filter)
    {
        const string sql = @"
            SELECT DISTINCT
                aa.DocId, aa.DocDate, aa.DocTypeId, aa.UserOftaId, aa.Email,
                aa.DocState,aa.DocName, aa.RequestedDocUrl, aa.UploadedDocId,
                aa.UploadedDocUrl, aa.PublishedDocUrl, aa.DocNumber,
                ISNULL(bb.DocTypeName, '') AS DocTypeName
            FROM    
                OFTA_Doc aa
                LEFT JOIN OFTA_DocType bb ON aa.DocTypeId = bb.DocTypeId
                LEFT JOIN OFTA_DocScope cc ON aa.DocId = cc.DocId
            WHERE
                cc.ScopeReffId IN @ScopeReffIds
            ORDER BY
                aa.DocId DESC";

        var dp = new DynamicParameters();
        dp.Add("@ScopeReffIds", filter.Select(x => x).ToArray());
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocModel>(sql, dp);
    }
}

public class DocDalTest
{
    private readonly DocScopeDal _docScopeDal;
    private readonly DocDal _sut;

    public DocDalTest()
    {
        _docScopeDal = new DocScopeDal(ConnStringHelper.GetTestEnv());
        _sut = new DocDal(ConnStringHelper.GetTestEnv());
    }

    private static DocModel FakerDoc() => new()
    {
        DocId = "A",
        DocDate = new DateTime(2025, 1, 15),
        DocTypeId = "B",
        DocTypeName = string.Empty,
        UserOftaId = "D",
        Email = "E",
        DocState = DocStateEnum.Created,
        DocName = "F",
        RequestedDocUrl = "G",
        UploadedDocId = "H",
        UploadedDocUrl = "I",
        PublishedDocUrl = "J",
        DocNumber = "K"
    };

    private static Periode FakerPeriode() => new(new DateTime(2025, 1, 1), new DateTime(2025, 2, 1));

    private static List<AbstractDocScopeModel> FakerScopes() =>
    [
        new DocScopeUserModel
        {
            DocId = "A",
            ScopeType = 0,
            UserOftaId = "D"
        }
    ];

    [Fact]
    public void InsertTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = FakerDoc();

        // ACT & ASSERT
        _sut.Insert(expected);
    }
    
    [Fact]
    public void UpdateTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = FakerDoc();

        // ACT & ASSERT
        _sut.Update(expected);
    }
    
    [Fact]
    public void DeleteTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = FakerDoc();

        // ACT & ASSERT
        _sut.Delete(expected);
    }
    
    [Fact]
    public void GetDataByDocIdTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = FakerDoc();
        _sut.Insert(expected);

        // ACT
        var actual = _sut.GetData(expected as IDocKey);
        
        // ASSERT
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetDataByUploadedDocIdTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = FakerDoc();
        _sut.Insert(expected);

        // ACT
        var actual = _sut.GetData(expected as IUploadedDocKey);
        
        // ASSERT
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ListDataByUserOftaIdAndPeriodeTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = FakerDoc();
        var fakerPeriode = FakerPeriode();
        var fakerScopes = FakerScopes();
        
        _docScopeDal.Insert(fakerScopes);
        _sut.Insert(expected);

        // ACT
        var actual = _sut.ListData(fakerPeriode, expected);
        
        // ASSERT
        actual.Should().ContainEquivalentOf(expected);
    }
    
    [Fact]
    public void ListDataByScopeListAndPageNumberTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = FakerDoc();
        var fakerScopes = FakerScopes();
        var fakerListScope = new List<string>() { "A", "D" };
        
        _docScopeDal.Insert(fakerScopes);
        _sut.Insert(expected);

        // ACT
        var actual = _sut.ListData(fakerListScope, 1);
        
        // ASSERT
        actual.Should().ContainEquivalentOf(expected);
    }
    
    [Fact]
    public void ListDataByScopeListTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = FakerDoc();
        var fakerScopes = FakerScopes();
        var fakerListScope = new List<string>() { "A", "D" };
        
        _docScopeDal.Insert(fakerScopes);
        _sut.Insert(expected);

        // ACT
        var actual = _sut.ListData(fakerListScope);
        
        // ASSERT
        actual.Should().ContainEquivalentOf(expected);
    }
}