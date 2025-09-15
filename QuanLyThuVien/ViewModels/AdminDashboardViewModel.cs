using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien.ViewModels
{
   // ViewModel cho mỗi dòng trong bảng "Hoạt động gần đây"
    public class RecentActivityViewModel
    {
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public string TenDocGia { get; set; }
        public DateTime NgayMuon { get; set; }
        public string TrangThai { get; set; }
    }

    // ViewModel chính cho toàn bộ trang Dashboard
    public class AdminDashboardViewModel
    {
        public int TongSoSach { get; set; }
        public int TongSoDocGia { get; set; }
        public int SachDangMuon { get; set; }
        public int SachQuaHan { get; set; }
        public List<RecentActivityViewModel> HoatDongGanDay { get; set; }

        public AdminDashboardViewModel()
        {
            HoatDongGanDay = new List<RecentActivityViewModel>();
        }
    }
}