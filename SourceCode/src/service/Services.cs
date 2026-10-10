using KindergartenManager.Model;
using KindergartenManager.Repository;
using System;
using System.Data;
namespace KindergartenManager.Service
{
    public class Services
    {
        private readonly DataRepository _repo;
        private readonly ThongBaoRepository _repotb;

        public Services()
        {
            _repo = new DataRepository();
            _repotb = new ThongBaoRepository();
        }

        public NguoiDung? Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            return _repo.KiemTraDangNhap(username, password);
        }
        public string ChangePassword(NguoiDung currentUser, string oldPass, string newPass, string confirmPass)
        {
            if (currentUser.MatKhau != oldPass)
                return "Mật khẩu cũ không chính xác.";

            if (newPass != confirmPass)
                return "Mật khẩu mới và xác nhận mật khẩu không khớp.";

            if (newPass.Length < 6)
                return "Mật khẩu mới phải có ít nhất 6 ký tự.";

            bool success = _repo.DoiMatKhau(currentUser.MaNguoiDung, newPass);

            if (success)
            {
                currentUser.MatKhau = newPass; 
                return "Đổi mật khẩu thành công!";
            }

            return "Có lỗi xảy ra trong quá trình cập nhật.";
        }

        public static void InDanhSachThongBao(DataTable dt, string prefix, bool laNguoiNhan)
        {
            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("- Không có thông báo.");
                return;
            }

            foreach (DataRow r in dt.Rows)
            {
                string time = Convert.ToDateTime(r["thoi_gian"])
                              .ToString("dd/MM/yyyy HH:mm");
                string trangThai = r["trang_thai"]?.ToString() ?? "chua_xem";
                string hienThiTrangThai = trangThai == "da_xem" ? "Đã xem" : "Chưa xem";

                Console.WriteLine($"[{time}] {prefix}: {r["ten_nguoi_lien_quan"]}");
                Console.WriteLine($" > {r["tieu_de"]}");
                Console.WriteLine($" > {r["noi_dung"]}");
                Console.WriteLine(new string('-', 30));

                if (laNguoiNhan)
                {
                    int maTB = Convert.ToInt32(r["ma_thong_bao"]);
                    ThongBaoRepository.CapNhatTrangThaiThongBao(maTB);
                }
            }
        }

        public void XemThongBao(string maNguoiDung)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"          MENU THONG BAO  \n");
               
                Console.WriteLine("1. Thông báo đã nhận");
                Console.WriteLine("2. Thông báo đã gửi");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");
                string subChoice = Console.ReadLine() ?? "";

                if (subChoice == "0") break;
                Console.Clear();


                switch (subChoice)
                {

                    case "1":
                        Console.Clear();
                        Console.WriteLine("=== HỘP THƯ ===");

                        Console.WriteLine("\n[ TIN ĐẾN ]");
                        InDanhSachThongBao(
                            _repotb.GetThongBaoDaNhan(maNguoiDung),
                            "Từ",
                            true
                        );
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "2":
                        Console.Clear();
                        Console.WriteLine("=== HỘP THƯ ===");
                        Console.WriteLine("\n[ TIN ĐÃ GỬI ]");
                        InDanhSachThongBao(
                            _repotb.GetThongBaoDaGui(maNguoiDung),
                            "Đến",
                            false
                        );
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("\nChọn sai ! \nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}