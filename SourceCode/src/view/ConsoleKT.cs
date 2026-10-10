using System;
using System.Data;
using KindergartenManager.Model;
using KindergartenManager.Service;
using KindergartenManager.Repository;

namespace KindergartenManager.View
{
    public class Console_KT
    {
        public static void ConsoleKT(KeToan kt, string choice)
        {
            KeToanService ktService = new KeToanService();
            Services commonServices = new Services(); 

            switch (choice)
            {
                case "1":
                    MenuHocPhi(kt.MaNguoiDung, ktService, kt.HoTen);
                    break;

                case "2":
                    Console.Write("Nhập tháng báo cáo: "); int t = int.Parse(Console.ReadLine() ?? "1");
                    Console.Write("Nhập năm báo cáo: "); int n = int.Parse(Console.ReadLine() ?? "2024");

                    Console.WriteLine("\n--- LỰA CHỌN XUẤT BÁO CÁO ---");
                    Console.WriteLine("1. Xem nhanh trên màn hình");
                    Console.WriteLine("2. Xuất file báo cáo tài chính (.csv/Excel)");
                    string bcChoice = Console.ReadLine();

                    if (bcChoice == "1") ktService.XemDoanhThu(t, n);
                    else if (bcChoice == "2") ktService.XuatBaoCaoExcel(t, n);
                    Console.ReadKey();
                    break;
                case "3":
                    commonServices.XemThongBao(kt.MaNguoiDung);
                    break;

                default:
                    Console.WriteLine("Chọn sai!! Vui lòng chọn lại!");
                    break;
            }
        }

        private static void MenuHocPhi(string maKT, KeToanService service, string Ten)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

                Console.WriteLine("\n     === HỆ THỐNG QUẢN LÝ TÀI CHÍNH ===");
                Console.WriteLine("1. Danh sách học phí");
                Console.WriteLine("2. Tìm kiếm hóa đơn/học phí theo mã");
                Console.WriteLine("3. Quản lý diện Miễn Giảm");
                Console.WriteLine("4. Xuất phiếu thu & Gửi thông báo cho Phụ huynh");
                Console.WriteLine("5. Thêm học phí");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Chọn chức năng: ");

                string sub = Console.ReadLine() ?? "";
                if (sub == "0") break;

                switch (sub)
                {
                    case "1":
                        service.HienThiDSHocPhiPhanLoai();
                        Console.WriteLine("\nẤn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Write("Nhập mã học phí/hóa đơn cần tìm: ");
                        string maSearch = Console.ReadLine() ?? "";
                        service.TimKiemHocPhi(maSearch);
                        Console.ReadKey();
                        break;

                    case "3":
                        MenuQuanLyMienGiam(service, Ten);
                        break;

                    case "4":
                        service.HienThiDSHocPhiNo(); 
                        Console.Write("\nNhập mã học phí xác nhận đóng & xuất phiếu: ");
                        string maHP = Console.ReadLine() ?? "";
                        service.XuLyThanhToanVaThongBao(maHP, maKT);
                        Console.ReadKey();
                        break;
                    case "5":
                        Console.Write("Mã trẻ: ");
                        string tId = Console.ReadLine();

                        Console.Write("Tháng: ");
                        int th = int.Parse(Console.ReadLine());

                        Console.Write("Năm: ");
                        int na = int.Parse(Console.ReadLine());
                        decimal st;
                        while (true)
                        {
                            Console.Write("Số tiền gốc: ");
                            decimal Sotien = decimal.Parse(Console.ReadLine());
                            if (Sotien <= 0)
                            {
                                Console.WriteLine("Số tiền không hợp lệ!");
                            }
                            else
                            {
                                st = Sotien;
                                break;
                            }
                        }

                        service.TaoMoiHocPhi(tId, th, na, st, maKT);
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Chọn sai!! Vui lòng chọn lại!");
                        break;
                }
            }
        }

        private static void MenuQuanLyMienGiam(KeToanService service, string Ten)
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("         Trường mầm non số 1 dcv          ");
            Console.WriteLine("==========================================");
            Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

            Console.WriteLine("\n          --- QUẢN LÝ MIỄN GIẢM ---");
            service.HienThiDSHocPhiMienGiam();

            Console.WriteLine("\n[A] Thêm mới | [E] Sửa | [D] Xóa | [Any] Thoát");
            string action = Console.ReadLine()?.ToUpper() ?? "";

            if (action == "A")
            {
                string ma = service.SinhMaTuDong("mien_giam", "ma_mien_giam", "MG");
                Console.Write("Lý do: "); string ld = Console.ReadLine() ?? "";
                Console.Write("Tỷ lệ (ví dụ 0.5 cho 50%): "); float tl = float.Parse(Console.ReadLine() ?? "0");
                service.ThemMienGiam(ma, ld, tl);
                Console.ReadLine();
            }
            else if (action == "E")
            {
                Console.Write("Nhập mã cần sửa: "); string ma = Console.ReadLine() ?? "";
                Console.Write("Lý do mới: "); string ld = Console.ReadLine() ?? "";
                Console.Write("Tỷ lệ mới: "); float tl = float.Parse(Console.ReadLine() ?? "0");
                service.CapNhatMienGiam(ma, ld, tl);
                Console.ReadLine();

            }
            else if (action == "D")
            {
                Console.Write("Nhập mã cần xóa: "); string ma = Console.ReadLine() ?? "";
                service.XoaMienGiam(ma);
                Console.ReadLine();

            }
        }
    }
}