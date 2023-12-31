using System.Data.SqlClient;
using Microsoft.Extensions.Options;
using Ners.Application.Helpers;

namespace Ners.Infrastructure.Helpers;

public class TglJamDal : ITglJamDal
{
    private readonly DatabaseOptions _opt;

    public TglJamDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public DateTime Now 
    { 
        get 
        {
            const string sql = @"SELECT GETDATE() TglJam";
            using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
            conn.Open();
            
            using var cmd = new SqlCommand(sql, conn);
            using var dr = cmd.ExecuteReader();
            dr.Read();
            var result = Convert.ToDateTime(dr["TglJam"]);

            return result;
        }
    }
}