using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien.ViewModels
{
    // Lớp nhỏ để chứa thông tin Top sách
    public class TopSachViewModel
    {
        public int Hang { get; set; }
        public string TenSach { get; set; }
        public string TenTacGia { get; set; }
        public int LuotMuon { get; set; }
    }

    // Lớp ViewModel chính cho trang Báo cáo
    public class BaoCaoViewModel
    {
        // 1. Các chỉ số tổng quan (Stats Cards)
        public int LuotMuonTrongThang { get; set; }
        public int SachDangQuaHan { get; set; }
        public int DocGiaMoiTrongThang { get; set; }
        public int SachMoiDuocNhap { get; set; }

        // 2. Dữ liệu cho các biểu đồ
        // Biểu đồ lượt mượn (6 tháng)
        public List<string> BieuDoLuotMuon_Nhan { get; set; } // Nhãn: Tháng 1, Tháng 2...
        public List<int> BieuDoLuotMuon_DuLieu { get; set; }  // Dữ liệu: số lượt mượn

        // Biểu đồ cơ cấu thể loại
        public List<string> BieuDoTheLoai_Nhan { get; set; }     // Nhãn: Khoa học, Văn học...
        public List<int> BieuDoTheLoai_DuLieu { get; set; }      // Dữ liệu: số lượng sách

        // 3. Danh sách Top 10
        public List<TopSachViewModel> Top10SachDuocMuon { get; set; }

        public BaoCaoViewModel()
        {
            // Khởi tạo các list để tránh lỗi null
            BieuDoLuotMuon_Nhan = new List<string>();
            BieuDoLuotMuon_DuLieu = new List<int>();
            BieuDoTheLoai_Nhan = new List<string>();
            BieuDoTheLoai_DuLieu = new List<int>();
            Top10SachDuocMuon = new List<TopSachViewModel>();
        }
    }
}