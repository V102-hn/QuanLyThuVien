using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThuVien.ViewModels
{
    public class QuanLyMuonTraViewModel
    {
        // Sử dụng lại MuonTraViewModel bạn đã tạo để hiển thị danh sách
        public IEnumerable<MuonTraViewModel> DanhSachPhieuMuon { get; set; }

        // Dữ liệu cho các Dropdown trong Modal
        public IEnumerable<SelectListItem> DanhSachDocGia { get; set; }
        public IEnumerable<SelectListItem> DanhSachSach { get; set; }
    }
}