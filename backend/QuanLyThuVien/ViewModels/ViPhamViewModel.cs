using System;

namespace QuanLyThuVien.ViewModels
{
    public class ViPhamViewModel
    {
        public int MaViPhamRaw { get; set; } // Giữ lại ID gốc để dùng cho việc sửa/xóa
        public string MaViPham { get; set; }
        public string TenDocGia { get; set; }
        public string LoaiViPham { get; set; }
        public string SoTienPhat { get; set; }
        public DateTime NgayGhiNhan { get; set; }
        public string TrangThai { get; set; }
    }
}