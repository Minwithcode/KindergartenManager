using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class GiaoVienRepository
    {

        public bool InsertGiaoVien(GiaoVien gv)
        {
            string sql = @"INSERT INTO giao_vien (ma_giao_vien, trinh_do, chuyen_mon, ngay_vao_lam, ghi_chu)
                           VALUES (@id,@td,@cm,@ngay,@gc)";
            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@id", gv.MaNguoiDung),
                new MySqlParameter("@td", gv.TrinhDo ?? ""),
                new MySqlParameter("@cm", gv.ChuyenMon ?? ""),
                new MySqlParameter("@ngay", gv.NgayVaoLam),
                new MySqlParameter("@gc", gv.GhiChu ?? "")
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



        public bool IsGiaoVienExists(string maGV)
        {
            string sql = "SELECT ma_giao_vien FROM giao_vien WHERE ma_giao_vien = @ma";
            return DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@ma", maGV)
            }).Rows.Count > 0;
        }

        public bool UpdateGiaoVien(GiaoVien gv)
        {
            string sql = @"
                        UPDATE giao_vien
                        SET 
                            trinh_do = @td,
                            chuyen_mon = @cm,
                            ngay_vao_lam = @nvl,
                            ghi_chu = @gc
                        WHERE ma_giao_vien = @ma";

            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
        new("@td", gv.TrinhDo),
        new("@cm", gv.ChuyenMon),
        new("@nvl", gv.NgayVaoLam),
        new("@gc", gv.GhiChu),
        new("@ma", gv.MaNguoiDung)
            }) > 0;
        }

    }
}
