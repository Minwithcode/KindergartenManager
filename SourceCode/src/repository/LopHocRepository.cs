using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class LopHocRepository
    {
        public DataTable GetGiaoVienChuaCoLop(string namHoc)
        {
            string sql = @"
                SELECT gv.ma_giao_vien, nd.ho_ten
                FROM giao_vien gv
                JOIN nguoi_dung nd ON gv.ma_giao_vien = nd.ma_nguoi_dung
                WHERE gv.ma_giao_vien NOT IN (
                    SELECT giao_vien_id FROM lop_hoc WHERE nam_hoc = @nh
                )";

            return DbManager.ExecuteQuery(sql,
               new MySqlParameter[] { new MySqlParameter("@nh", namHoc) });
        }
        public bool InsertLopHoc(Model.LopHoc lop)
        {
            string sql = @"INSERT INTO lop_hoc
                (ma_lop_hoc, ten_lop, giao_vien_id, nam_hoc, ghi_chu)
                VALUES (@ma, @ten, @gv, @nh, @gc)";

            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@ma", lop.MaLopHoc),
                new MySqlParameter("@ten", lop.TenLop),
                new MySqlParameter("@gv", lop.GiaoVienId),
                new MySqlParameter("@nh", lop.NamHoc),
                new MySqlParameter("@gc", lop.GhiChu ?? "")
            }) > 0;
        }

        public DataTable GetTreChuaCoLop(string namHoc)
        {
            string sql = @"
                SELECT * FROM tre
                WHERE ma_tre NOT IN (
                    SELECT ma_tre FROM hoc_tai WHERE nam_hoc = @nh
                ) AND trang_thai = 'dang_theo_hoc'";

            return DbManager.ExecuteQuery(sql,
                new MySqlParameter[] { new MySqlParameter("@nh", namHoc) });
        }

        public bool InsertHocTai(string maTre, string maLop, string namHoc)
        {
            string sql = @"INSERT INTO hoc_tai (ma_tre, ma_lop_hoc, nam_hoc)
                           VALUES (@t, @l, @n)";

            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@t", maTre),
                new MySqlParameter("@l", maLop),
                new MySqlParameter("@n", namHoc)
            }) > 0;
        }

        public bool UpdateTenLop(string maLop, string tenMoi)
        {
            return DbManager.ExecuteNonQuery(
                "UPDATE lop_hoc SET ten_lop=@t WHERE ma_lop_hoc=@m", new MySqlParameter[]
            {
                new MySqlParameter("@t", tenMoi),
                new MySqlParameter("@m", maLop)
            }
            ) > 0;
        }

        public bool UpdateGiaoVien(string maLop, string maGV)
        {
            return DbManager.ExecuteNonQuery(
                "UPDATE lop_hoc SET giao_vien_id=@gv WHERE ma_lop_hoc=@l", new MySqlParameter[]
            {
                new MySqlParameter("@gv", maGV),
                new MySqlParameter("@l", maLop)
            }
            ) > 0;
        }

        public DataTable GetTreChuaCoLopCoTuoi(string namHoc)
        {
            string sql = @"
        SELECT 
            t.ma_tre,
            t.ho_ten,
            TIMESTAMPDIFF(YEAR, t.ngay_sinh, CURDATE()) AS tuoi
        FROM tre t
        WHERE NOT EXISTS (
            SELECT 1 
            FROM hoc_tai ht
            WHERE ht.ma_tre = t.ma_tre
              AND ht.nam_hoc = @namHoc
        )
        ORDER BY tuoi";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@namHoc", namHoc)
            });
        }
        public int GetSoLuongTreTrongLop(string maLop, string namHoc)
        {
            string sql = @"
        SELECT COUNT(*) AS so_luong
        FROM hoc_tai
        WHERE ma_lop = @lop AND nam_hoc = @namHoc";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@lop", maLop),
        new MySqlParameter("@namHoc", namHoc)
            });

            return Convert.ToInt32(dt.Rows[0]["so_luong"]);
        }
        public bool IsLopHocExists(string maLop)
        {
            string sql = "SELECT ma_lop_hoc FROM lop_hoc WHERE ma_lop_hoc = @ma";
            return DbManager.ExecuteQuery(sql, new MySqlParameter[]{
        new MySqlParameter("@ma", maLop)}
                ).Rows.Count > 0;
        }
        public DataTable GetLopChuaDuTre(string namHoc)
        {
            string sql = @"
        SELECT 
            lh.ma_lop_hoc,
            lh.ten_lop,
            nd.ho_ten AS ten_giao_vien,
            COUNT(ht.ma_tre) AS si_so
        FROM lop_hoc lh
        JOIN giao_vien gv ON lh.giao_vien_id = gv.ma_giao_vien
        LEFT JOIN hoc_tai ht 
            ON lh.ma_lop_hoc = ht.ma_lop_hoc
            AND ht.nam_hoc = lh.nam_hoc
        JOIN nguoi_dung nd ON nd.ma_nguoi_dung = gv.ma_giao_vien
        WHERE lh.nam_hoc = @namHoc
        GROUP BY lh.ma_lop_hoc, lh.ten_lop, nd.ho_ten
        HAVING COUNT(ht.ma_tre) < 10";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@namHoc", namHoc)
            });
        }

        public (int min, int max) GetDoTuoiLop(string maLop)
        {
            string sql = @"SELECT ten_lop FROM lop_hoc WHERE ma_lop_hoc = @maLop";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@maLop", maLop)
            });

            string tenLop = dt.Rows[0]["ten_lop"].ToString()!.ToLower();

            if (tenLop.Contains("mầm")) return (3, 4);
            if (tenLop.Contains("chồi")) return (4, 5);
            if (tenLop.Contains("lá")) return (5, 6);

            return (0, 100); 
        }

        public int GetSiSoLop(string maLop, string namHoc)
        {
            string sql = @"
        SELECT COUNT(*) AS si_so
        FROM hoc_tai
        WHERE ma_lop_hoc = @maLop AND nam_hoc = @namHoc";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@maLop", maLop),
        new MySqlParameter("@namHoc", namHoc)
            });

            return Convert.ToInt32(dt.Rows[0]["si_so"]);
        }

        public DataTable GetLopTheoNamHoc(string namHoc)
        {
            string sql = @"
        SELECT ma_lop_hoc, ten_lop
        FROM lop_hoc
        WHERE nam_hoc = @namHoc";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@namHoc", namHoc)
            });
        }

        public string GetNamHocByMaLop(string maLop)
        {
            string sql = @"SELECT nam_hoc FROM lop_hoc WHERE ma_lop_hoc = @ma";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@ma", maLop)
            });

            return dt.Rows.Count > 0 ? dt.Rows[0]["nam_hoc"].ToString()! : "";
        }

        public void UpdateTenVaNamHoc(string maLop, string tenMoi, string namHocMoi)
        {
            string sql = @"UPDATE lop_hoc
                            SET ten_lop = @ten, nam_hoc = @nam
                            WHERE ma_lop_hoc = @ma";

            DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@ten", tenMoi),
                new MySqlParameter("@nam", namHocMoi),
                new MySqlParameter("@ma", maLop)
            });
        }

    }

}