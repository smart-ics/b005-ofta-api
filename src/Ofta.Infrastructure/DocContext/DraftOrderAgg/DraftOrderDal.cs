using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DraftOrderAgg;
using Ofta.Domain.DocContext.DraftOrderAgg;
using Ofta.Infrastructure.Helpers;
using Usman.Lib.NetStandard.Interfaces;

namespace Ofta.Infrastructure.DocContext.DraftOrderAgg;

public class DraftOrderDal: IDraftOrderDal
{
    private readonly DatabaseOptions _opt;

    public DraftOrderDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(DraftOrderModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_DraftOrder(DraftOrderId, DraftOrderDate, RequesterUserId, DrafterUserId, DocTypeId, Context, ContextReffId)
            VALUES(@DraftOrderId, @DraftOrderDate, @RequesterUserId, @DrafterUserId, @DocTypeId, @Context, @ContextReffId)";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DraftOrderId", model.DraftOrderId, SqlDbType.VarChar);
        dp.AddParam("@DraftOrderDate", model.DraftOrderDate, SqlDbType.DateTime);
        dp.AddParam("@RequesterUserId", model.RequesterUserId, SqlDbType.VarChar);
        dp.AddParam("@DrafterUserId", model.DrafterUserId, SqlDbType.VarChar);
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@Context", model.Context, SqlDbType.VarChar);
        dp.AddParam("@ContextReffId", model.ContextReffId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(DraftOrderModel model)
    {
        const string sql = @"
            UPDATE 
                OFTA_DraftOrder
            SET 
                DraftOrderDate = @DraftOrderDate,
                RequesterUserId = @RequesterUserId,
                DrafterUserId = @DrafterUserId,
                DocTypeId = @DocTypeId,
                Context = @Context,
                ContextReffId = @ContextReffId
            WHERE
                DraftOrderId = @DraftOrderId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DraftOrderId", model.DraftOrderId, SqlDbType.VarChar);
        dp.AddParam("@DraftOrderDate", model.DraftOrderDate, SqlDbType.DateTime);
        dp.AddParam("@RequesterUserId", model.RequesterUserId, SqlDbType.VarChar);
        dp.AddParam("@DrafterUserId", model.DrafterUserId, SqlDbType.VarChar);
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@Context", model.Context, SqlDbType.VarChar);
        dp.AddParam("@ContextReffId", model.ContextReffId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IDraftOrderKey key)
    {
        const string sql = @"
            DELETE FROM 
                OFTA_DraftOrder
            WHERE
                DraftOrderId = @DraftOrderId";

        var dp = new DynamicParameters();
        dp.AddParam("@DraftOrderId", key.DraftOrderId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public DraftOrderModel GetData(IDraftOrderKey key)
    {
        const string sql = @"
            SELECT
                aa.DraftOrderId, aa.DraftOrderDate, aa.RequesterUserId, aa.DrafterUserId,
                aa.DocTypeId, aa.Context, aa.ContextReffId,
                ISNULL(bb.DocTypeName, '') AS DocTypeName
            FROM
                OFTA_DraftOrder aa
                LEFT JOIN OFTA_DocType bb ON aa.DocTypeId = bb.DocTypeId
            WHERE
                DraftOrderId = @DraftOrderId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DraftOrderId", key.DraftOrderId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<DraftOrderModel>(sql, dp);
    }

    public IEnumerable<DraftOrderModel> ListData(IUserKey filter)
    {
        const string sql = @"
            SELECT
                aa.DraftOrderId, aa.DraftOrderDate, aa.RequesterUserId, aa.DrafterUserId,
                aa.DocTypeId, aa.Context, aa.ContextReffId,
                ISNULL(bb.DocTypeName, '') AS DocTypeName
            FROM
                OFTA_DraftOrder aa
                LEFT JOIN OFTA_DocType bb ON aa.DocTypeId = bb.DocTypeId
            WHERE
                aa.RequesterUserId = @UserId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserId", filter.UserId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DraftOrderModel>(sql, dp);
    }

    public IEnumerable<DraftOrderModel> ListData(Periode filter)
    {
        const string sql = @"
            SELECT
                aa.DraftOrderId, aa.DraftOrderDate, aa.RequesterUserId, aa.DrafterUserId,
                aa.DocTypeId, aa.Context, aa.ContextReffId,
                ISNULL(bb.DocTypeName, '') AS DocTypeName
            FROM
                OFTA_DraftOrder aa
                LEFT JOIN OFTA_DocType bb ON aa.DocTypeId = bb.DocTypeId
            WHERE
                aa.DraftOrderDate BETWEEN @Tgl1 AND @Tgl2";
        
        var dp = new DynamicParameters();
        dp.AddParam("@Tgl1", filter.Tgl1, SqlDbType.DateTime);
        dp.AddParam("@Tgl2", filter.Tgl2, SqlDbType.DateTime);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DraftOrderModel>(sql, dp);
    }
}