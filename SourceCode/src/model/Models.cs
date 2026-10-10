using System;
using System.Collections.Generic;
using KindergartenManager.Repository;
namespace KindergartenManager.Model
{
 
    public interface INotification
    {
        void Send(string recipient, string message);
    }

    public class EmailNotification : INotification
    {
        public void Send(string recipient, string message)
        {
            Console.WriteLine($"[EMAIL SENT] To: {recipient} | Content: {message}");
        }
    }

    public static class NotificationFactory
    {
        public static INotification Create(string type)
        {
            return type.ToLower() switch
            {
                "email" => new EmailNotification(),
                _ => new EmailNotification()
            };
        }
    }
    public static class NotificationService
    {
                public static void GuiThongBao(string NguoiGuiid, string nguoiNhanId, string tieuDe, string noiDung, string loai)
        {
            ThongBao tb = new ThongBao
            {
                NguoiGuiId = NguoiGuiid,
                NguoiNhanId = nguoiNhanId,
                Loai = loai,
                TieuDe = tieuDe,
                NoiDung = noiDung,
                ThoiGian = DateTime.Now,
                TrangThai = "chua_xem"
            };
            ThongBaoRepository.ThemThongBao(tb);
        }
    }


    public interface IAttendanceObserver
    {
        void Update(string tenTre, string trangThai);
    }


    #region ENUMs (Mapping MySQL)

    public enum TrangThaiNguoiDung
    {
        hoat_dong = 1,
        khong_hoat_dong = 0
    }

    public enum GioiTinh
    {
        Nam = 1,
        Nu = 2,
        Khac = 3
    }

    public enum TrangThaiTre
    {
        dang_theo_hoc = 1,
        da_ra_truong = 2
    }

    public enum TrangThaiDiemDanh
    {
        co_mat = 1,
        vang = 2,
        nghi_phep = 3,
        vang_khong_ly_do = 4
    }

    public enum TrangThaiDonNghi
    {
        cho_duyet = 0,
        duyet = 1,
        tu_choi = 2
    }

    public enum TrangThaiHocPhi
    {
        chua_thanh_toan = 0,
        da_thanh_toan = 1,
        tam_giu = 2
    }

    public enum TrangThaiThongBao
    {
        chua_xem = 0,
        da_xem = 1
    }

    public enum LoaiThongBao
    {
        he_thong = 1,
        hoc_phi = 2,
        diem_danh = 3,
        don_nghi = 4,
        sinh_hoat = 5,
        khac = 6
    }

    #endregion


    #region User Classes (Inheritance)

    public abstract class NguoiDung 
    {
        public string MaNguoiDung { get; set; } = string.Empty; 
        public string HoTen { get; set; } = string.Empty;     
        public DateTime NgaySinh { get; set; }  
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;   
        public string Email { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty; 
        public string TrangThai { get; set; } = string.Empty;   
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }

        public abstract void HienThiMenu();

        public virtual void HienThiThongTin()
        {
            Console.WriteLine("\n          === THÔNG TIN CÁ NHÂN ===\n");
            Console.WriteLine($"Họ tên: {HoTen}        Ngày sinh: {NgaySinh:dd/MM/yyyy}");
            Console.WriteLine($"Mã số: {MaNguoiDung}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"SĐT: {SoDienThoai}");
            Console.WriteLine($"Trạng thái: {TrangThai}");
        }
    }

    public class QuanTriVien : NguoiDung
    {
        public override void HienThiMenu()
        {
            Console.WriteLine("\n--- MENU QUẢN TRỊ VIÊN ---");
            Console.WriteLine("1. Quản lý người dùng");
            Console.WriteLine("2. Quản lý trẻ");
            Console.WriteLine("3. Quản lý lớp học");
            Console.WriteLine("4. Xem báo cáo");
            Console.WriteLine("5. Quản lý thông báo");
        }
    }

    public class KeToan : NguoiDung
    {
               public string MaChungChiHanhNghe { get; set; } = string.Empty;
        public string PhongBan { get; set; } = string.Empty;
        public override void HienThiMenu()
        {
            Console.WriteLine("\n--- MENU KẾ TOÁN ---");
            Console.WriteLine("1. Quản lý học phí");
            Console.WriteLine("2. Thống kê doanh thu");
            Console.WriteLine("3. Thông báo");
        }
        public override void HienThiThongTin()
        {
            base.HienThiThongTin();
            Console.WriteLine($"Phòng ban: {PhongBan}");
            Console.WriteLine($"Chứng chỉ: {MaChungChiHanhNghe}");
        }
    }

