using KindergartenManager.Model;
using KindergartenManager.Repository;
using System;
using System.Data;
using System.Globalization;
using System.Text;
using MySqlConnector;
using System.Collections.Generic;


namespace KindergartenManager.Service
{
    public class QuanTri_Service
    {
        private readonly DataRepository _repo;
        private readonly QuanTriRepository _repoqt;
        private readonly GiaoVienRepository _repogv;
        private readonly PhuHuynhRepository _repoph;
        private readonly KeToanRepository _repokt;
        private readonly TreRepository _repot;

        private readonly LopHocRepository _repol;
        public QuanTri_Service()
        {
            _repo = new DataRepository();
            _repoqt = new QuanTriRepository();
            _repogv = new GiaoVienRepository();
            _repoph = new PhuHuynhRepository();
            _repokt = new KeToanRepository();
            _repot = new TreRepository();
            _repol = new LopHocRepository();
        }

        public void TaoNguoiDung(string Ten)
        {
            while (true)
            {
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

                Console.WriteLine("\n         --- MENU TẠO TÀI KHOẢN ---");
                Console.WriteLine("1. Tạo tài khoản quản trị");
                Console.WriteLine("2. Tạo tài khoản giáo viên");
                Console.WriteLine("3. Tạo tài khoản kế toán");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");

                string choice = Console.ReadLine()!;
                if (choice == "0") break;

                try
                {
                    switch (choice)
                    {
                        case "1":
                            TaoQuanTri();
                            break;
                        case "2":
                            TaoGiaoVien();
                            break;
                        case "3":
                            TaoKeToan();
                            break;
                        default:
                            Console.WriteLine(" Lựa chọn không hợp lệ");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi: " + ex.Message);
                }

                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
            }
        }


        public void InTreChuaCoLop(string namHoc)
        {
            Console.WriteLine("\n--- TRẺ CHƯA CÓ LỚP ---");
            DataTable tre = _repol.GetTreChuaCoLopCoTuoi(namHoc);

            if (tre.Rows.Count == 0)
            {
                Console.WriteLine("Không có trẻ trống");
                return;
            }

            Console.WriteLine("Mã trẻ | Họ tên | Tuổi");
            foreach (DataRow r in tre.Rows)
            {
                Console.WriteLine(
                    $"{r["ma_tre"],-8} | {r["ho_ten"],-20} | {r["tuoi"]}"
                );
            }
        }

        public void PhanTreThuCong(string maTre, string maLop, string namHoc)
        {
            if (_repot.TreDaCoLop(maTre, namHoc))
            {
                Console.WriteLine(" Trẻ đã có lớp trong năm học này. Hủy thao tác.");
                return;
            }

            int tuoiTre = _repot.GetTuoiTre(maTre);
            var (min, max) = _repol.GetDoTuoiLop(maLop);

            if (tuoiTre < min || tuoiTre > max)
            {
                Console.WriteLine($" Trẻ {tuoiTre} tuổi, không phù hợp độ tuổi lớp ({min}-{max}).");
                Console.Write("Admin có muốn tiếp tục? (y/n): ");
                if (Console.ReadLine()?.ToLower() != "y")
                {
                    Console.WriteLine(" Đã hủy thao tác.");
                    return;
                }
            }

            int siSo = _repol.GetSiSoLop(maLop, namHoc);
            if (siSo >= 10)
            {
                Console.WriteLine("Lớp đã đủ 10 trẻ (LỚP ĐẦY).");
                Console.Write("Admin có cho phép thêm trẻ? (y/n): ");
                if (Console.ReadLine()?.ToLower() != "y")
                {
                    Console.WriteLine("Không thêm trẻ vào lớp.");
                    return;
                }
            }


            _repol.InsertHocTai(maTre, maLop, namHoc);
            Console.WriteLine("Gán trẻ vào lớp thành công.");
        }

        public void InLopChuaDuTre(string namHoc)
        {
            DataTable dt = _repol.GetLopChuaDuTre(namHoc);

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("Không có lớp nào chưa đủ trẻ.");
                return;
            }

            Console.WriteLine("\n--- DANH SÁCH LỚP CHƯA ĐỦ TRẺ ---");
            Console.WriteLine("{0,-10} {1,-20} {2,-25} {3,5}",
                "Mã lớp", "Tên lớp", "Giáo viên", "Sĩ số");

            foreach (DataRow r in dt.Rows)
            {
                Console.WriteLine("{0,-10} {1,-20} {2,-25} {3,5}",
                    r["ma_lop_hoc"],
                    r["ten_lop"],
                    r["ten_giao_vien"],
                    r["si_so"]);
            }
        }

