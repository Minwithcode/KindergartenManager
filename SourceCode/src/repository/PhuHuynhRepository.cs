using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class PhuHuynhRepository
    {

        public bool InsertPhuHuynh(PhuHuynh ph)
        {
            string sql = @"INSERT INTO phu_huynh (ma_phu_huynh, nghe_nghiep, dia_chi, ghi_chu)
                           VALUES (@id,@nn,@dc,@gc)";
            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@id", ph.MaNguoiDung),
                new MySqlParameter("@nn", ph.NgheNghiep ?? ""),
                new MySqlParameter("@dc", ph.DiaChi ?? ""),
                new MySqlParameter("@gc", ph.GhiChu ?? "")
            }) > 0;
        }
        public bool UpdatePhuHuynh(PhuHuynh ph)
        {
            string sql = @"
        UPDATE phu_huynh
        SET nghe_nghiep = @nn,
            dia_chi = @dc,
            ghi_chu = @gc
        WHERE ma_phu_huynh = @ma";

            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
        new("@nn", ph.NgheNghiep),
        new("@dc", ph.DiaChi),
        new("@gc", ph.GhiChu),
        new("@ma", ph.MaNguoiDung)
            }) > 0;
        }

    }
}
