using MySqlConnector;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class BaoCaoRepository
    {
        public decimal GetTongDoanhThu(int m, int y) {
            DataTable dt = DbManager.ExecuteQuery("SELECT SUM(so_tien) FROM hoc_phi WHERE thang=@m AND nam=@y AND trang_thai='da_thanh_toan'",
                new MySqlParameter[] { new MySqlParameter("@m", m), new MySqlParameter("@y", y) });
            return dt.Rows[0][0] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0][0]) : 0;
        }
        public DataTable GetChiTietDoanhThu(int thang, int nam)
        {
            string sql = @"
                SELECT h.ma_hoc_phi, t.ho_ten, h.so_tien, h.thang, h.nam, p.ma_phieu_thu, p.ngay_lap
                FROM hoc_phi h
                JOIN tre t ON h.tre_id = t.ma_tre
                JOIN phieu_thu p ON h.ma_hoc_phi = p.hoc_phi_id
                WHERE h.thang = @m AND h.nam = @y AND h.trang_thai = 'da_thanh_toan'";
            return DbManager.ExecuteQuery(sql, new MySqlParameter[] { 
                new MySqlParameter("@m", thang), 
                new MySqlParameter("@y", nam) 
            });
        }   
    }
}