        public void DoiGiaoVien(string maLop, string maGV)
        {
            if (!_repol.IsLopHocExists(maLop))
            {
                Console.WriteLine("Lớp không tồn tại");
                return;
            }

            if (!_repogv.IsGiaoVienExists(maGV))
            {
                Console.WriteLine("Giáo viên không tồn tại");
                return;
            }

            _repol.UpdateGiaoVien(maLop, maGV);
            Console.WriteLine("Đã đổi giáo viên phụ trách");
        }

        public void ThongTinTuyenSinh()
        {
            TaoPhuHuynhVaTre();

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();

        }

        private void TaoQuanTri()
        {
            var qt = new QuanTriVien();
            NhapNguoiDung(qt);
            qt.MaNguoiDung = IdGenerator.GenerateId("QT", "nguoi_dung", "ma_nguoi_dung");

            _repo.Insert(qt);
            _repoqt.InsertQuanTriVien(qt);
            Console.WriteLine("Tạo quản trị viên thành công");
        }

        private void TaoGiaoVien()
        {
            var gv = new GiaoVien();
            NhapNguoiDung(gv);
            gv.MaNguoiDung = IdGenerator.GenerateId("GV", "nguoi_dung", "ma_nguoi_dung");

            Console.Write("Trình độ: ");
            gv.TrinhDo = Console.ReadLine();

            Console.Write("Chuyên môn: ");
            gv.ChuyenMon = Console.ReadLine();

            gv.NgayVaoLam = ReadDate("Ngày vào làm (yyyy-MM-dd): ");

            Console.Write("Ghi chú: ");
            gv.GhiChu = Console.ReadLine();

            _repo.Insert(gv);
            _repogv.InsertGiaoVien(gv);
            Console.WriteLine(" Tạo giáo viên thành công");
        }

        private void TaoKeToan()
        {
            var kt = new KeToan();
            NhapNguoiDung(kt);
            kt.MaNguoiDung = IdGenerator.GenerateId("KT", "nguoi_dung", "ma_nguoi_dung");

            Console.Write("Mã chứng chỉ hành nghề: ");
            kt.MaChungChiHanhNghe = Console.ReadLine();

            Console.Write("Phòng ban: ");
            kt.PhongBan = Console.ReadLine();

            _repo.Insert(kt);
            _repokt.InsertKeToan(kt);
            Console.WriteLine(" Tạo kế toán thành công");
        }

        private void TaoPhuHuynhVaTre()
        {Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("         Trường mầm non số 1 dcv          ");
            Console.WriteLine("==========================================");
            Console.WriteLine($"           HỆ THỐNG NHÀ TRẺ               ");
            Console.WriteLine("    --- NHẬP THÔNG TIN PHỤ HUYNH ---");
            var ph = new PhuHuynh();
            NhapNguoiDung(ph);
            ph.MaNguoiDung = IdGenerator.GenerateId("PH", "nguoi_dung", "ma_nguoi_dung");

            Console.Write("Nghề nghiệp: ");
            ph.NgheNghiep = Console.ReadLine();

            Console.Write("Địa chỉ: ");
            ph.DiaChi = Console.ReadLine();

            Console.Write("Ghi chú: ");
            ph.GhiChu = Console.ReadLine();

            _repo.Insert(ph);
            _repoph.InsertPhuHuynh(ph);

            Console.WriteLine("\n--- NHẬP THÔNG TIN TRẺ ---");

            do
            {
                var tre = NhapTre(ph.MaNguoiDung);
                _repot.InsertTre(tre);

                Console.Write("Nhập thêm trẻ cho phụ huynh này? (y/n): ");
            }
            while (Console.ReadLine()?.ToLower() == "y");

            Console.WriteLine(" Tạo phụ huynh và trẻ thành công");
        }



