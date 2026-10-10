using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class KeToanRepository
    {

        public bool InsertKeToan(KeToan kt)

        {

            string sql = @"INSERT INTO ke_toan (ma_ke_toan, ma_chung_chi_hanh_nghe, phong_ban)

                           VALUES (@id,@cc,@pb)";

            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]

            {

                new MySqlParameter("@id", kt.MaNguoiDung),

                new MySqlParameter("@cc", kt.MaChungChiHanhNghe ?? ""),

                new MySqlParameter("@pb", kt.PhongBan ?? "")

            }) > 0;

        }

        public string GetLastId(string tableName, string columnId, string prefix)
        {
            string sql = $"SELECT {columnId} FROM {tableName} WHERE {columnId} LIKE '{prefix}%' ORDER BY {columnId} DESC LIMIT 1";
            DataTable dt = DbManager.ExecuteQuery(sql);
            return dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : null;
        }
    }
}