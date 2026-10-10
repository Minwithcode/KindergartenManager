using MySqlConnector;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class MienGiamRepository
    {


        public DataTable GetAllMienGiam() => DbManager.ExecuteQuery("SELECT * FROM mien_giam");

        public bool ThemMienGiam(MienGiam mg)
        {
            string sql = "INSERT INTO mien_giam (ma_mien_giam, ly_do, ty_le) VALUES (@id, @ld, @tl)";
            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[] {
                new MySqlParameter("@id", mg.MaMienGiam), new MySqlParameter("@ld", mg.LyDo), new MySqlParameter("@tl", mg.TyLe)
            }) > 0;
        }

        public bool SuaMienGiam(MienGiam mg)
        {
            string sql = "UPDATE mien_giam SET ly_do=@ld, ty_le=@tl WHERE ma_mien_giam=@id";
            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[] {
                new MySqlParameter("@ld", mg.LyDo), new MySqlParameter("@tl", mg.TyLe), new MySqlParameter("@id", mg.MaMienGiam)
            }) > 0;
        }

        public bool XoaMienGiam(string id) => DbManager.ExecuteNonQuery("DELETE FROM mien_giam WHERE ma_mien_giam=@id", new MySqlParameter[] { new MySqlParameter("@id", id) }) > 0;


        public DataTable GetDanhSachMienGiam()
        {
            string sql = "SELECT ma_mien_giam, ly_do, ty_le FROM mien_giam";
            return DbManager.ExecuteQuery(sql);
        }

    }
}