        public void NhapNguoiDung(NguoiDung user)
        {
            do
            {
                Console.Write("Họ tên: ");
                user.HoTen = Console.ReadLine()!;
            }
            while (string.IsNullOrWhiteSpace(user.HoTen));
            while (true)
            {
                Console.Write("Ngày sinh (yyyy-MM-dd): ");
                string ns = Console.ReadLine()!;

                if (DateTime.TryParseExact(
                    ns,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime ngaySinh))
                {
                    user.NgaySinh = ngaySinh;
                    break;
                }

                Console.WriteLine(" Ngày sinh không hợp lệ.");
            }

            Console.Write("Tên đăng nhập (Enter = mặc định): ");

            string username;
            while (true)
            {
                string input = Console.ReadLine()!.Trim();

                username = string.IsNullOrWhiteSpace(input)
                    ? GenerateUsername(user.HoTen)
                    : input;

                if (!_repo.IsUsernameExists(username))
                    break;

                Console.WriteLine($" Tên đăng nhập '{username}' đã tồn tại, vui lòng chọn lại.");
                Console.Write("Nhập tên đăng nhập khác: ");
            }

            user.TenDangNhap = username;

            user.MatKhau = "hash123";

            while (true)
            {
                Console.Write("Email: ");
                string email = Console.ReadLine()!.Trim();

                if (string.IsNullOrEmpty(email) || IsValidEmail(email))
                {
                    user.Email = email;
                    break;
                }

                Console.WriteLine(" Email không đúng định dạng (vd: abc@gmail.com)");
            }
            while (true)
            {
                Console.Write("Số điện thoại (10 số): ");
                string phone = Console.ReadLine()!.Trim();

                if (string.IsNullOrEmpty(phone) || IsValidPhone(phone))
                {
                    user.SoDienThoai = phone;
                    break;
                }

                Console.WriteLine(" Số điện thoại phải gồm đúng 10 chữ số.");
            }

            Console.Write("Trạng thái (Hoạt động[1]/Không hoạt động[2]): ");
            user.TrangThai = Console.ReadLine() == "1"
                ? TrangThaiNguoiDung.hoat_dong.ToString()
                : TrangThaiNguoiDung.khong_hoat_dong.ToString();
        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            return System.Text.RegularExpressions.Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"
            );
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            return System.Text.RegularExpressions.Regex.IsMatch(
                phone,
                @"^\d{10}$"
            );
        }


        public static Tre NhapTre(string maPhuHuynh)
        {
            var tre = new Tre
            {
                MaTre = IdGenerator.GenerateId("T", "tre", "ma_tre"),
                PhuHuynhId = maPhuHuynh
            };

            Console.Write("Họ tên trẻ: ");
            tre.HoTen = Console.ReadLine()!;

            DateTime ngaySinh;
            while (true)
            {
                Console.Write("Ngày sinh (yyyy-MM-dd): ");
                string ns = Console.ReadLine()!;
                if (DateTime.TryParse(ns, out ngaySinh))
                {
                    tre.NgaySinh = ngaySinh;
                    break;
                }
                Console.WriteLine(" Ngày sinh không hợp lệ.");
            }

            Console.Write("Giới tính (Nam/Nu/Khac): ");
            tre.GioiTinh = Console.ReadLine()!;

            Console.Write("Tình trạng: ");
            tre.TinhTrang = Console.ReadLine();

            Console.Write("Trạng thái (dang_theo_hoc[1]/da_ra_truong[2]): ");
            tre.TrangThai = Console.ReadLine() == "1"
                ? TrangThaiTre.dang_theo_hoc.ToString()
                : TrangThaiTre.da_ra_truong.ToString();

            return tre;
        }


        public static DateTime ReadDate(string message)
        {
            while (true)
            {
                Console.Write(message);
                if (DateTime.TryParseExact(
                        Console.ReadLine(),
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime date))
                    return date;

                Console.WriteLine("Sai định dạng ngày (yyyy-MM-dd)");
            }
        }

