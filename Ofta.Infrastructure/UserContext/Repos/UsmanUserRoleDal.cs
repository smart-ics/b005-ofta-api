using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Infrastructure.Helpers;
using Usman.Lib.NetStandard.Interfaces;
using Usman.Lib.NetStandard.Models;

namespace Ofta.Infrastructure.UserContext.Repos;

public class UsmanUserRoleDal : IUsmanUserRoleDal
{
    private readonly DatabaseOptions _opt;

    public UsmanUserRoleDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(UsmanUserRoleModel model)
    {
        const string sql = @"
            INSERT INTO 
                USMAN_UserRole (
                    UserId, Role)
            VALUES (@UserId, @Role)";

        var dp = new DynamicParameters();
        dp.AddParam("@UserId", model.UserId, SqlDbType.UniqueIdentifier);
        dp.AddParam("@Role", model.Role, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IUserKey key)
    {
        const string sql = @"
            DELETE 
                USMAN_UserRole 
            WHERE
                UserId = @UserId";

        var dp = new DynamicParameters();
        dp.AddParam("@UserId", key.UserId, SqlDbType.UniqueIdentifier);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<UsmanUserRoleModel> ListData(IUserKey key)
    {
        const string sql = @"
            SELECT 
                UserId, Role
            FROM
                USMAN_UserRole
            WHERE
                UserId = @UserId ";

        var dp = new DynamicParameters();
        dp.AddParam("@UserId", key.UserId, SqlDbType.UniqueIdentifier);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<UsmanUserRoleModel>(sql, dp);
    }
}