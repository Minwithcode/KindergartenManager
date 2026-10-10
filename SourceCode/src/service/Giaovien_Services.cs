using KindergartenManager.Model;
using KindergartenManager.Repository;
using System;
using System.Data;

namespace KindergartenManager.Service
{
    public class GiaoVien_Service
    {
        private readonly GiaoVienRepository _repo;
        private readonly DiemDanhRepository _repodd;
        private readonly TreRepository _repot;
        private readonly DonXinNghiRepository _repodn;

        public GiaoVien_Service()
        {
            _repo = new GiaoVienRepository();
            _repodd = new DiemDanhRepository();
            _repot = new TreRepository();
            _repodn = new DonXinNghiRepository();
        }

        public void TraCuuDiemDanh(string maLop, string ngay)
        {
            DataTable dt = _repodd.GetBaoCaoDiemDanhTheoNgay(maLop, ngay);

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("Không có dữ liệu.");
                return;
            }

            foreach (DataRow r in dt.Rows)
            {
                if (Enum.TryParse<TrangThaiDiemDanh>(
                        r["trang_thai"].ToString(),
                        true,
                        out var st))
                {
                    Console.WriteLine($"{r["ho_ten"]} : {st}");
                }
                else
                {
                    Console.WriteLine($"{r["ho_ten"]} : Không xác định");
                }
            }
        }


        public void ThucHienDiemDanh(string maLop, string maGV)
        {
            string homNay = DateTime.Now.ToString("yyyy-MM-dd");
            DataTable dsTre = _repodd.GetTreChuaDiemDanh(maLop, homNay);

            foreach (DataRow row in dsTre.Rows)
            {
                string maTre = row["ma_tre"]?.ToString() ?? "";
                string tenTre = row["ho_ten"]?.ToString() ?? "Học sinh";

                Console.WriteLine($"\n[Học sinh: {tenTre}]");
                Console.Write("Chọn (1: Có mặt, 2: Vắng, 0: Dừng): ");
                string c = Console.ReadLine() ?? "0";

                if (c == "0") break;

                TrangThaiDiemDanh trangThai = (c == "1") ? TrangThaiDiemDanh.co_mat : TrangThaiDiemDanh.vang;

                bool success = _repodd.LuuDiemDanh(maTre, trangThai.ToString(), homNay, maGV);


                if (success && trangThai == TrangThaiDiemDanh.vang)
                {
                    PhuHuynh? ph = _repot.GetPhuHuynhCuaTre(maTre);
                    if (ph != null)
                    {

                        NotificationService.GuiThongBao(maGV,
                            ph.MaNguoiDung,
                            $"Bé {tenTre} vắng mặt hôm nay",
                            $"Bé {tenTre} vắng mặt hôm nay. Vui lòng liên hệ giáo viên nếu cần.",
                            "diem_danh"
                        );
                    }
                }

            }
        }
        public void TraCuuHocSinhTheoTen(string maLop, string ten)
        {
            DataTable dt = _repot.SearchHocSinh(maLop, ten);
            InKetQuaTraCuu(dt, $"Kết quả tìm kiếm '{ten}':");
        }

        public void TraCuuToanBoLop(string maLop)
        {
            DataTable dt = _repot.GetDanhSachTreTheoLop(maLop);
            InKetQuaTraCuu(dt, "Danh sách học sinh:");
        }

        public void TraCuuPhuHuynh(string maLop)
        {
            DataTable dt = _repot.GetThongTinPhuHuynhTheoLop(maLop);

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("Không có phụ huynh nào trong lớp này.");
                return;
            }

            Console.WriteLine("\n--- THÔNG TIN PHỤ HUYNH ---\n");

            Console.WriteLine("{0,-25} | {1,-25} | {2,-15}",
                "Họ tên phụ huynh", "Tên trẻ", "Liên hệ");
            Console.WriteLine(new string('-', 70));