        public static string GenerateUsername(string hoTen)
        {
            return RemoveDiacritics(hoTen)
                .Replace(" ", "")
                .ToLower();
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalized)
                if (Char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public void TaoLop(string tenLop, string namHoc)
        {
            if (string.IsNullOrWhiteSpace(tenLop) || string.IsNullOrWhiteSpace(namHoc))
            {
                Console.WriteLine("Tên lớp và năm học không được để trống.");
                return;
            }

            DataTable gv = _repol.GetGiaoVienChuaCoLop(namHoc);
            if (gv.Rows.Count == 0)
            {
                Console.WriteLine("Không còn giáo viên trống. Vui lòng tạo giáo viên trước.");
                return;
            }

            Console.WriteLine("Danh sách giáo viên chưa có lớp:");
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {gv.Rows[i]["ho_ten"]}");
            }

            int chon;
            while (true)
            {
                Console.Write("Chọn giáo viên (số): ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out chon))
                {
                    Console.WriteLine("Vui lòng nhập số.");
                    continue;
                }

                if (chon < 1 || chon > gv.Rows.Count)
                {
                    Console.WriteLine("Số không hợp lệ.");
                    continue;
                }

                break;
            }

            string maGV = gv.Rows[chon - 1]["ma_giao_vien"].ToString();
            string maLop = "LH" + DateTime.Now.Ticks.ToString().Substring(10);

            _repol.InsertLopHoc(new LopHoc
            {
                MaLopHoc = maLop,
                TenLop = tenLop.Trim(),
                GiaoVienId = maGV,
                NamHoc = namHoc.Trim()
            });

            Console.WriteLine("Tạo lớp thành công.");
        }


        public void PhanTreTuDong(string namHoc)
        {
            DataTable dsTre = _repol.GetTreChuaCoLop(namHoc);
            if (dsTre.Rows.Count == 0)
            {
                Console.WriteLine("Không có trẻ nào chưa có lớp.");
                return;
            }

            DataTable dsLop = _repol.GetLopTheoNamHoc(namHoc);
            if (dsLop.Rows.Count == 0)
            {
                Console.WriteLine("Không có lớp nào trong năm học này.");
                return;
            }

            List<string> treKhongGanDuoc = new();
            int demGanThanhCong = 0;

            foreach (DataRow tre in dsTre.Rows)
            {
                string maTre = tre["ma_tre"].ToString()!;
                string tenTre = tre["ho_ten"].ToString()!;
                int tuoiTre = _repot.GetTuoiTre(maTre);

                bool daGan = false;

                foreach (DataRow lop in dsLop.Rows)
                {
                    string maLop = lop["ma_lop_hoc"].ToString()!;
                    string tenLop = lop["ten_lop"].ToString()!;

                    var (min, max) = _repol.GetDoTuoiLop(maLop);
                    if (tuoiTre < min || tuoiTre > max)
                        continue;

                    int siSo = _repol.GetSiSoLop(maLop, namHoc);
                    if (siSo >= 10)
                        continue;

                    _repol.InsertHocTai(maTre, maLop, namHoc);
                    demGanThanhCong++;
                    daGan = true;

                    Console.WriteLine($" Gán trẻ {tenTre} ({tuoiTre} tuổi) → lớp {tenLop}");
                    break;
                }

                if (!daGan)
                    treKhongGanDuoc.Add($"{tenTre} ({tuoiTre} tuổi)");
            }

            Console.WriteLine("\n===== KẾT QUẢ PHÂN LỚP =====");
            Console.WriteLine($" Gán thành công: {demGanThanhCong} trẻ");

            if (treKhongGanDuoc.Count > 0)
            {
                Console.WriteLine("Các trẻ KHÔNG tìm được lớp phù hợp:");
                foreach (var t in treKhongGanDuoc)
                    Console.WriteLine(" - " + t);
            }
        }

        public void DoiTenLop(string maLop, string tenMoi)
        {
            if (string.IsNullOrWhiteSpace(maLop) || string.IsNullOrWhiteSpace(tenMoi))
            {
                Console.WriteLine(" Mã lớp hoặc tên mới không hợp lệ.");
                return;
            }

            string namHocCu = _repol.GetNamHocByMaLop(maLop.Trim());
            if (string.IsNullOrEmpty(namHocCu))
            {
                Console.WriteLine(" Không tìm thấy lớp.");
                return;
            }

            string namHocMoi = namHocCu;

            Console.Write($"Bạn có muốn cập nhật năm học? (y/n): ");
            if (Console.ReadLine()!.Trim().ToLower() == "y")
            {
                namHocMoi = TangNamHoc(namHocCu);
                Console.WriteLine($" Năm học mới: {namHocMoi}");
            }

            _repol.UpdateTenVaNamHoc(
                maLop.Trim(),
                tenMoi.Trim(),
                namHocMoi
            );

            Console.WriteLine(" Đã cập nhật tên lớp" +
                (namHocMoi != namHocCu ? " và năm học." : "."));
        }


        private string TangNamHoc(string namHoc)
        {
            var parts = namHoc.Split('-');
            if (parts.Length != 2) return namHoc;

            int nam1 = int.Parse(parts[0]) + 1;
            int nam2 = int.Parse(parts[1]) + 1;

            return $"{nam1}-{nam2}";
        }


        public void SuaTre(string maQT)
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("         Trường mầm non số 1 dcv          ");
            Console.WriteLine("==========================================");
            Console.WriteLine($"          HỆ THỐNG NHÀ TRẺ               ");
            Console.Write("Nhập mã trẻ: ");
            string ma = Console.ReadLine()!;

            DataTable dt = _repot.GetTreByMa(ma);
            if (dt.Rows.Count == 0)
            {
                Console.WriteLine(" Không tìm thấy trẻ.");
                return;
            }

            DataRow r = dt.Rows[0];

            Console.WriteLine("\n THÔNG TIN TRẺ");
            Console.WriteLine($"Tên        : {r["ho_ten"]}");
            Console.WriteLine($"Ngày sinh  : {Convert.ToDateTime(r["ngay_sinh"]):dd/MM/yyyy}");
            Console.WriteLine($"Tình trạng : {r["tinh_trang"]}");
            Console.WriteLine($"Giới tính  : {r["gioi_tinh"]}");
            Console.WriteLine($"Trạng thái : {r["trang_thai"]}");

            Console.WriteLine("\n Nhập mới (Enter = giữ nguyên)");


            Console.Write("Tên: ");
            string ten = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(ten))
                ten = r["ho_ten"].ToString()!;

