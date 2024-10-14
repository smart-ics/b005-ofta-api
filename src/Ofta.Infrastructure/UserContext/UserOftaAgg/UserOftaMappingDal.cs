using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.UserContext.UserOftaAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.UserContext.UserOftaAgg;

public class UserOftaMappingDal: IUserOftaMappingDal
{
    private readonly DatabaseOptions _opt;

    public UserOftaMappingDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<UserOftaMappingModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        
        conn.Open();
        bcp.AddMap("UserOftaId", "UserOftaId");
        bcp.AddMap("UserMappingId", "UserMappingId");
        bcp.AddMap("PegId", "PegId");
        bcp.AddMap("UserType", "UserType");

        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_UserMapping";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(IUserOftaKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_UserMapping
            WHERE
                UserOftaId = @UserOftaId";

        var dp = new DynamicParameters();
        dp.AddParam("@UserOftaId", key.UserOftaId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<UserOftaMappingModel> ListData(IUserOftaKey filter)
    {
        const string sql = @"
            SELECT
                UserOftaId, UserMappingId, PegId, UserType
            FROM 
                OFTA_UserMapping
            WHERE
                UserOftaId = @UserOftaId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserOftaId", filter.UserOftaId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<UserOftaMappingDto>(sql, dp);
    }

    public IEnumerable<UserOftaMappingModel> ListData(string filter)
    {
        const string sql = @"
            SELECT
                UserOftaId, UserMappingId, PegId, UserType
            FROM 
                OFTA_UserMapping
            WHERE
                UserMappingId = @UserMappingId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserMappingId", filter, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<UserOftaMappingDto>(sql, dp);
    }
}

public class UserOftaMappingDto : UserOftaMappingModel
{
    public UserOftaMappingDto(string userOftaId, string userMappingId, string pegId, UserTypeEnum userType) : base(userOftaId, userMappingId, pegId, userType)
    {
        UserOftaId = userOftaId;
        UserMappingId = userMappingId;
        PegId = pegId;
        UserType = userType;
    }
}