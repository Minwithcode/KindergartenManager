using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class DiemDanhRepository
    {
        public DataTable GetTreChuaDiemDanh(string maLop, string ngay)
        {
            string sql = @"SELECT t.ma_tre, t.ho_ten 
                           FROM tre t 
                           JOIN hoc_tai ht ON t.ma_tre = ht.ma_tre
                           WHERE ht.ma_lop_hoc = @maLop 
                           AND t.ma_tre NOT IN 
                               (SELECT tre_dd FROM diem_danh WHERE ngay = @ngay)";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@maLop", maLop),
                new MySqlParameter("@ngay", ngay)
            });
        }

        public bool KiemTraHoanThanhDiemDanh(string maLop, string ngay)
        {
            string sql = @"SELECT COUNT(*) 
                           FROM tre t 
                           JOIN hoc_tai ht ON t.ma_tre = ht.ma_tre
                           WHERE ht.ma_lop_hoc = @maLop 
                           AND t.ma_tre NOT IN 
                               (SELECT tre_dd FROM diem_danh WHERE ngay = @ngay)";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@maLop", maLop),
                new MySqlParameter("@ngay", ngay)
            });

            return Convert.ToInt32(dt.Rows[0][0]) == 0;
        }

        public bool LuuDiemDanh(string maTre, string trangThai, string ngay, string maGV)
        {
            string sql = @"INSERT INTO diem_danh
                (ma_diem_danh, tre_dd, ngay, trang_thai, giao_vien_dd)
                VALUES (@id, @tre, @ngay, @st, @gv)";
            DateTime d = DateTime.Parse(ngay);
            string maDD = "D" + maGV.Substring(maGV.Length - 2)   
                        + maTre.Substring(maTre.Length - 2)   
                        + d.ToString("ddMM");               

            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@id", maDD),
                new MySqlParameter("@tre", maTre),
                new MySqlParameter("@ngay", ngay),
                new MySqlParameter("@st", trangThai.ToString().ToLower()),
                new MySqlParameter("@gv", maGV)
            }) > 0;
        }
         public DataTable GetThongKeDiemDanh(string ngay)
        {
            string sql = @"SELECT * FROM diem_danh WHERE ngay = @ngay";
            return DbManager.ExecuteQuery(sql, new MySqlParameter[] {
                new MySqlParameter("@ngay", ngay)
            });
        }
        public DataTable GetBaoCaoDiemDanhTheoNgay(string maLop, string ngay)
        {
            string sql = @"SELECT t.ho_ten, t.ma_tre, dd.trang_thai
                   FROM diem_danh dd
                   JOIN tre t ON dd.tre_dd = t.ma_tre
                   JOIN hoc_tai ht ON t.ma_tre = ht.ma_tre
                   WHERE ht.ma_lop_hoc = @maLop
                   AND dd.ngay = @ngay";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@maLop", maLop),
        new MySqlParameter("@ngay", ngay)
            });
        }       
    }
}
