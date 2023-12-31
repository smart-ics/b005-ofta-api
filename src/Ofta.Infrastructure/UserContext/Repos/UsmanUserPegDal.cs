using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Infrastructure.Helpers;
using Usman.Lib.NetStandard.Interfaces;
using Usman.Lib.NetStandard.Models;

namespace Ofta.Infrastructure.UserContext.Repos
{
    public class UsmanPegDal : IUsmanPegDal
    {
        private readonly DatabaseOptions _opt;

        public UsmanPegDal(IOptions<DatabaseOptions> opt)
        {
            _opt = opt.Value;
        }

        public UsmanPegModel GetData(IEmail email)
        {
            const string sql = @"
                SELECT
                    aa.fs_kd_peg AS Nik, aa.fs_nm_peg AS PegName, aa.fd_tgl_lahir AS BirtDate,
                    aa.fs_kd_bagian AS BagianId, aa.fs_kd_departemen AS DepartementId,
                    aa.fs_kd_jabatan AS JabatanId, aa.fs_kd_golongan AS GolonganId,
                    ISNULL(bb.fs_nm_bagian, '') AS BagianName,
                    ISNULL(cc.fs_nm_departemen, '') AS DepartemenName,
                    ISNULL(dd.fs_nm_golongan, '') AS GolonganName,
                    ISNULL(ee.fs_nm_jabatan, '') AS JabatanName
                FROM
                    td_peg aa
                    LEFT JOIN td_bagian bb ON aa.fs_kd_bagian = bb.fs_kd_bagian
                    LEFT JOIN td_departemen cc ON aa.fs_kd_departemen = cc.fs_kd_departemen
                    LEFT JOIN td_golongan dd ON aa.fs_kd_golongan = dd.fs_kd_golongan
                    LEFT JOIN td_jabatan ee ON aa.fs_kd_jabatan = ee.fs_kd_jabatan 
                WHERE
                    aa.fs_email = @fs_email ";

            var dp = new DynamicParameters();
            dp.AddParam("@fs_email", email, SqlDbType.VarChar);

            using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
            return conn.ReadSingle<UsmanPegModel>(sql, dp);
        }
    }
}
