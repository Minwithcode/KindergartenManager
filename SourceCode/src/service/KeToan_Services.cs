using System;
using System.IO;
using System.Text;
using System.Data;
using KindergartenManager.Repository;
using KindergartenManager.Model;

namespace KindergartenManager.Service
{
    public class KeToanService
    {
        private readonly KeToanRepository _repo = new KeToanRepository();
        private readonly HocPhiRepository _repohp = new HocPhiRepository();
        private readonly MienGiamRepository _repomg = new MienGiamRepository();
        private readonly BaoCaoRepository _repobc = new BaoCaoRepository();
        public void XemDoanhThu(int thang, int nam)
        {
            decimal tong = _repobc.GetTongDoanhThu(thang, nam);
            Console.WriteLine($"\n>> TỔNG DOANH THU THÁNG {thang}/{nam}: {tong:N0} VNĐ");
            Console.WriteLine("Xem chi tiết hóa đơn (y)/Thoat(Any)");
            string choice = Console.ReadLine();




            if (choice == "y" || choice == "Y")
            {
                DataTable dt = _repobc.GetChiTietDoanhThu(thang, nam);
                Console.WriteLine("TRƯỜNG MẦM NON SỐ 1 DCV ");
                Console.WriteLine($"BÁO CÁO THU CHI CHI TIẾT - THÁNG {thang}/{nam}");
                Console.WriteLine($"Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}");
                Console.WriteLine();
                int stt = 1;
                foreach (DataRow r in dt.Rows)
                {
                    decimal soTien = Convert.ToDecimal(r["so_tien"]);

                    Console.WriteLine($"{stt}  {r["ma_hoc_phi"]}   {r["ho_ten"]}  {r["ma_phieu_thu"]}   {Convert.ToDateTime(r["ngay_lap"]):dd/MM/yyyy}   {soTien}");
                    stt++;
                }
                Console.ReadLine();

            }



        }

        public void HienThiDSHocPhiPhanLoai()
        {
            InBangHocPhi(_repohp.GetHocPhiUuTienChuaDong());
        }

        public void TimKiemHocPhi(string keyword)
        {
            InBangHocPhi(_repohp.FindHocPhi(keyword));
        }

        public void HienThiDSHocPhiNo()
        {
            DataTable dt = _repohp.GetHocPhiUuTienChuaDong();
            DataView dv = new DataView(dt) { RowFilter = "trang_thai = 'chua_thanh_toan'" };
            InBangHocPhi(dv.ToTable());
        }

        public void XuLyThanhToanVaThongBao(string maHP, string maKT)
        {
            string maPT = "PT-" + DateTime.Now.Ticks.ToString().Substring(12);
            if (_repohp.UpdateTrangThaiHocPhi(maHP, "da_thanh_toan", maKT))
            {
                _repohp.InsertPhieuThu(new PhieuThu { MaPhieuThu = maPT, HocPhiId = maHP, LapBoi = maKT, GhiChu = "Đã thu" });
                string idPH = _repohp.GetPhuHuynhIdTuHocPhi(maHP);
                if (!string.IsNullOrEmpty(idPH))
                {
                    NotificationService.GuiThongBao(maKT, idPH, "Học phí đã đóng", $"Mã HP {maHP} đã hoàn tất. Số phiếu: {maPT}", "hoc_phi");
                    NotificationFactory.Create("email").Send(idPH, $"Hóa đơn điện tử {maPT} đã được gửi.");
                    Console.WriteLine(" Thành công: Đã lập phiếu và gửi thông báo.");
                }
            }
            else Console.WriteLine(" Lỗi: Không thể cập nhật trạng thái học phí.");
        }


        public void ThemMienGiam(string id, string ld, float tl) => _repomg.ThemMienGiam(new MienGiam { MaMienGiam = id, LyDo = ld, TyLe = tl });
        public void CapNhatMienGiam(string id, string ld, float tl) => _repomg.SuaMienGiam(new MienGiam { MaMienGiam = id, LyDo = ld, TyLe = tl });
        public void XoaMienGiam(string id) => _repomg.XoaMienGiam(id);

        private void InBangHocPhi(DataTable dt)
        {
            Console.WriteLine("\n{0,-10} | {1,-20} | {2,-12} | {3}", "Mã HP", "Tên Trẻ", "Số Tiền", "Trạng Thái");
            foreach (DataRow r in dt.Rows)
                Console.WriteLine("{0,-10} | {1,-20} | {2,-12:N0} | {3}", r["ma_hoc_phi"], r["ten_tre"], r["so_tien"], r["trang_thai"]);
        }



        public string SinhMaTuDong(string tableName, string columnId, string prefix)
        {
            string lastId = _repo.GetLastId(tableName, columnId, prefix);
            if (string.IsNullOrEmpty(lastId)) return prefix + "001";

            int number = int.Parse(lastId.Substring(prefix.Length)) + 1;
            return prefix + number.ToString("D3");
        }


        private string ChonMienGiam()
        {
            DataTable dt = _repomg.GetDanhSachMienGiam();

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine(" Không có diện miễn giảm.");
                return null;
            }

            Console.WriteLine("\n--- DANH SÁCH MIỄN GIẢM ---");
            Console.WriteLine("0. Không áp dụng");

