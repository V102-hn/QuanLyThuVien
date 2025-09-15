using System;

namespace QuanLyThuVien.ViewModels
{
    public class ViPhamViewModel
    {
        public string MaViPham { get; set; }
        public string TenDocGia { get; set; }
        public string LoaiViPham { get; set; }
        public string SoTienPhat { get; set; } // Dùng string để có thể định dạng tiền tệ (vd: "30,000đ")
        public DateTime NgayGhiNhan { get; set; }
        public string TrangThai { get; set; } // Sẽ chứa "Chưa thanh toán" hoặc "Đã thanh toán"
    }
}