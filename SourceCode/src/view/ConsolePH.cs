using System;
using KindergartenManager.Model;
using KindergartenManager.Service;
using KindergartenManager.Repository;

namespace KindergartenManager.View
{
    public class Console_PH
    {
        public static void ConsolePH(PhuHuynh ph, string choice)
        {
            var repo = new PhuHuynhRepository();
            PhuHuynh_Service phService = new PhuHuynh_Service();
            Services Services = new Services();
            switch (choice)
            {
                case "1": 
                    phService.TraCuuTre(ph.MaNguoiDung);
                    break;

                case "2": 
                    XemNhatKy(ph.MaNguoiDung, phService, ph.HoTen);
                    break;

                case "3":
                    Services.XemThongBao(ph.MaNguoiDung);
                    break;
            }

        }
        private static void XemNhatKy(string maPH, PhuHuynh_Service Service, string Ten)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

                Console.WriteLine("\n        --- Nhật ký sinh hoạt ---");
                Console.WriteLine("1. Thông tin hàng ngày");
                Console.WriteLine("2. Đơn xin nghỉ học");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");

                string subChoice = Console.ReadLine() ?? "";

                if (subChoice == "0") break;
                Console.Clear();
                switch (subChoice)
                {
                    case "1":
                        Service.XemThongTinSinhHoat(maPH);
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "2":
                        ShowGuiDon(maPH, Service, Ten);
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

        private static void ShowGuiDon(string maPH, PhuHuynh_Service Service, string Ten)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {Ten.ToUpper()}");

                Console.WriteLine("\n        --- Đơn xin nghỉ ---");
                Console.WriteLine("1. Tạo đơn mới");
                Console.WriteLine("2. Xem đơn đã gửi");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");

                string subChoice = Console.ReadLine() ?? "";

                if (subChoice == "0") break;
                Console.Clear();
                switch (subChoice)
                {
                    case "1":
                        Service.GuiDonXinNghi(maPH);
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                        break;
                    case "2":
                        Service.XemDonDaGui(maPH);
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