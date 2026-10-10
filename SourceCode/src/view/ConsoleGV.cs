using System;
using KindergartenManager.Model;
using KindergartenManager.Service;
using KindergartenManager.Repository;

namespace KindergartenManager.View
{
    public class Console_GV
    {
        public static void ConsoleGV(GiaoVien gv, string choice)
        {
            var repo = new GiaoVienRepository();
            string maLop = repo.GetMaLopCuaGiaoVien(gv.MaNguoiDung);

            if (string.IsNullOrEmpty(maLop))
            {
                Console.WriteLine("Giáo viên chưa được phân lớp.");
                return;
            }

            GiaoVien_Service gvService = new GiaoVien_Service();
            Services Services = new Services();
            switch (choice)
            {

                case "1":
                    ShowTraCuuMenu(maLop, gvService, gv.HoTen);
                    break;
                case "2":
                    ShowMenuSinhhoat(maLop, gv.MaNguoiDung, gvService, gv.HoTen);

                    break;

                case "3":
                    Services.XemThongBao(gv.MaNguoiDung);
                    break;

                default:
                    Console.WriteLine("Chọn sai!!! Vui lòng chọn lại");
                    break;
            }

        }
        private static void ShowMenuSinhhoat(string maLop, string maGV, GiaoVien_Service Service, string Ten)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

                Console.WriteLine("\n     ---  Nhật ký sinh hoạt  ---");
                Console.WriteLine("1. Điểm danh");
                Console.WriteLine("2. Duyệt đơn xin nghỉ");
                Console.WriteLine("3. Cập nhật sinh hoạt hàng ngày");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");
                string subChoice = Console.ReadLine() ?? "";

                if (subChoice == "0") break;
                Console.Clear();


                switch (subChoice)
                {

                    case "1":
                        Service.ThucHienDiemDanh(maLop, maGV);
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "2":
                        Service.DuyetDonXinNghi(maLop, maGV);
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "3":
                        Service.CapNhatNhatKy(maLop, maGV);
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

        private static void ShowTraCuuMenu(string maLop, GiaoVien_Service service, string Ten)
        {



            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

                Console.WriteLine("\n          --- Tra cứu ---");
                Console.WriteLine("1. Tra cứu HS toàn lớp");
                Console.WriteLine("2. Tìm kiếm HS theo tên");
                Console.WriteLine("3. Tra cứu Phụ huynh của lớp");
                Console.WriteLine("4. Thống kê điểm danh theo ngày");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");
                string subChoice = Console.ReadLine() ?? "";

                if (subChoice == "0") break;


                switch (subChoice)
                {

                    case "1":
                        service.TraCuuToanBoLop(maLop);
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "2":
                        Console.Write("Nhập tên học sinh cần tìm: ");
                        string ten = Console.ReadLine() ?? "";
                        service.TraCuuHocSinhTheoTen(maLop, ten);
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "3":
                        service.TraCuuPhuHuynh(maLop);
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "4":
                        Console.Write("Nhập ngày cần tra cứu (yyyy-mm-dd): ");
                        string ngay = Console.ReadLine() ?? DateTime.Now.ToString("yyyy-MM-dd");
                        service.TraCuuDiemDanh(maLop, ngay);
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