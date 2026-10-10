using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class HocPhiRepository
    {
        public bool InsertHocPhi(HocPhi hp, string maKeToan)
        {
            string sql = @"INSERT INTO hoc_phi 
        (ma_hoc_phi, tre_id, thang, nam, so_tien, mien_giam_id, nguoi_gui, trang_thai)
        VALUES 
        (@ma, @tre, @thang, @nam, @tien, @mg, @kt, 'chua_thanh_toan')";

            MySqlParameter[] p =
            {
        new MySqlParameter("@ma", hp.MaHocPhi),
        new MySqlParameter("@tre", hp.TreId),
        new MySqlParameter("@thang", hp.Thang),
        new MySqlParameter("@nam", hp.Nam),
        new MySqlParameter("@tien", hp.SoTien),
        new MySqlParameter("@mg", (object?)hp.MienGiamId ?? DBNull.Value),
        new MySqlParameter("@kt", maKeToan)
    };

            return DbManager.ExecuteNonQuery(sql, p) > 0;
        }


        
        public string GetPhuHuynhIdTuHocPhi(string maHocPhi)
        {
            string sql = @"SELECT t.phu_huynh_id
                           FROM hoc_phi hp
                           JOIN tre t ON hp.tre_id = t.ma_tre
                           WHERE hp.ma_hoc_phi = @ma";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]{
                new MySqlParameter("@ma", maHocPhi)}
            );
            if (dt.Rows.Count == 0)
                return null;

            return dt.Rows[0]["phu_huynh_id"].ToString();
        }

        public DataTable GetHocPhiUuTienChuaDong()
        {
            string sql = @"SELECT h.*, t.ho_ten AS ten_tre FROM hoc_phi h 
                           JOIN tre t ON h.tre_id = t.ma_tre 
                           ORDER BY CASE WHEN h.trang_thai = 'chua_thanh_toan' THEN 0 ELSE 1 END, h.nam DESC";
            return DbManager.ExecuteQuery(sql);
        }

        
        public DataTable FindHocPhi(string keyword)
        {
            string sql = @"SELECT h.*, t.ho_ten AS ten_tre FROM hoc_phi h 
                           JOIN tre t ON h.tre_id = t.ma_tre 
                           WHERE h.ma_hoc_phi LIKE @key OR t.ho_ten LIKE @key";
            return DbManager.ExecuteQuery(sql, new MySqlParameter[] { new MySqlParameter("@key", "%" + keyword + "%") });
        }

        
        public bool UpdateTrangThaiHocPhi(string maHP, string trangThai, string maKeToan)
        {
            string sql = "UPDATE hoc_phi SET trang_thai = @tt, nguoi_gui = @kt WHERE ma_hoc_phi = @id";
            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[] {
                new MySqlParameter("@tt", trangThai),
                new MySqlParameter("@kt", maKeToan),
                new MySqlParameter("@id", maHP)
            }) > 0;
        }

        public bool InsertPhieuThu(PhieuThu pt)
        {
            string sql = "INSERT INTO phieu_thu (ma_phieu_thu, hoc_phi_id, ghi_chu, lap_boi, ngay_lap) VALUES (@id, @hpid, @gc, @kt, NOW())";
            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[] {
                new MySqlParameter("@id", pt.MaPhieuThu), new MySqlParameter("@hpid", pt.HocPhiId),
                new MySqlParameter("@gc", pt.GhiChu), new MySqlParameter("@kt", pt.LapBoi)
            }) > 0;
        }
    }
}