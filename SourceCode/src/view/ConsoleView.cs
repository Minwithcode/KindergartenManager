using System;
using KindergartenManager.Model;
using KindergartenManager.Service;
using KindergartenManager.Repository;

namespace KindergartenManager.View
{
    public class ConsoleView
    {
        private readonly Services _services;

        public ConsoleView(Services services)
        {
            _services = services;
        }

        public void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Đăng nhập");
                Console.WriteLine("0. Thoát hệ thống");
                Console.Write("Chọn chức năng: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "1") HandleLogin();
                else if (choice == "0") return;
            }
        }

        private void HandleLogin()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("         Trường mầm non số 1 dcv          ");
            Console.WriteLine("==========================================");
            Console.WriteLine("      --- ĐĂNG NHẬP HỆ THỐNG ---");
            Console.Write("Tên đăng nhập: ");
            string user = Console.ReadLine() ?? "";
            Console.Write("Mật khẩu: ");
            string pass = Console.ReadLine() ?? "";

            NguoiDung? User = _services.Authenticate(user, pass);
            if (User != null)
            {
                if (User.TrangThai == "hoat_dong")
                {
                     Console.WriteLine($"\nChào mừng {User.HoTen}!");
                    ShowUserMenu(User);
                }
                else
                {
                    Console.WriteLine("Tài khoản của bạn đã bị xóa hoặc khóa. \nVui lòng liên hệ với quản trị viên hoặc nhà trường để được giải đáp thắc mắc.");
                    Console.ReadKey();
                }

            }
            else
            {
                Console.WriteLine("\nSai tên đăng nhập hoặc mật khẩu! Nhấn phím bất kỳ...");
                Console.ReadKey();
            }
        }

        private void ShowUserMenu(NguoiDung user)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("         Trường mầm non số 1 dcv          ");
                Console.WriteLine("==========================================");
                Console.WriteLine($"   HỆ THỐNG NHÀ TRẺ  |    {user.HoTen.ToUpper()}");

                user.HienThiMenu();
                Console.WriteLine("a. Đổi mật khẩu");
                Console.WriteLine("b. Xem thông tin cá nhân");
                Console.WriteLine("0. Đăng xuất");
                Console.Write("Chọn chức năng: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "0") break;

                switch (choice)
                {
                    case "b":
                        Console.Clear();
                        user.HienThiThongTin();
                        Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
                        Console.ReadKey();
                        break;
                    case "a":
                        HandleChangePassword(user);
                        break;
                    default:
                        HandleRoleSpecificChoice(user, choice);
                        break;
                }
            }
        }

        private void HandleRoleSpecificChoice(NguoiDung user, string choice)
        {
            if (user is QuanTriVien qtv)
            {
                Console_QT.ConsoleQT(qtv, choice);
            }
            else if (user is GiaoVien gv)
            {
                Console_GV.ConsoleGV(gv, choice);
            }
            else if (user is PhuHuynh ph)
            {
                Console_PH.ConsolePH(ph, choice);
            }
            else if (user is KeToan kt)
            {
                Console_KT.ConsoleKT(kt, choice);
            }
            else
            {
                Console.WriteLine("Vui long chon lai!!!");
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        private void HandleChangePassword(NguoiDung user)
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("         Trường mầm non số 1 dcv          ");
            Console.WriteLine("==========================================");
            Console.WriteLine($"  HỆ THỐNG NHÀ TRẺ  |  {user.HoTen.ToUpper()}");

            Console.WriteLine("\n          --- ĐỔI MẬT KHẨU ---");
            Console.Write("Nhập mật khẩu cũ: ");
            string old = Console.ReadLine() ?? "";
            Console.Write("Nhập mật khẩu mới: ");
            string n1 = Console.ReadLine() ?? "";
            Console.Write("Xác nhận mật khẩu mới: ");
            string n2 = Console.ReadLine() ?? "";

            string result = _services.ChangePassword(user, old, n1, n2);
            Console.WriteLine(result);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }
    }
}