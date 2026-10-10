using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class DonXinNghiRepository
    {
       public bool InsertDon(DonXinNghi don)
        {
            string sql = @"INSERT INTO don_xin_nghi
        (ma_don, tre_id, ngay_bat_dau, ngay_ket_thuc, ly_do, trang_thai, ngay_gui)
        VALUES (@id, @tre, @ngaybd, @ngaykt, @lydo, @tt, @ngayg)";

            try
            {
                return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
                {
            new("@id", don.MaDon),
            new("@tre", don.TreId),
            new("@ngaybd", don.NgayBatDau),
            new("@ngaykt", don.NgayKetThuc),
            new("@lydo", don.LyDo),
            new("@tt", don.TrangThai),
            new("@ngayg", don.NgayGui)
                }) > 0;
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Console.WriteLine("Mã đơn bị trùng, vui lòng thử lại.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi InsertDon: " + ex.Message);
                return false;
            }
        }
        public DataTable GetDonDaGui(string maPH)
        {
            string sql = @"SELECT d.ma_don,
                          t.ho_ten      AS ten_tre,
                          d.ngay_bat_dau,
                          d.ngay_ket_thuc,
                          d.ly_do,
                          d.trang_thai,
                          d.ngay_gui,
                          d.ngay_duyet,
                          nd.ho_ten     AS giao_vien_duyet
                   FROM don_xin_nghi d
                   JOIN tre t ON d.tre_id = t.ma_tre
                   LEFT JOIN nguoi_dung nd ON d.duyet_boi = nd.ma_nguoi_dung
                   WHERE t.phu_huynh_id = @maPH
                   ORDER BY d.ngay_gui DESC";

            return DbManager.ExecuteQuery(sql,
                        new MySqlParameter[] { new MySqlParameter("@maPH", maPH) });
        }
        public DataTable GetDonXinNghiChoDuyet(string maLop)
        {
            string sql = @"SELECT d.*, t.ho_ten
                           FROM don_xin_nghi d
                           JOIN tre t ON d.tre_id = t.ma_tre
                           JOIN hoc_tai ht ON t.ma_tre = ht.ma_tre
                           WHERE ht.ma_lop_hoc = @maLop
                           AND d.trang_thai = @st";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@maLop", maLop),
                new MySqlParameter("@st", TrangThaiDonNghi.cho_duyet.ToString())
            });
        }

        public bool UpdateTrangThaiDon(string maDon, TrangThaiDonNghi trangThai, string maGV)
        {
            string sql = @"UPDATE don_xin_nghi
                           SET trang_thai = @st,
                               ngay_duyet = NOW(),
                               duyet_boi = @gv
                           WHERE ma_don = @id";

            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@st", trangThai.ToString()),
                new MySqlParameter("@gv", maGV),
                new MySqlParameter("@id", maDon)
            }) > 0;
        }
        public string GetMaLopCuaGiaoVien(string maGV)
        {
            string sql = "SELECT ma_lop_hoc FROM lop_hoc WHERE giao_vien_id = @gv";
            DataTable dt = DbManager.ExecuteQuery(sql,
                new MySqlParameter[]
                {
                    new MySqlParameter("@gv", maGV)
                }
            );
            return dt.Rows.Count > 0 ? dt.Rows[0][0].ToString()! : "";
        }
    }
}
