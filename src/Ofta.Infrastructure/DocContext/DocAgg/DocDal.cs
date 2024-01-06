﻿using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserOftaContext;
using Ofta.Infrastructure.Helpers;

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
                DocState, RequestedDocUrl, UploadedDocId,
                UploadedDocUrl, PublishedDocUrl)
            VALUES (
                @DocId, @DocDate, @DocTypeId, @UserOftaId, @Email,
                @DocState, @RequestedDocUrl, @UploadedDocId,
                @UploadedDocUrl, @PublishedDocUrl)";

        var dp = new DynamicParameters();
        dp.AddParam("@DocId", model.DocId, SqlDbType.VarChar);
        dp.AddParam("@DocDate", model.DocDate, SqlDbType.DateTime);
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@Email", model.Email, SqlDbType.VarChar);
        dp.AddParam("@DocState", model.DocState, SqlDbType.Int);
        dp.AddParam("@RequestedDocUrl", model.RequestedDocUrl, SqlDbType.VarChar);
        dp.AddParam("@UploadedDocId", model.UploadedDocId, SqlDbType.VarChar);
        dp.AddParam("@UploadedDocUrl", model.UploadedDocUrl, SqlDbType.VarChar);
        dp.AddParam("@PublishedDocUrl", model.PublishedDocUrl, SqlDbType.VarChar);
        
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
                RequestedDocUrl = @RequestedDocUrl, 
                UploadedDocId = @UploadedDocId,
                UploadedDocUrl = @UploadedDocUrl, 
                PublishedDocUrl = @PublishedDocUrl
            WHERE
                DocId = @DocId ";

        var dp = new DynamicParameters();
        dp.AddParam("@DocId", model.DocId, SqlDbType.VarChar);
        dp.AddParam("@DocDate", model.DocDate, SqlDbType.DateTime);
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@Email", model.Email, SqlDbType.VarChar);
        dp.AddParam("@DocState", model.DocState, SqlDbType.Int);
        dp.AddParam("@RequestedDocUrl", model.RequestedDocUrl, SqlDbType.VarChar);
        dp.AddParam("@UploadedDocId", model.UploadedDocId, SqlDbType.VarChar);
        dp.AddParam("@UploadedDocUrl", model.UploadedDocUrl, SqlDbType.VarChar);
        dp.AddParam("@PublishedDocUrl", model.PublishedDocUrl, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IDocKey key)
    {
        const string sql = @"
            UPDATE
                OFTA_Doc
            WHERE
                DocId = @DocId ";

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
                aa.DocState, aa.RequestedDocUrl, aa.UploadedDocId,
                aa.UploadedDocUrl, aa.PublishedDocUrl,
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

    public IEnumerable<DocModel> ListData(Periode filter1, IUserOftaKey filter2)
    {
        const string sql = @"
            SELECT
                aa.DocId, aa.DocDate, aa.DocTypeId, aa.UserOftaId, aa.Email,
                aa.DocState, aa.RequestedDocUrl, aa.UploadedDocId,
                aa.UploadedDocUrl, aa.PublishedDocUrl,
                ISNULL(bb.DocTypeName, '') AS DocTypeName
            FROM    
                OFTA_Doc aa
                LEFT JOIN OFTA_DocType bb ON aa.DocTypeId = bb.DocTypeId
            WHERE
                DocDate BETWEEN @Tgl1 AND @Tgl2 
                AND UserId = @UserId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@Tgl1", filter1.Tgl1, SqlDbType.DateTime);
        dp.AddParam("@Tgl2", filter1.Tgl2, SqlDbType.DateTime);
        dp.AddParam("@UserId", filter2.UserOftaId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocModel>(sql, dp);
    }
}