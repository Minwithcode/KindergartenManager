 CREATE DATABASE IF NOT EXISTS quan_ly_nha_tre;

USE quan_ly_nha_tre;

CREATE TABLE IF NOT EXISTS nguoi_dung (
    ma_nguoi_dung VARCHAR(8)PRIMARY KEY,
    ho_ten VARCHAR(50) NOT NULL,
    ngay_sinh DATE NOT NULL,
    ten_dang_nhap VARCHAR(50) NOT NULL UNIQUE,
    mat_khau VARCHAR(255) NOT NULL,
    email VARCHAR(50),
    so_dien_thoai VARCHAR(10),
    trang_thai ENUM('Hoạt động','Không hoạt động') NOT NULL,
    ngay_tao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ngay_cap_nhat TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS phu_huynh (
    ma_phu_huynh VARCHAR(8) PRIMARY KEY,
    nghe_nghiep VARCHAR(30),
    dia_chi VARCHAR(100),
    ghi_chu TEXT,
    FOREIGN KEY (ma_phu_huynh) REFERENCES nguoi_dung(ma_nguoi_dung) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS giao_vien (
    ma_giao_vien VARCHAR(8) PRIMARY KEY,
    trinh_do VARCHAR(30),
    chuyen_mon VARCHAR(50),
    ngay_vao_lam DATE,
    ghi_chu TEXT,
    FOREIGN KEY (ma_giao_vien) REFERENCES nguoi_dung(ma_nguoi_dung) ON DELETE CASCADE

);

CREATE TABLE IF NOT EXISTS quan_tri_vien (
    ma_quan_tri VARCHAR(8) PRIMARY KEY,
    FOREIGN KEY (ma_quan_tri) REFERENCES nguoi_dung(ma_nguoi_dung) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ke_toan (
    ma_ke_toan VARCHAR(8) PRIMARY KEY,
	ma_chung_chi_hanh_nghe VARCHAR(50),
    phong_ban VARCHAR(30),
    FOREIGN KEY (ma_ke_toan) REFERENCES nguoi_dung(ma_nguoi_dung) ON DELETE CASCADE
);



CREATE TABLE IF NOT EXISTS tre (
    ma_tre VARCHAR(8) PRIMARY KEY,
    ho_ten VARCHAR(200) NOT NULL,
    ngay_sinh DATE CHECK (ngay_sinh <= CURRENT_DATE AND ngay_sinh >= DATE_SUB(CURRENT_DATE, INTERVAL 10 YEAR)),
    tinh_trang VARCHAR(50), 
    gioi_tinh ENUM('Nam', 'Nu', 'Khac'),
    phu_huynh_id VARCHAR(8),
    ngay_tao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    trang_thai ENUM('Đang theo học','Đã ra trường') NOT NULL,
	FOREIGN KEY (phu_huynh_id)  REFERENCES phu_huynh(ma_phu_huynh) ON DELETE CASCADE ON UPDATE CASCADE
);

/***************************

 3. LopHoc và phân công

***************************/

CREATE TABLE IF NOT EXISTS lop_hoc (
    ma_lop_hoc VARCHAR(8) PRIMARY KEY,
    ten_lop VARCHAR(200) NOT NULL,
    giao_vien_id VARCHAR(8),
    nam_hoc VARCHAR(50),
    ghi_chu TEXT,
	FOREIGN KEY (giao_vien_id)  REFERENCES giao_vien(ma_giao_vien) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE IF NOT EXISTS hoc_tai (
    ma_tre VARCHAR(8),
	ma_lop_hoc VARCHAR(8),
    nam_hoc VARCHAR(50),
	PRIMARY KEY (ma_lop_hoc, ma_tre),
    FOREIGN KEY (ma_lop_hoc) REFERENCES lop_hoc(ma_lop_hoc) ON DELETE CASCADE,
    FOREIGN KEY (ma_tre) REFERENCES tre(ma_tre) ON DELETE CASCADE
);
ALTER TABLE hoc_tai ADD UNIQUE (ma_lop_hoc, ma_tre, nam_hoc);

CREATE TABLE IF NOT EXISTS diem_danh (
    ma_diem_danh VARCHAR(8) PRIMARY KEY,
    tre_dd VARCHAR(8),
    ngay DATE NOT NULL,
    trang_thai ENUM('co_mat','vang','nghi_phep','vang_khong_ly_do') NOT NULL,
    ghi_chu TEXT,
    giao_vien_dd VARCHAR(8),
    FOREIGN KEY (tre_dd) REFERENCES tre(ma_tre) ON DELETE CASCADE,
    FOREIGN KEY (giao_vien_dd) REFERENCES giao_vien(ma_giao_vien)

);

ALTER TABLE diem_danh ADD UNIQUE (tre_dd, ngay);

CREATE TABLE IF NOT EXISTS don_xin_nghi (
    ma_don VARCHAR(8) PRIMARY KEY,
    tre_id VARCHAR(8),
    ngay_bat_dau DATE,
    ngay_ket_thuc DATE,
    ly_do TEXT,
    trang_thai ENUM('cho_duyet','duyet','tu_choi') DEFAULT 'cho_duyet',
    ngay_gui TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ngay_duyet TIMESTAMP NULL,
    duyet_boi VARCHAR(8),
    FOREIGN KEY (tre_id) REFERENCES tre(ma_tre) ON DELETE CASCADE,
    FOREIGN KEY (duyet_boi) REFERENCES giao_vien(ma_giao_vien)

);

CREATE TABLE IF NOT EXISTS cap_nhat_tt (
    ma_cap_nhat VARCHAR(8) PRIMARY KEY,
    tre_id VARCHAR(8),
    ngay_cap_nhat DATE,
    an_uong TEXT,
    suc_khoe TEXT,
    nhan_xet TEXT,
    giao_vien_id VARCHAR(8),
    FOREIGN KEY (tre_id) REFERENCES tre(ma_tre) ON DELETE CASCADE,
    FOREIGN KEY (giao_vien_id) REFERENCES giao_vien(ma_giao_vien)
);

ALTER TABLE cap_nhat_tt ADD UNIQUE (tre_id, ngay_cap_nhat);

CREATE TABLE IF NOT EXISTS mien_giam (
    ma_mien_giam VARCHAR(8) PRIMARY KEY,
    ly_do TEXT,
    ty_le FLOAT, 
    ghi_chu TEXT
);

CREATE TABLE IF NOT EXISTS hoc_phi (
    ma_hoc_phi VARCHAR(8) PRIMARY KEY ,
    tre_id VARCHAR(8),
    thang INT NOT NULL,
    nam INT NOT NULL,
    so_tien DECIMAL(12,2) NOT NULL,
    mien_giam_id VARCHAR(8),
    nguoi_gui VARCHAR(8),
    trang_thai ENUM('chua_thanh_toan','da_thanh_toan','tam_giu') DEFAULT 'chua_thanh_toan',
    FOREIGN KEY (tre_id) REFERENCES tre(ma_tre) ON DELETE CASCADE,
	FOREIGN KEY (mien_giam_id) REFERENCES mien_giam(ma_mien_giam) ON DELETE CASCADE,
    FOREIGN KEY (nguoi_gui) REFERENCES ke_toan(ma_ke_toan) ON DELETE CASCADE
);

ALTER TABLE hoc_phi ADD UNIQUE (tre_id, thang, nam);

CREATE TABLE IF NOT EXISTS phieu_thu (
    ma_phieu_thu VARCHAR(8) PRIMARY KEY,
    ngay_lap TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    hoc_phi_id VARCHAR(8),
    ghi_chu TEXT,
    lap_boi VARCHAR(8),
    FOREIGN KEY (hoc_phi_id) REFERENCES hoc_phi(ma_hoc_phi) ON DELETE SET NULL,
    FOREIGN KEY (lap_boi) REFERENCES ke_toan(ma_ke_toan)
);
CREATE TABLE thong_bao (
    ma_thong_bao INT PRIMARY KEY AUTO_INCREMENT,
    nguoi_gui_id VARCHAR(8),
    nguoi_nhan_id VARCHAR(8),
    loai ENUM('he_thong','hoc_phi','diem_danh','don_nghi','sinh_hoat','khac') DEFAULT 'khac',
    tieu_de VARCHAR(255) NOT NULL,
    noi_dung TEXT NOT NULL,
    thoi_gian DATETIME DEFAULT CURRENT_TIMESTAMP,
    trang_thai ENUM('chua_xem','da_xem') DEFAULT 'chua_xem',
    FOREIGN KEY (nguoi_gui_id) REFERENCES nguoi_dung(ma_nguoi_dung),
    FOREIGN KEY (nguoi_nhan_id) REFERENCES nguoi_dung(ma_nguoi_dung)
);
