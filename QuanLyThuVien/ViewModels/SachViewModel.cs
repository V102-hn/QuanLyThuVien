using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; // Cần cho SelectListItem
using QuanLyThuVien.Models;

namespace QuanLyThuVien.ViewModels
{
    public class SachViewModel
    {
        // Dùng để hiển thị danh sách sách trong bảng
        public IEnumerable<Sach> DanhSachSach { get; set; }

        // Dùng cho form modal "Thêm/Sửa"
        public Sach Sach { get; set; }
        public IEnumerable<SelectListItem> DanhSachNXB { get; set; }
        public IEnumerable<SelectListItem> DanhSachTheLoai { get; set; }

        // Dùng để nhận danh sách các thể loại được chọn từ form (vì là quan hệ nhiều-nhiều)
        public int[] SelectedTheLoaiIds { get; set; }
    }
}