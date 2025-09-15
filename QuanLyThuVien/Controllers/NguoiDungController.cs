using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThuVien.Models; // <-- THÊM DÒNG NÀY

namespace QuanLyThuVien.Controllers
{
    public class NguoiDungController : Controller
    {
        // Khởi tạo đối tượng context để kết nối đến DB
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: NguoiDung
        public ActionResult Index()
        {
            // Lấy danh sách tất cả người dùng hệ thống (từ bảng ThuThu)
            // Trong trường hợp này, chúng ta cần hiển thị cả người dùng "Bị khóa" (DaXoa = true)
            // nên không cần lọc theo điều kiện DaXoa.
            var danhSachNguoiDung = db.ThuThus.ToList();

            // Gửi danh sách đã lấy được đến View
            return View(danhSachNguoiDung);
        }
    }
}