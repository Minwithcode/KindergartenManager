using KindergartenManager.Model;
using KindergartenManager.Repository;
using System;
using System.Data;

namespace KindergartenManager.Service
{
    public class PhuHuynh_Service
    {
        private readonly DonXinNghiRepository _repodn;
        private readonly TreRepository _repot;

        public PhuHuynh_Service()
        {
            _repodn = new DonXinNghiRepository();
            _repot = new TreRepository();

        }
        public void TraCuuTre(string maPH)
        {
            DataTable dt = _repot.GetThongTinTre(maPH);
            Console.WriteLine("             ===  THÔNG TIN TRẺ ===                           \n");
            foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine($"Mã trẻ: {row["ma_tre"]}");
                Console.WriteLine($"Tên trẻ: {row["ho_ten"]}      Ngày sinh: {Convert.ToDateTime(row["ngay_sinh"]):dd/MM/yyyy}");
                Console.WriteLine($"Giới tính: {row["gioi_tinh"]}");
                Console.WriteLine($"Tên lớp: {row["ten_lop"]}");
                Console.WriteLine($"Tên giáo viên: {row["co_giao"]}");
            }
        }
        public void GuiDonXinNghi(string maPH)
        {
            Console.Clear();
            Console.WriteLine("=== GỬI ĐƠN XIN NGHỈ ===");

            DataTable dsTre = _repot.GetDanhSachTre(maPH);
            if (dsTre.Rows.Count == 0)
            {
                Console.WriteLine("Không tìm thấy trẻ nào đang theo học.");
                return;
            }

            Console.WriteLine("\nDanh sách trẻ:");
            for (int i = 0; i < dsTre.Rows.Count; i++)
                Console.WriteLine($"{i + 1}. {dsTre.Rows[i]["ho_ten"]} ({dsTre.Rows[i]["ma_tre"]})");

            Console.Write("\nChọn số thứ tự trẻ: ");
            int chon = int.Parse(Console.ReadLine()!);
            if (chon < 1 || chon > dsTre.Rows.Count) return;

            string maTre = dsTre.Rows[chon - 1]["ma_tre"].ToString()!;
            string tenTre = dsTre.Rows[chon - 1]["ho_ten"].ToString()!;

            DataTable dtGV = _repot.GetGiaoVienPhuTrach(maTre);
            if (dtGV.Rows.Count == 0)
            {
                Console.WriteLine("Không xác định được giáo viên phụ trách.");
                return;
            }
            string maGV = dtGV.Rows[0]["giao_vien_id"].ToString()!;

            Console.Write("Ngày bắt đầu (dd/MM/yyyy): ");
            DateTime ngayBD = DateTime.ParseExact(Console.ReadLine()!, "dd/MM/yyyy", null);

            Console.Write("Ngày kết thúc (dd/MM/yyyy): ");
            DateTime ngayKT = DateTime.ParseExact(Console.ReadLine()!, "dd/MM/yyyy", null);

            Console.Write("Lý do nghỉ: ");
            string lyDo = Console.ReadLine() ?? "";

            bool ok = false;
            DonXinNghi don = new();

            for (int i = 0; i < 5 && !ok; i++) {
                don = new DonXinNghi
                {
                    MaDon = TaoMaDonXinNghi(),
                    TreId = maTre,
                    NgayBatDau = ngayBD,
                    NgayKetThuc = ngayKT,
                    LyDo = lyDo,
                    TrangThai = TrangThaiDonNghi.cho_duyet.ToString(),
                    NgayGui = DateTime.Now
                };

                ok = _repodn.InsertDon(don);
            }

            if (!ok)
            {
                Console.WriteLine("Gửi đơn thất bại (lỗi mã trùng).");
                return;
            }

            Console.WriteLine($"Đã gửi đơn xin nghỉ cho bé {tenTre}");

            NotificationService.GuiThongBao(
                maPH,
                maGV,
                "Đơn xin nghỉ",
                $"Phụ huynh gửi đơn xin nghỉ cho bé {tenTre} ({ngayBD:dd/MM} - {ngayKT:dd/MM}).",
                LoaiThongBao.don_nghi.ToString()
            );
        }

        public void XemDonDaGui(string maPH)
        {
            Console.Clear();
            Console.WriteLine("=== ĐƠN XIN NGHỈ ĐÃ GỬI ===\n");

            DataTable dt = _repodn.GetDonDaGui(maPH);

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine(" Chưa gửi đơn xin nghỉ nào.");
                return;
            }

            foreach (DataRow r in dt.Rows)
            {
                Console.WriteLine($"Mã đơn: {r["ma_don"]}");
                Console.WriteLine($"Trẻ: {r["ten_tre"]}");
                Console.WriteLine(
                    $"Thời gian nghỉ: {Convert.ToDateTime(r["ngay_bat_dau"]):dd/MM/yyyy} " +
                    $"→ {Convert.ToDateTime(r["ngay_ket_thuc"]):dd/MM/yyyy}"
                );
                Console.WriteLine($"Lý do: {r["ly_do"]}");
                Console.WriteLine($"Trạng thái: {HienThiTrangThai(r["trang_thai"].ToString()!)}");
                Console.WriteLine($"Ngày gửi: {Convert.ToDateTime(r["ngay_gui"]):dd/MM/yyyy HH:mm}");

                if (r["ngay_duyet"] != DBNull.Value)
                {
                    Console.WriteLine($"Ngày duyệt: {Convert.ToDateTime(r["ngay_duyet"]):dd/MM/yyyy}");
                    Console.WriteLine($"Duyệt bởi: {r["giao_vien_duyet"]}");
                }

                Console.WriteLine(new string('-', 40));
            }
        }
        private string HienThiTrangThai(string trangThai)
        {
            return trangThai switch
            {
                "cho_duyet" => "Chờ duyệt",
                "duyet" => "Đã duyệt",
                "tu_choi" => "Từ chối",
                _ => trangThai
            };
        }
        public static string TaoMaDonXinNghi()
        {   
            string timePart = DateTime.UtcNow.Ticks.ToString();
            timePart = timePart.Substring(timePart.Length - 4);

            string randomPart = Guid.NewGuid().ToString("N")
                                .Substring(0, 2)
                                .ToUpper();

            return "DN" + timePart + randomPart; 
        }

        public void XemThongTinSinhHoat(string maPH)
        {
            Console.Clear();
            Console.WriteLine("=== THÔNG TIN SINH HOẠT ===\n");

            DataTable dt = _repot.GetThongTinSinhHoat(maPH);

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("Chưa có thông tin sinh hoạt.");
                return;
            }

            foreach (DataRow r in dt.Rows)
            {
                Console.WriteLine($"Trẻ: {r["ten_tre"]}");
                Console.WriteLine($"Ngày: {Convert.ToDateTime(r["ngay_cap_nhat"]):dd/MM/yyyy}");
                Console.WriteLine($"Ăn uống: {r["an_uong"]}");
                Console.WriteLine($"Sức khỏe: {r["suc_khoe"]}");
                Console.WriteLine($"Nhận xét: {r["nhan_xet"]}");
                Console.WriteLine($"Giáo viên: {r["giao_vien"]}");
                Console.WriteLine(new string('-', 40));
            }
        }

    }


}
