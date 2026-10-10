using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class ThongBaoRepository{
        public static void ThemThongBao(ThongBao tb)
        {
            string sql = @"INSERT INTO thong_bao
                   (nguoi_gui_id, nguoi_nhan_id, loai, tieu_de, noi_dung, thoi_gian, trang_thai)
                   VALUES (@nguoi_gui_id, @nguoi_nhan_id, @loai, @tieu_de, @noi_dung, @thoi_gian, @trang_thai)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@nguoi_gui_id", tb.NguoiGuiId),
                new MySqlParameter("@nguoi_nhan_id", tb.NguoiNhanId),
                new MySqlParameter("@loai", tb.Loai),
                new MySqlParameter("@tieu_de", tb.TieuDe),
                new MySqlParameter("@noi_dung", tb.NoiDung),
                new MySqlParameter("@thoi_gian", tb.ThoiGian),
                new MySqlParameter("@trang_thai", tb.TrangThai)
            };

            DbManager.ExecuteNonQuery(sql, parameters); 
        }

        public DataTable GetThongBaoDaNhan(string maNguoiNhan)
        {
            string sql = @"SELECT tb.*, n.ho_ten AS ten_nguoi_lien_quan
                           FROM thong_bao tb
                           JOIN nguoi_dung n ON tb.nguoi_gui_id = n.ma_nguoi_dung
                           WHERE tb.nguoi_nhan_id = @id
                           ORDER BY tb.thoi_gian DESC";

            return DbManager.ExecuteQuery(sql,
                new MySqlParameter[] { new MySqlParameter("@id", maNguoiNhan) });
        }

        public DataTable GetThongBaoDaGui(string maNguoiGui)
        {
            string sql = @"SELECT tb.*, n.ho_ten AS ten_nguoi_lien_quan
                           FROM thong_bao tb
                           JOIN nguoi_dung n ON tb.nguoi_nhan_id = n.ma_nguoi_dung
                           WHERE tb.nguoi_gui_id = @id
                           ORDER BY tb.thoi_gian DESC";

            return DbManager.ExecuteQuery(sql,
                new MySqlParameter[] { new MySqlParameter("@id", maNguoiGui) });
        }
        public static bool CapNhatTrangThaiThongBao(int maThongBao, string trangThaiMoi = "da_xem")
        {
            string sql = @"UPDATE thong_bao
                   SET trang_thai = @trangThai
                   WHERE ma_thong_bao = @id";

            MySqlParameter[] parameters = {
                new MySqlParameter("@trangThai", trangThaiMoi),
                new MySqlParameter("@id", maThongBao)
            };

            return DbManager.ExecuteNonQuery(sql, parameters) > 0;
        }
    }

}