    public class GiaoVien : NguoiDung
    {
        public string TrinhDo { get; set; } = string.Empty;
        public string ChuyenMon { get; set; } = string.Empty;
        public DateTime? NgayVaoLam { get; set; }
        public string GhiChu { get; set; } = string.Empty;
        public override void HienThiMenu()
        {
            Console.WriteLine("\n--- MENU GIÁO VIÊN ---");
            Console.WriteLine("1. Tra cứu hồ sơ trẻ");
            Console.WriteLine("2. Ghi nhận sinh hoạt hàng ngày");
            Console.WriteLine("3. Quản lý Thông báo");
        }
        public override void HienThiThongTin()
        {
            base.HienThiThongTin(); 
            Console.WriteLine($"Trình độ: {TrinhDo}");
            Console.WriteLine($"Chuyên môn: {ChuyenMon}");
            Console.WriteLine($"Ngày vào làm: {NgayVaoLam:dd/MM/yyyy}");
        }
    }

    public class PhuHuynh : NguoiDung, IAttendanceObserver
    {
        
        public string NgheNghiep { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public override void HienThiMenu()
        {
            Console.WriteLine("\n--- MENU PHỤ HUYNH ---");
            Console.WriteLine("1. Xem thông tin của trẻ");
            Console.WriteLine("2. Xem nhật ký sinh hoạt");
            Console.WriteLine("3. Quản lý thông báo");
        }
        public override void HienThiThongTin()
        {
            base.HienThiThongTin();
            Console.WriteLine($"Nghề nghiệp: {NgheNghiep}");
            Console.WriteLine($"Địa chỉ: {DiaChi}");
            Console.WriteLine($"Ghi chú: {GhiChu}");
        }
        public void Update(string tenTre, string trangThai)
        {
            if (trangThai == "Vắng")
            {
                var notifier = NotificationFactory.Create("email");
                notifier.Send(this.Email, $"Thông báo: Bé {tenTre} vắng mặt hôm nay. Vui lòng liên hệ giáo viên nếu cần.");
            }
        }
    }

    #endregion

    #region Management Classes

    public class Tre
    {
        public string MaTre { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string TinhTrang { get; set; } = string.Empty;
        public string GioiTinh { get; set; } = string.Empty;   
        public string PhuHuynhId { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public string TrangThai { get; set; } = string.Empty;   
    }

    public class LopHoc
    {
        public string MaLopHoc { get; set; } = string.Empty;
        public string TenLop { get; set; } = string.Empty;
        public string GiaoVienId { get; set; } = string.Empty;
        public string NamHoc { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
    }

    public class HocTai
    {
        public string MaTre { get; set; } = string.Empty;
        public string MaLopHoc { get; set; } = string.Empty;
        public string NamHoc { get; set; } = string.Empty;
    }

    public class DiemDanh
    {
        public string MaDiemDanh { get; set; } = string.Empty;
        public string TreDd { get; set; } = string.Empty;
        public DateTime Ngay { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public string GiaoVienDd { get; set; } = string.Empty;
    }

    public class NhatKySinhHoat
    {
        public string MaCapNhat { get; set; } = string.Empty;
        public string TreId { get; set; } = string.Empty;
        public DateTime NgayCapNhat { get; set; }
        public string AnUong { get; set; } = string.Empty;
        public string SucKhoe { get; set; } = string.Empty;
        public string NhanXet { get; set; } = string.Empty;
        public string GiaoVienId { get; set; } = string.Empty;
    }

    public class DonXinNghi
    {
        public string MaDon { get; set; } = string.Empty;
        public string TreId { get; set; } = string.Empty;
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string LyDo { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty; 
        public DateTime NgayGui { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string DuyetBoi { get; set; } = string.Empty;
    }

    public class MienGiam
    {
        public string MaMienGiam { get; set; } = string.Empty;
        public string LyDo { get; set; } = string.Empty;
        public float TyLe { get; set; } 
        public string GhiChu { get; set; } = string.Empty;
    }

    public class HocPhi
    {
        public string MaHocPhi { get; set; } = string.Empty;
        public string TreId { get; set; } = string.Empty;
        public int Thang { get; set; }
        public int Nam { get; set; }
        public decimal SoTien { get; set; }
        public string MienGiamId { get; set; } = string.Empty;
        public string NguoiGui { get; set; } = string.Empty; 
        public string TrangThai { get; set; } = string.Empty;
    }

    public class PhieuThu
    {
        public string MaPhieuThu { get; set; } = string.Empty;
        public DateTime NgayLap { get; set; }
        public string HocPhiId { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public string LapBoi { get; set; } = string.Empty;
    }

    public class ThongBao
    {
        public int MaThongBao { get; set; }
        public string NguoiGuiId { get; set; } = string.Empty;
        public string NguoiNhanId { get; set; } = string.Empty;
        public string Loai { get; set; } = string.Empty;         
        public string TieuDe { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public DateTime ThoiGian { get; set; }
        public string TrangThai { get; set; } = string.Empty; 
    }

    #endregion
}