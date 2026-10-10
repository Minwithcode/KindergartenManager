using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public static class DbManager
    {
        private static string bieu_thuc_ket_noi = "Server=localhost;Database=quan_ly_nha_tre;User=root;Password=;Port=3306;" + "CharSet=utf8mb4;";
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(bieu_thuc_ket_noi);
        }


        public static DataTable ExecuteQuery(string query, MySqlParameter[]? parameters = null)
        {
            DataTable data = new DataTable();
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (parameters != null) command.Parameters.AddRange(parameters);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(data);
                    }
                }
            }
            return data;
        }

        public static int ExecuteNonQuery(string query, MySqlParameter[]? parameters = null)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (parameters != null) command.Parameters.AddRange(parameters);
                    return command.ExecuteNonQuery();
                }
            }
        }
    }

    public class DataRepository
    {
        public NguoiDung? KiemTraDangNhap(string username, string password)
        {

            string sql = @"
                        SELECT n.*, 
                            q.ma_quan_tri,
                            g.trinh_do, g.chuyen_mon, g.ngay_vao_lam, g.ghi_chu AS ghi_chu_gv,
                            p.nghe_nghiep, p.dia_chi, p.ghi_chu AS ghi_chu_ph,
                            k.ma_chung_chi_hanh_nghe, k.phong_ban
                        FROM nguoi_dung n
                        LEFT JOIN quan_tri_vien q ON n.ma_nguoi_dung = q.ma_quan_tri
                        LEFT JOIN giao_vien g ON n.ma_nguoi_dung = g.ma_giao_vien
                        LEFT JOIN phu_huynh p ON n.ma_nguoi_dung = p.ma_phu_huynh
                        LEFT JOIN ke_toan k ON n.ma_nguoi_dung = k.ma_ke_toan
                        WHERE n.ten_dang_nhap = @user AND n.mat_khau = @pass ";

            MySqlParameter[] parameters = {
        new MySqlParameter("@user", username),
        new MySqlParameter("@pass", password)};
       

            DataTable dt = DbManager.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                NguoiDung user;


                if (row["ma_quan_tri"] != DBNull.Value)
                {
                    user = new QuanTriVien();
                }
                else if (row["trinh_do"] != DBNull.Value)
                {
                    user = new GiaoVien
                    {
                        TrinhDo = row["trinh_do"].ToString() ?? "",
                        ChuyenMon = row["chuyen_mon"].ToString() ?? "",
                        NgayVaoLam = row["ngay_vao_lam"] != DBNull.Value ? Convert.ToDateTime(row["ngay_vao_lam"]) : null,
                        GhiChu = row["ghi_chu_gv"].ToString() ?? ""
                    };
                }
                else if (row["nghe_nghiep"] != DBNull.Value)
                {
                    user = new PhuHuynh
                    {
                        NgheNghiep = row["nghe_nghiep"].ToString() ?? "",
                        DiaChi = row["dia_chi"].ToString() ?? "",
                        GhiChu = row["ghi_chu_ph"].ToString() ?? ""
                    };
                }
                else if (row["ma_chung_chi_hanh_nghe"] != DBNull.Value)
                {
                    user = new KeToan
                    {
                        MaChungChiHanhNghe = row["ma_chung_chi_hanh_nghe"].ToString() ?? "",
                        PhongBan = row["phong_ban"].ToString() ?? ""
                    };
                }
                else
                {
                    throw new Exception("Tài khoản không có thông tin vai trò hợp lệ trong hệ thống.");
                }


                user.MaNguoiDung = row["ma_nguoi_dung"].ToString() ?? "";
                user.HoTen = row["ho_ten"].ToString() ?? "";
                user.NgaySinh = Convert.ToDateTime(row["ngay_sinh"]);
                user.TenDangNhap = row["ten_dang_nhap"].ToString() ?? "";
                user.MatKhau = row["mat_khau"].ToString() ?? "";
                user.Email = row["email"].ToString() ?? "";
                user.SoDienThoai = row["so_dien_thoai"].ToString() ?? "";
                user.TrangThai = row["trang_thai"].ToString() ?? "";
                user.NgayTao = Convert.ToDateTime(row["ngay_tao"]);
                user.NgayCapNhat = Convert.ToDateTime(row["ngay_cap_nhat"]);

                return user;
            }
            return null;
        }


        public bool DoiMatKhau(string maNguoiDung, string matKhauMoi)
        {
            string sql = "UPDATE nguoi_dung SET mat_khau = @pass, ngay_cap_nhat = CURRENT_TIMESTAMP WHERE ma_nguoi_dung = @id";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@pass", matKhauMoi),
                new MySqlParameter("@id", maNguoiDung)
            };

            int result = DbManager.ExecuteNonQuery(sql, parameters);
            return result > 0;
        }

        private NguoiDung CreateUserInstance(string maNguoiDung)
        {
            if (maNguoiDung.StartsWith("QTV")) return new QuanTriVien();
            if (maNguoiDung.StartsWith("GV")) return new GiaoVien();
            if (maNguoiDung.StartsWith("PH")) return new PhuHuynh();
            if (maNguoiDung.StartsWith("KT")) return new KeToan();

            throw new Exception("Mã người dùng không hợp lệ hoặc không xác định được vai trò.");
        }

        public bool Insert(NguoiDung user)
        {
            string sql = @"INSERT INTO nguoi_dung
                           (ma_nguoi_dung, ho_ten, ngay_sinh, ten_dang_nhap, mat_khau, email, so_dien_thoai, trang_thai)
                           VALUES (@id,@ten,@ns,@username,@pwd,@email,@phone,@tt)";

            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@id", user.MaNguoiDung),
                new MySqlParameter("@ten", user.HoTen),
                new MySqlParameter("@ns", user.NgaySinh),
                new MySqlParameter("@username", user.TenDangNhap),
                new MySqlParameter("@pwd", user.MatKhau),
                new MySqlParameter("@email", user.Email ?? ""),
                new MySqlParameter("@phone", user.SoDienThoai ?? ""),
                new MySqlParameter("@tt", user.TrangThai.ToString())
            }) > 0;
        }

        public bool IsUsernameExists(string username)
        {
            string sql = "SELECT COUNT(*) FROM nguoi_dung WHERE ten_dang_nhap = @u";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@u", username)
            });

            if (dt.Rows.Count == 0)
                return false;

            int count = Convert.ToInt32(dt.Rows[0][0]);
            return count > 0;
        }

        public DataTable GetNguoiDungByMa(string maND)
        {
            string sql = @"
                SELECT ma_nguoi_dung, ho_ten, email, so_dien_thoai,
                    ngay_tao, ngay_cap_nhat
                FROM nguoi_dung
                WHERE ma_nguoi_dung = @ma";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@ma", maND)
            });
        }

        public void UpdateNguoiDung(
            string maND, string hoTen, string email, string sdt)
        {
            string sql = @"
                        UPDATE nguoi_dung
                        SET ho_ten = @ten,
                            email = @email,
                            so_dien_thoai = @sdt,
                            ngay_cap_nhat = NOW()
                        WHERE ma_nguoi_dung = @ma";

            DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@ten", hoTen),
                new MySqlParameter("@email", email),
                new MySqlParameter("@sdt", sdt),
                new MySqlParameter("@ma", maND)
            });
        }
        public bool DeleteNguoiDung(string maNguoiDung)
        {
            string sql = @"
                        UPDATE nguoi_dung
                        SET 
                            trang_thai = 'khong_hoat_dong',
                            ngay_cap_nhat = NOW()
                        WHERE ma_nguoi_dung = @ma;";

            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
        new MySqlParameter("@ma", maNguoiDung)
            }) > 0;
        }

        public DataTable GetNguoiDung(string keyword = "")
        {
            string sql = @"
        SELECT ma_nguoi_dung, ho_ten, email, so_dien_thoai,
               trang_thai, ngay_tao, ngay_cap_nhat
        FROM nguoi_dung
        WHERE trang_thai = 'hoat_dong'
          AND (ho_ten LIKE @kw OR email LIKE @kw OR so_dien_thoai LIKE @kw)";

            return DbManager.ExecuteQuery(sql, new MySqlParameter[]{
        new MySqlParameter("@kw", "%" + keyword + "%")});
        }
        public NguoiDung? GetNguoiDungById(string ma)
        {
            string sql = @"SELECT nd.*,
                                gv.trinh_do, gv.chuyen_mon, gv.ngay_vao_lam,
                                ph.nghe_nghiep, ph.dia_chi, ph.ghi_chu,
                                kt.ma_chung_chi_hanh_nghe, kt.phong_ban
                            FROM nguoi_dung nd
                            LEFT JOIN giao_vien gv ON nd.ma_nguoi_dung = gv.ma_giao_vien
                            LEFT JOIN phu_huynh ph ON nd.ma_nguoi_dung = ph.ma_phu_huynh
                            LEFT JOIN ke_toan kt ON nd.ma_nguoi_dung = kt.ma_ke_toan
                            WHERE nd.ma_nguoi_dung = @ma
                            AND nd.trang_thai <> 'khong_hoat_dong'";

            DataTable dt = DbManager.ExecuteQuery(sql, new MySqlParameter[]{
        new MySqlParameter("@ma", ma)});

            if (dt.Rows.Count == 0)
                return null;

            DataRow r = dt.Rows[0];

            
            if (r["trinh_do"] != DBNull.Value)
            {
                return new GiaoVien
                {
                    MaNguoiDung = r["ma_nguoi_dung"].ToString()!,
                    HoTen = r["ho_ten"].ToString()!,
                    Email = r["email"].ToString()!,
                    SoDienThoai = r["so_dien_thoai"].ToString()!,
                    TrangThai = r["trang_thai"].ToString()!,
                    NgaySinh = Convert.ToDateTime(r["ngay_sinh"]),
                    NgayTao = Convert.ToDateTime(r["ngay_tao"]),
                    NgayCapNhat = Convert.ToDateTime(r["ngay_cap_nhat"]),
                    TrinhDo = r["trinh_do"].ToString()!,
                    ChuyenMon = r["chuyen_mon"].ToString()!,
                    NgayVaoLam = r["ngay_vao_lam"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(r["ngay_vao_lam"])
                };
            }

            
            if (r["nghe_nghiep"] != DBNull.Value)
            {
                return new PhuHuynh
                {
                    MaNguoiDung = r["ma_nguoi_dung"].ToString()!,
                    HoTen = r["ho_ten"].ToString()!,
                    Email = r["email"].ToString()!,
                    SoDienThoai = r["so_dien_thoai"].ToString()!,
                    TrangThai = r["trang_thai"].ToString()!,
                    NgaySinh = Convert.ToDateTime(r["ngay_sinh"]),
                    NgayTao = Convert.ToDateTime(r["ngay_tao"]),
                    NgayCapNhat = Convert.ToDateTime(r["ngay_cap_nhat"]),
                    NgheNghiep = r["nghe_nghiep"].ToString()!,
                    DiaChi = r["dia_chi"].ToString()!,
                    GhiChu = r["ghi_chu"].ToString()!
                };
            }

           
            return new KeToan
            {
                MaNguoiDung = r["ma_nguoi_dung"].ToString()!,
                HoTen = r["ho_ten"].ToString()!,
                Email = r["email"].ToString()!,
                SoDienThoai = r["so_dien_thoai"].ToString()!,
                TrangThai = r["trang_thai"].ToString()!,
                NgaySinh = Convert.ToDateTime(r["ngay_sinh"]),
                NgayTao = Convert.ToDateTime(r["ngay_tao"]),
                NgayCapNhat = Convert.ToDateTime(r["ngay_cap_nhat"]),
                MaChungChiHanhNghe = r["ma_chung_chi_hanh_nghe"].ToString()!,
                PhongBan = r["phong_ban"].ToString()!
            };
        }
    }

}