            DateTime ngaySinh;
            Console.Write("Ngày sinh (yyyy-MM-dd): ");
            string ns = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(ns))
            {
                ngaySinh = Convert.ToDateTime(r["ngay_sinh"]);
            }
            else if (!DateTime.TryParse(ns, out ngaySinh))
            {
                Console.WriteLine(" Ngày sinh không hợp lệ.");
                return;
            }


            Console.Write("Tình trạng: ");
            string tinhTrang = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(tinhTrang))
                tinhTrang = r["tinh_trang"].ToString()!;

            Console.Write("Giới tính (1-Nam, 2-Nữ, 3-Khác, Enter=giữ nguyên): ");

            GioiTinh gtCu = Enum.Parse<GioiTinh>(r["gioi_tinh"].ToString()!);
            GioiTinh gioiTinhMoi = ReadEnumOrKeep("Giới tính", gtCu);

            Console.Write("Trạng thái (1-Đang theo học, 2-Đã ra trường, Enter=giữ nguyên): ");
            TrangThaiTre ttCu =
            Enum.Parse<TrangThaiTre>(r["trang_thai"].ToString()!);

            TrangThaiTre trangThaiMoi =
                ReadEnumOrKeep("Trạng thái", ttCu);

            _repot.UpdateTre(
                ma,
                ten,
                tinhTrang,
                ngaySinh,
                gioiTinhMoi.ToString(),
                trangThaiMoi.ToString()
            );

            string phuHuynhId = _repot.GetPhuHuynhIdByTre(ma);
            if (!string.IsNullOrEmpty(phuHuynhId))
            {
                DataTable dtGV = _repot.GetGiaoVienPhuTrach(ma);
                string gvId = dtGV.Rows.Count > 0 ? dtGV.Rows[0][0].ToString()! : "";

                string noiDung =
                    "Thông tin của bé vừa được cập nhật.\n"
                  + "Vui lòng kiểm tra lại.\n"
                  + (string.IsNullOrEmpty(gvId)
                        ? "Nếu có thắc mắc, vui lòng liên hệ quản trị viên."
                        : $"Liên hệ giáo viên phụ trách ({gvId}) hoặc quản trị viên.");

                NotificationService.GuiThongBao(
                    NguoiGuiid: maQT,
                    nguoiNhanId: phuHuynhId,
                    tieuDe: "Cập nhật thông tin trẻ",
                    noiDung: noiDung,
                    loai: LoaiThongBao.he_thong.ToString()
                );
            }

            Console.WriteLine("Đã cập nhật thông tin trẻ.");
        }

        public static T ReadEnumOrKeep<T>(string label, T current) where T : struct, Enum
        {
            Console.WriteLine($"{label}: ");
            foreach (var v in Enum.GetValues(typeof(T)))
            {
                Console.WriteLine($"  {Convert.ToInt32(v)} - {v}");
            }
            Console.Write("Chọn (Enter = giữ nguyên): ");
            string input = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(input))
                return current;

            if (int.TryParse(input, out int val) && Enum.IsDefined(typeof(T), val))
                return (T)Enum.ToObject(typeof(T), val);

            Console.WriteLine("Giá trị không hợp lệ, giữ nguyên.");
            return current;
        }
        public void SuaPhuHuynh(PhuHuynh ph, string maQT)
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("         Trường mầm non số 1 dcv          ");
            Console.WriteLine("==========================================");
            Console.WriteLine($"          HỆ THỐNG NHÀ TRẺ               ");
            Console.WriteLine("\n  --- SỬA THÔNG TIN PHỤ HUYNH ---");

            Console.Write($"Nghề nghiệp ({ph.NgheNghiep}): ");
            string nn = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nn)) ph.NgheNghiep = nn;

            Console.Write($"Địa chỉ ({ph.DiaChi}): ");
            string dc = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dc)) ph.DiaChi = dc;

            _repo.UpdateNguoiDung(ph.MaNguoiDung, ph.HoTen, ph.Email, ph.SoDienThoai);
            _repoph.UpdatePhuHuynh(ph);

            NotificationService.GuiThongBao(
                maQT,
                ph.MaNguoiDung,
                "Cập nhật thông tin",
                "Thông tin cá nhân của bạn vừa được cập nhật. "
                + "Nếu có sai sót, vui lòng liên hệ nhà trường.",
                LoaiThongBao.he_thong.ToString()
            );

            Console.WriteLine(" Đã cập nhật thông tin phụ huynh.");
        }

        public void MenuSuaNguoiDung(string maQT, string Ten)
        {
            Console.Write("Nhập mã người dùng cần sửa: ");
            string ma = Console.ReadLine()!.Trim();

            NguoiDung? user = _repo.GetNguoiDungById(ma);
            if (user == null)
            {
                Console.WriteLine(" Không tìm thấy người dùng.");
                return;
            }

            user.HienThiThongTin();
            Console.WriteLine("==========================================");
            Console.WriteLine("         Trường mầm non số 1 dcv          ");
            Console.WriteLine("==========================================");
            Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

            Console.WriteLine("\n        --- CHỈNH SỬA THÔNG TIN ---");
            Console.WriteLine("1. Thông tin chung");
            Console.WriteLine("2. Thông tin theo vai trò");
            Console.WriteLine("0. Hủy");
            Console.Write("Chọn: ");

            string chon = Console.ReadLine()!;
            if (chon == "1")
                SuaThongTinChung(user, maQT);
            else if (chon == "2")
                SuaTheoVaiTro(user, maQT);
        }
        private void SuaThongTinChung(NguoiDung u, string maQT)
        {
            Console.Write($"Họ tên ({u.HoTen}): ");
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) u.HoTen = input;

            {
                Console.Write("Email: ");
                string email = Console.ReadLine()!.Trim();

                if (string.IsNullOrEmpty(email) || IsValidEmail(email))
                {
                    u.Email = email;
                }
                else
                {
                    Console.WriteLine(" Email không đúng định dạng (vd: abc@gmail.com)");
                }

            }
            while (true)
            {
                Console.Write("Số điện thoại (10 số): ");
                string phone = Console.ReadLine()!.Trim();

                if (string.IsNullOrEmpty(phone) || IsValidPhone(phone))
                {
                    u.SoDienThoai = phone;
                    break;
                }

                Console.WriteLine(" Số điện thoại phải gồm đúng 10 chữ số.");
            }
            _repo.UpdateNguoiDung(u.MaNguoiDung, u.HoTen, u.Email, u.SoDienThoai);

            NotificationService.GuiThongBao(
                maQT,
                u.MaNguoiDung,
                "Cập nhật thông tin",
                "Thông tin cá nhân của bạn vừa được cập nhật.",
                LoaiThongBao.he_thong.ToString()
            );

            Console.WriteLine(" Đã cập nhật thông tin chung.");
        }

        private void SuaTheoVaiTro(NguoiDung u, string maQT)
        {
            if (u is GiaoVien gv)
                SuaGiaoVien(gv, maQT);
            else if (u is PhuHuynh ph)
                SuaPhuHuynh(ph, maQT);
            else
                Console.WriteLine(" Vai trò này không có thông tin riêng để sửa.");
        }
        private void SuaGiaoVien(GiaoVien gv, string maQT)
        {
            Console.WriteLine("\n--- SỬA THÔNG TIN GIÁO VIÊN ---");

            Console.Write($"Trình độ ({gv.TrinhDo}): ");
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) gv.TrinhDo = input;

            Console.Write($"Chuyên môn ({gv.ChuyenMon}): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) gv.ChuyenMon = input;

            Console.Write($"Ngày vào làm ({gv.NgayVaoLam:yyyy-MM-dd}): ");
            input = Console.ReadLine();
            if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
            {
                gv.NgayVaoLam = d;
            }
            else
            {
                Console.WriteLine(" Sai định dạng ngày.");
            }
            Console.Write($"Ghi chú ({gv.GhiChu}): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) gv.GhiChu = input;

            _repo.UpdateNguoiDung(gv.MaNguoiDung, gv.HoTen, gv.Email, gv.SoDienThoai);
            _repogv.UpdateGiaoVien(gv);

            NotificationService.GuiThongBao(
                maQT,
                gv.MaNguoiDung,
                "Cập nhật thông tin giáo viên",
                "Thông tin nghề nghiệp của bạn vừa được cập nhật. "
                + "Nếu có sai sót, vui lòng liên hệ quản trị viên.",
                LoaiThongBao.he_thong.ToString()
            );

            Console.WriteLine(" Đã cập nhật thông tin giáo viên.");
        }

        public void VoHieuHoaNguoiDung()
        {
            string ma = Console.ReadLine()!.Trim();

            NguoiDung? u = _repo.GetNguoiDungById(ma);
            if (u == null)
            {
                Console.WriteLine(" Không tìm thấy người dùng.");
                return;
            }

            _repo.DeleteNguoiDung(ma);

            var notifier = NotificationFactory.Create("email");
            notifier.Send(
                u.Email,
                "Tài khoản của bạn đã bị vô hiệu hóa. "
                + "Vui lòng liên hệ quản trị viên nếu cần hỗ trợ."
            );

            Console.WriteLine(" Đã vô hiệu hóa người dùng.");
        }

        public void InNguoiDung(string keyword = "")
        {
            DataTable dt = _repo.GetNguoiDung(keyword);

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine(" Không tìm thấy người dùng.");
                return;
            }

            Console.WriteLine("Danh sách người dùng:");
            foreach (DataRow r in dt.Rows)
            {
                string vai_tro = GetVaiTro(r);
                Console.WriteLine(
                    $"[{r["ma_nguoi_dung"]}] {r["ho_ten"]} | {r["email"]} | {r["so_dien_thoai"]} | " +
                    $"Vai trò: {vai_tro} | Trạng thái: {r["trang_thai"]} | " +
                    $"Tạo: {r["ngay_tao"]} | Cập nhật: {r["ngay_cap_nhat"]}"
                );
            }
        }
        private string GetVaiTro(DataRow r)
        {
            if (r.Table.Columns.Contains("ma_giao_vien") && r["ma_giao_vien"] != DBNull.Value)
                return "Giáo viên";

            if (r.Table.Columns.Contains("ma_phu_huynh") && r["ma_phu_huynh"] != DBNull.Value)
                return "Phụ huynh";

            if (r.Table.Columns.Contains("ma_ke_toan") && r["ma_ke_toan"] != DBNull.Value)
                return "Kế toán";

            if (r.Table.Columns.Contains("ma_quan_tri") && r["ma_quan_tri"] != DBNull.Value)
                return "Quản trị viên";

            return "Không xác định";
        }

    }

    public static class IdGenerator
    {
        public static string GenerateId(string prefix, string tableName, string columnName)
        {
            return prefix + Guid.NewGuid().ToString("N").Substring(0, 6);
        }
    }
}