            foreach (DataRow r in dt.Rows)
            {
                Console.WriteLine("{0,-25} | {1,-25} | {2,-15}",
                    r["ho_ten"],
                    r["ten_tre"],
                    r["so_dien_thoai"]);
            }
        }

        private void InKetQuaTraCuu(DataTable dt, string title)
        {
            Console.WriteLine($"\n{title}");
            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("Không có dữ liệu.");
                return;
            }
            Console.WriteLine("{0,-10} | {1,-25} | {2,-12}",
     "Mã trẻ", "Họ và tên trẻ", "Ngày sinh");
            Console.WriteLine(new string('-', 55));

            foreach (DataRow r in dt.Rows)
            {
                Console.WriteLine("{0,-10} | {1,-25} | {2,-12}",
                    r["ma_tre"],
                    r["ho_ten"],
                    Convert.ToDateTime(r["ngay_sinh"]).ToString("dd/MM/yyyy"));
            }
        }

        public void CapNhatNhatKy(string maLop, string maGV)
        {
            string ngay = DateTime.Now.ToString("yyyy-MM-dd");

            DataTable dtDiemDanh = _repodd.GetBaoCaoDiemDanhTheoNgay(maLop, ngay);

            if (dtDiemDanh.Rows.Count == 0)
            {
                Console.WriteLine("Chưa có dữ liệu điểm danh hôm nay.");
                return;
            }

            foreach (DataRow r in dtDiemDanh.Rows)
            {
                string maTre = r["ma_tre"].ToString()!;
                string tenTre = r["ho_ten"].ToString()!;
                string trangThai = r["trang_thai"].ToString()!;

                if (trangThai.Equals(TrangThaiDiemDanh.vang.ToString(),
                                      StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"\nHọc sinh: {tenTre} (Vắng) → bỏ qua nhật ký");
                    continue;
                }

                DataRow? nk = _repot.GetNhatKyTreTheoNgay(maTre, ngay);

                if (nk != null)
                {
                    Console.WriteLine($"\n{tenTre} (Đã cập nhật)");
                    Console.WriteLine($"Ăn uống : {nk["an_uong"]}");
                    Console.WriteLine($"Sức khỏe: {nk["suc_khoe"]}");
                    Console.WriteLine($"Nhận xét: {nk["nhan_xet"]}");
                    continue;
                }

                Console.WriteLine($"\n{tenTre}");
                Console.Write("Ăn uống: ");
                string an = Console.ReadLine() ?? "";
                Console.Write("Sức khỏe: ");
                string sk = Console.ReadLine() ?? "";
                Console.Write("Nhận xét: ");
                string nx = Console.ReadLine() ?? "";

                _repot.InsertNhatKy(maTre, ngay, an, sk, nx, maGV);
            }

            Console.WriteLine("\nĐã cập nhật nhật ký cho các học sinh có mặt.");
        }

        public void DuyetDonXinNghi(string maLop, string maGV)
        {
            DataTable dt = _repodn.GetDonXinNghiChoDuyet(maLop);
            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("Không có đơn chờ duyệt.");
                return;
            }

            foreach (DataRow r in dt.Rows)
            {
                string maDon = r["ma_don"].ToString()!;
                string tenTre = r["ho_ten"].ToString()!;
                DataTable t = _repot.SearchHocSinh(maLop, tenTre);
                string maPH = t.Rows[0]["phu_huynh_id"].ToString()!;

                Console.WriteLine($"\nĐơn: {maDon}");
                Console.WriteLine($"Học sinh: {tenTre}");
                Console.WriteLine($"Lý do: {r["ly_do"]}");
                Console.Write("Duyệt (D) / Từ chối (T): ");
                string c = Console.ReadLine() ?? "";

                if (c.Equals("D", StringComparison.OrdinalIgnoreCase))
                {
                    _repodn.UpdateTrangThaiDon(maDon, TrangThaiDonNghi.duyet, maGV);

                    NotificationService.GuiThongBao(
                        maGV,
                        maPH,
                        "Đơn xin nghỉ được duyệt",
                        $"Đơn xin nghỉ của bé {tenTre} đã được giáo viên duyệt.",
                        LoaiThongBao.don_nghi.ToString()
                    );
                }
                else if (c.Equals("T", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("Nhập lý do từ chối: ");
                    string lyDoTC = Console.ReadLine() ?? "";

                    _repodn.UpdateTrangThaiDon(maDon, TrangThaiDonNghi.tu_choi, maGV);

                    NotificationService.GuiThongBao(
                        maGV,
                        maPH,
                        "Đơn xin nghỉ bị từ chối",
                        $"Đơn xin nghỉ của bé {tenTre} bị từ chối. Lý do: {lyDoTC}",
                        LoaiThongBao.don_nghi.ToString()
                    );
                }
            }

            Console.WriteLine("\nHoàn tất xử lý đơn.");
        }


    }

}
