using System;
using KindergartenManager.Model;
using KindergartenManager.Service;
using KindergartenManager.Repository;
using System.Data.SqlTypes;

namespace KindergartenManager.View
{
    public class Console_QT
    {
        public static void ConsoleQT(QuanTriVien qt, string choice)
        {
            var repo = new QuanTriRepository();
            QuanTri_Service qtService = new QuanTri_Service();
            Services Services = new Services();
            KeToanService ktService = new KeToanService();

            switch (choice)
            {
                case "1":
                    ShowMenuQLND(qt.MaNguoiDung, qtService, qt.HoTen);
                    break;
                case "2":
                    MenuQLT(qt.MaNguoiDung, qtService, qt.HoTen);
                    break;
                case "3":
                    MenuQuanLyLopHoc(qtService, qt.HoTen);
                    break;
                case "4":
                    Console.Write("Nhập tháng báo cáo: "); int t = int.Parse(Console.ReadLine() ?? "1");
                    Console.Write("Nhập năm báo cáo: "); int n = int.Parse(Console.ReadLine() ?? "2024");

                    Console.WriteLine("\n--- LỰA CHỌN XUẤT BÁO CÁO ---");
                    Console.WriteLine("1. Xem nhanh trên màn hình");
                    Console.WriteLine("2. Xuất file báo cáo tài chính (.csv/Excel)");
                    string bcChoice = Console.ReadLine();

                    if (bcChoice == "1") ktService.XemDoanhThu(t, n);
                    else if (bcChoice == "2") ktService.XuatBaoCaoExcel(t, n);
                    break;

                case "5":
                    Services.XemThongBao(qt.MaNguoiDung);
                    Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                    Console.ReadKey();
                    break;
                default:
                    break;

            }

        }
        public static void MenuQuanLyLopHoc(QuanTri_Service service, string Ten)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

                Console.WriteLine("\n    ===== QUẢN LÝ LỚP HỌC =====");
                Console.WriteLine("1. Tạo lớp học");
                Console.WriteLine("2. Xem lớp trống (chưa đủ trẻ)");
                Console.WriteLine("3. Xem trẻ chưa có lớp");
                Console.WriteLine("4. Phân trẻ tự động");
                Console.WriteLine("5. Phân trẻ thủ công");
                Console.WriteLine("6. Đổi tên lớp");
                Console.WriteLine("7. Đổi giáo viên phụ trách");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");

                string chon = Console.ReadLine()!;
                if (chon == "0") break;

                try
                {
                    switch (chon)
                    {
                        case "1":
                            Console.Write("Tên lớp: ");
                            string tenLop = Console.ReadLine()!;
                            Console.Write("Năm học: ");
                            string namHoc1 = Console.ReadLine()!;
                            service.TaoLop(tenLop, namHoc1);
                            break;

                        case "2":
                            Console.Write("Năm học: ");
                            string namHoc2 = Console.ReadLine()!;
                            service.InLopChuaDuTre(namHoc2);
                            break;

                        case "3":
                            Console.Write("Năm học: ");
                            string namHoc3 = Console.ReadLine()!;
                            service.InTreChuaCoLop(namHoc3);
                            break;

                        case "4":
                            Console.Write("Năm học: ");
                            string namHoc4 = Console.ReadLine()!;
                            service.PhanTreTuDong(namHoc4);
                            break;

                        case "5":
                            Console.Write("Mã trẻ: ");
                            string maTre = Console.ReadLine()!;
                            Console.Write("Mã lớp: ");
                            string maLopTC = Console.ReadLine()!;
                            Console.Write("Năm học: ");
                            string namHoc5 = Console.ReadLine()!;
                            service.PhanTreThuCong(maTre, maLopTC, namHoc5);
                            break;

                        case "6":
                            Console.Write("Mã lớp: ");
                            string maLopDoiTen = Console.ReadLine()!;
                            Console.Write("Tên lớp mới: ");
                            string tenMoi = Console.ReadLine()!;
                            service.DoiTenLop(maLopDoiTen, tenMoi);
                            break;

                        case "7":
                            Console.Write("Mã lớp: ");
                            string maLopDoiGV = Console.ReadLine()!;
                            Console.Write("Mã giáo viên mới: ");
                            string maGV = Console.ReadLine()!;
                            service.DoiGiaoVien(maLopDoiGV, maGV);
                            break;

                        default:
                            Console.WriteLine(" Lựa chọn không hợp lệ");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" Lỗi: " + ex.Message);
                }

                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
            }
        }

        private static void ShowMenuQLND(string maQT, QuanTri_Service Service, string Ten)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

                Console.WriteLine("\n   --- MENU Quản lý người dùng ---");
                Console.WriteLine("1. Tạo tài khoản người dùng");
                Console.WriteLine("2. Sửa thông tin người dùng");
                Console.WriteLine("3. Xóa thông tin người dùng");
                Console.WriteLine("4. Tìm kiếm thông tin người dùng");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");

                string subChoice = Console.ReadLine() ?? "";
                if (subChoice == "0") break;
                // Console.Clear();
                switch (subChoice)
                {
                    case "1":
                        Service.TaoNguoiDung(Ten);
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "2":
                        Console.Write("Nhập mã người dùng cần sửa: ");
                        Service.MenuSuaNguoiDung(maQT, Ten);
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "3":
                        Console.Write("Nhập mã người dùng cần xóa: ");
                        Service.VoHieuHoaNguoiDung();
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "4":

                        Console.Write("Nhập từ khóa tìm kiếm (tên / email / SĐT): ");
                        string kw = Console.ReadLine()!;
                        Service.InNguoiDung(kw);

                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("\nChọn sai!!! \nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                }

            }
        }
        private static void MenuQLT(string maQT, QuanTri_Service Service, string Ten)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

                Console.WriteLine("\n     --- MENU QUẢN LÝ TRẺ ---");
                Console.WriteLine("1. Thêm thông tin tuyển sinh");
                Console.WriteLine("2. Sửa thông tin trẻ");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");

                string subChoice = Console.ReadLine() ?? "";
                if (subChoice == "0") break;
                Console.Clear();
                switch (subChoice)
                {
                    case "1":
                        Service.ThongTinTuyenSinh();
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "2":
                        Service.SuaTre(maQT);
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();

                        break;
                    default:
                        Console.WriteLine("\nChọn sai!!! \nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;

                }


            }
        }
    }
}