            int i = 1;
            foreach (DataRow r in dt.Rows)
            {
                Console.WriteLine($"{i}. {r["ly_do"]} - {(float)r["ty_le"] * 100}%");
                i++;
            }

            Console.Write("Chọn miễn giảm: ");
            int chon = int.Parse(Console.ReadLine() ?? "0");

            if (chon <= 0 || chon > dt.Rows.Count) return null;

            return dt.Rows[chon - 1]["ma_mien_giam"].ToString();
        }

        public void TaoMoiHocPhi(string treId, int thang, int nam, decimal soTien, string maKT)
        {
            string maHP = SinhMaTuDong("hoc_phi", "ma_hoc_phi", "HP");


            string maMG = ChonMienGiam();

            HocPhi hp = new HocPhi
            {
                MaHocPhi = maHP,
                TreId = treId,
                Thang = thang,
                Nam = nam,
                SoTien = soTien,
                MienGiamId = maMG
            };

            if (_repohp.InsertHocPhi(hp, maKT))
            {
                string idPH = _repohp.GetPhuHuynhIdTuHocPhi(maHP);

                if (!string.IsNullOrEmpty(idPH))
                {
                    NotificationService.GuiThongBao(
                        maKT,
                        idPH,
                        "Thông báo học phí",
                        $"Học phí tháng {thang}/{nam} của bé đã được khởi tạo. " +
                        $"Số tiền: {soTien:N0}đ. Vui lòng thanh toán sớm.",
                        "hoc_phi"
                    );
                }

                Console.WriteLine($" Đã tạo mã học phí {maHP}.");
            }
            else
            {
                Console.WriteLine(" Lỗi: Không thể tạo học phí.");
            }
        }

        public void XuatBaoCaoExcel(int thang, int nam)
        {
            DataTable dt = _repobc.GetChiTietDoanhThu(thang, nam);
            string fileName = $"BaoCao_TaiChinh_T{thang}_{nam}.csv";

            try
            {
               
                using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8))
                {
                    
                    sw.WriteLine("TRƯỜNG MẦM NON SỐ 1 DCV");
                    sw.WriteLine($"BÁO CÁO THU CHI CHI TIẾT - THÁNG {thang}/{nam}");
                    sw.WriteLine($"Ngày xuất bản:; {DateTime.Now:dd/MM/yyyy HH:mm}");
                    sw.WriteLine();

                    sw.WriteLine("STT;Mã Hóa Đơn;Tên Trẻ;Mã Phiếu Thu;Ngày Lập;Số Tiền (VNĐ)");

                    decimal tongCong = 0;
                    int stt = 1;

                    foreach (DataRow r in dt.Rows)
                    {
                        decimal soTien = Convert.ToDecimal(r["so_tien"]);
                        tongCong += soTien;

                        sw.WriteLine($"{stt};{r["ma_hoc_phi"]};{r["ho_ten"]};{r["ma_phieu_thu"]};{Convert.ToDateTime(r["ngay_lap"]):dd/MM/yyyy};{soTien}");
                        stt++;
                    }

                    sw.WriteLine();
                    sw.WriteLine($";;;;TỔNG CỘNG:;{tongCong}");
                    sw.WriteLine();
                    sw.WriteLine(";;;Người lập biểu;;;Kế toán trưởng");
                    sw.WriteLine(";;;(Ký và ghi rõ họ tên);;;(Ký và ghi rõ họ tên)");
                }
                Console.WriteLine($"\n Đã xuất file thành công: {Path.GetFullPath(fileName)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xuất file: " + ex.Message);
            }
        }

        public void HienThiDSHocPhiMienGiam()
        {
            DataTable dt = _repomg.GetAllMienGiam();

            Console.WriteLine("\n--- DANH SÁCH MIỄN GIẢM ---");
            foreach (DataRow r in dt.Rows)
            {
                Console.WriteLine($"{r["ma_mien_giam"]} - {r["ly_do"]} ({Convert.ToSingle(r["ty_le"]) * 100}%)");
            }
        }

        public string ChonMienGiamVaTinhTien(decimal tienGoc, out decimal tienSauGiam)
        {
            DataTable dt = _repomg.GetAllMienGiam();
            tienSauGiam = tienGoc;

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine(" Không có miễn giảm.");
                return null;
            }

            Console.WriteLine("\n--- DANH SÁCH MIỄN GIẢM ---");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {dt.Rows[i]["ma_mien_giam"]} - {dt.Rows[i]["ly_do"]} ({Convert.ToSingle(dt.Rows[i]["ty_le"]) * 100}%)");
            }
            Console.WriteLine("0. Không áp dụng miễn giảm");

            Console.Write("Chọn: ");
            if (!int.TryParse(Console.ReadLine(), out int chon) || chon < 0 || chon > dt.Rows.Count)
            {
                Console.WriteLine(" Lựa chọn không hợp lệ.");
                return null;
            }

            if (chon == 0) return null;

            DataRow r = dt.Rows[chon - 1];
            float tyLe = Convert.ToSingle(r["ty_le"]);

            tienSauGiam = tienGoc * (decimal)(1 - tyLe);
            return r["ma_mien_giam"].ToString();
        }
    }
}