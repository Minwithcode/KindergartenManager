using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class TreRepository
    {
        public PhuHuynh GetPhuHuynhCuaTre(string maTre)
        {
            string sql = @"SELECT n.* FROM nguoi_dung n 
                   JOIN tre t ON n.ma_nguoi_dung = t.phu_huynh_id 
                   WHERE t.ma_tre = @maTre";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[] { new MySqlParameter("@maTre", maTre) });

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new PhuHuynh
                {
                    MaNguoiDung = row["ma_nguoi_dung"].ToString(),
                    HoTen = row["ho_ten"].ToString(),
                    Email = row["email"].ToString()
                };
            }
            return null;
        }

        public DataTable GetDanhSachTreTheoLop(string maLop)
        {
            string sql = @"SELECT t.* 
                           FROM tre t 
                           JOIN hoc_tai ht ON t.ma_tre = ht.ma_tre
                           WHERE ht.ma_lop_hoc = @maLop";

            return DbManager.ExecuteQuery(sql,
                new MySqlParameter[] { new MySqlParameter("@maLop", maLop) });
        }

        public DataTable SearchHocSinh(string maLop, string ten)
        {
            string sql = @"SELECT t.* 
                   FROM tre t 
                   JOIN hoc_tai ht ON t.ma_tre = ht.ma_tre
                   WHERE ht.ma_lop_hoc = @maLop
                   AND t.ho_ten COLLATE utf8mb4_unicode_ci 
                       LIKE @ten COLLATE utf8mb4_unicode_ci";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@maLop", maLop),
                new MySqlParameter("@ten", "%" + ten + "%")
            });
        }

        public DataTable GetThongTinPhuHuynhTheoLop(string maLop)
        {
            string sql = @"SELECT t.ho_ten AS ten_tre, n.ho_ten, n.so_dien_thoai
                   FROM tre t
                   JOIN hoc_tai ht ON t.ma_tre = ht.ma_tre
                   JOIN nguoi_dung n ON t.phu_huynh_id = n.ma_nguoi_dung
                   WHERE ht.ma_lop_hoc = @maLop";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[] {
                new MySqlParameter("@maLop", maLop)
            });
        }
        public DataRow? GetNhatKyTreTheoNgay(string maTre, string ngay)
        {
            string sql = @"SELECT * FROM cap_nhat_tt
                   WHERE tre_id= @maTre AND ngay_cap_nhat = @ngay";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]
                    {
        new MySqlParameter("@maTre", maTre),
        new MySqlParameter("@ngay", ngay)
                    });

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public bool InsertNhatKy(string maTre, string ngay, string anUong,
                                 string sucKhoe, string nhanXet, string maGV)
        {
            string sql = @"INSERT INTO cap_nhat_tt
                (ma_cap_nhat, tre_id, ngay_cap_nhat, an_uong, suc_khoe, nhan_xet, giao_vien_id)
                VALUES (@id,@tre,@ngay,@an,@sk,@nx,@gv)";

            DateTime d = DateTime.Parse(ngay);
            string id = "C"
            + maGV.Substring(maGV.Length - 2)
            + maTre.Substring(maTre.Length - 2)
            + d.ToString("ddMM");

            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@id", id),
                new MySqlParameter("@tre", maTre),
                new MySqlParameter("@ngay", ngay),
                new MySqlParameter("@an", anUong),
                new MySqlParameter("@sk", sucKhoe),
                new MySqlParameter("@nx", nhanXet),
                new MySqlParameter("@gv", maGV)
            }) > 0;
        }

        public DataTable GetGiaoVienPhuTrach(string maTre)
        {
            string sql = @"SELECT l.giao_vien_id
                           FROM hoc_tai ht
                           JOIN lop_hoc l ON ht.ma_lop_hoc = l.ma_lop_hoc
                           WHERE ht.ma_tre = @maTre
                           LIMIT 1";

            return DbManager.ExecuteQuery(
               sql,
               new MySqlParameter[] { new MySqlParameter("@maTre", maTre) }
           );

        }

        public DataTable GetThongTinSinhHoat(string maPH)
        {
            string sql = @"
        SELECT 
            t.ho_ten        AS ten_tre,
            c.ngay_cap_nhat,
            c.an_uong,
            c.suc_khoe,
            c.nhan_xet,
            nd.ho_ten       AS giao_vien
        FROM cap_nhat_tt c
        JOIN tre t ON c.tre_id = t.ma_tre
        JOIN nguoi_dung nd ON c.giao_vien_id = nd.ma_nguoi_dung
        WHERE t.phu_huynh_id = @maPH
        ORDER BY c.ngay_cap_nhat DESC;";

            return DbManager.ExecuteQuery(sql,
                new MySqlParameter[] { new MySqlParameter("@maPH", maPH) });
        }
        public DataTable GetThongTinTre(string maPH)
        {
            string sql = @"SELECT 
    t.ma_tre,
    t.ho_ten,
    t.ngay_sinh,
    t.gioi_tinh,
    IFNULL(l.ten_lop, 'Chưa xếp lớp') AS ten_lop,
    IFNULL(nd.ho_ten, 'Chưa phân công') AS co_giao
FROM tre t
LEFT JOIN hoc_tai ht ON t.ma_tre = ht.ma_tre
LEFT JOIN lop_hoc l ON ht.ma_lop_hoc = l.ma_lop_hoc
LEFT JOIN nguoi_dung nd ON l.giao_vien_id = nd.ma_nguoi_dung
WHERE t.phu_huynh_id = @maPH";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[] {
        new MySqlParameter("@maPH", maPH)
    });
        }



        public DataTable GetDanhSachTre(string maPH)
        {
            string sql = @"SELECT ma_tre, ho_ten
                   FROM tre
                   WHERE phu_huynh_id = @maPH
                         AND trang_thai = 'dang_theo_hoc'";
            return DbManager.ExecuteQuery(sql,
                new MySqlParameter[] { new MySqlParameter("@maPH", maPH) });
        }

        public bool InsertTre(Tre tre)
        {
            string sql = @"INSERT INTO tre 
                           (ma_tre, ho_ten, ngay_sinh, gioi_tinh, phu_huynh_id, tinh_trang, trang_thai)
                           VALUES (@id,@ten,@ns,@gt,@ph,@tt,@trangthai)";
            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@id", tre.MaTre),
                new MySqlParameter("@ten", tre.HoTen),
                new MySqlParameter("@ns", tre.NgaySinh),
                new MySqlParameter("@gt", tre.GioiTinh.ToString()),
                new MySqlParameter("@ph", tre.PhuHuynhId),
                new MySqlParameter("@tt", tre.TinhTrang ?? ""),
                new MySqlParameter("@trangthai", tre.TrangThai.ToString())
            }) > 0;
        }
        public bool TreDaCoLop(string maTre, string namHoc)
        {
            string sql = @"
                        SELECT 1
                        FROM hoc_tai
                        WHERE ma_tre = @maTre AND nam_hoc = @namHoc
                        LIMIT 1";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@maTre", maTre),
                new MySqlParameter("@namHoc", namHoc)
            });

            return dt.Rows.Count > 0;
        }
        public int GetTuoiTre(string maTre)
        {
            string sql = @"SELECT ngay_sinh FROM tre WHERE ma_tre = @maTre";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@maTre", maTre)
            });

            DateTime ns = Convert.ToDateTime(dt.Rows[0]["ngay_sinh"]);
            return DateTime.Now.Year - ns.Year;
        }

        public string GetPhuHuynhIdByTre(string maTre)
        {
            string sql = "SELECT phu_huynh_id FROM tre WHERE ma_tre = @ma";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@ma", maTre)
            });

            return dt.Rows.Count > 0 ? dt.Rows[0][0].ToString()! : "";
        }

        public DataTable GetTreByMa(string maTre)
        {
            string sql = @"SELECT ma_tre, ho_ten, ngay_sinh, tinh_trang, gioi_tinh,
                            trang_thai
                        FROM tre
                        WHERE ma_tre = @ma";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@ma", maTre)
            });
        }

        public void UpdateTre(string maTre, string hoTen, string tinh_trang,
                              DateTime ngaySinh, string gioiTinh, string trang_thai
                              )
        {
            string sql = @"UPDATE tre
                            SET ho_ten = @ten,
                                ngay_sinh = @ns,
                                tinh_trang = @tt,
                                gioi_tinh = @gt,
                                trang_thai = @trthai
                            WHERE ma_tre = @ma";

            DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@ten", hoTen),
                new MySqlParameter("@ns", ngaySinh),
                new MySqlParameter("@tt", tinh_trang),

                new MySqlParameter("@gt", gioiTinh),
                new MySqlParameter("@ma", maTre),
                new MySqlParameter("@trthai", trang_thai)
            });
        }


    }
}
